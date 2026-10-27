using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Runtime.InteropServices;
using Heroes.SDK.API;
using Heroes.SDK.Classes.NativeClasses;
using Heroes.SDK.Classes.PseudoNativeClasses;
using HeroesPowerPlant.RemoteControl.Shared;
using HeroesPowerPlant.RemoteControl.Shared.Messages;
using LiteNetLib;
using Reloaded.Hooks.Definitions;
using Reloaded.Messaging;
using Reloaded.Messaging.Host.LiteNetLib;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Utilities;
using Reloaded.Mod.Interfaces;

namespace HeroesPowerPlant.RemoteControl
{
    public unsafe class Server : IMsgRefAction<SwapCollision, LiteNetLibState>
    {
        private int Port => _simpleHost.Manager.LocalPort;
        private readonly LiteNetLibHost<MessageDispatcher<LiteNetLibState>> _simpleHost;

        private MessageQueue _queue;
        private MemoryMappedFile _serverLocator; // Not unused. Used by client to locate this server.
        private ILogger _logger;

        /* Server Setup */
        public Server(ILogger logger)
        {
            _simpleHost     = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(true, new MessageDispatcher<LiteNetLibState>(), "");
            _logger         = logger;
            RegisterFunctions();

            _simpleHost.Manager.UnsyncedEvents = false;
            _simpleHost.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            _queue          = new MessageQueue(this);
            _serverLocator  = ServerLocator.RegisterPortForServer(Port);

            #if DEBUG
            _simpleHost.Manager.DisconnectTimeout = Int32.MaxValue;
            #endif
        }

        public void PollEvents()         => _simpleHost.Manager.PollEvents();
        private void RegisterFunctions() => default(SwapCollision).AddToDispatcher(this, ref _simpleHost.Dispatcher);

        /* Event Handlers */
        public void OnMessageReceive(ref SwapCollision message, ref LiteNetLibState state)
        {
            var loadCollisionMessage = message;
            var peer                 = state.Peer;

            WriteLine($"[LoadCollision] {message.CollisionFileName}");
            
            _queue.Queue.Enqueue(() =>
            {
                // Unload existing collision and load new.
                World.LoadCollision(loadCollisionMessage.CollisionFileName);
                SendAck(peer);
            });
        }

        /* Helpers */
        private void SendAck(NetPeer peer)
        {
            var message = new Acknowledgement();
            using (var data = message.Serialize(ref message))
                peer.Send(data.Span, DeliveryMethod.ReliableOrdered);
        }

        private void WriteLine(string text) => _logger.WriteLine($"[RemoteControl] {text}", _logger.ColorGreenLight);
    }
}
