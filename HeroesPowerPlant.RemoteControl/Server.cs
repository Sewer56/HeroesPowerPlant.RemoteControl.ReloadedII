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
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Structs;
using Reloaded.Mod.Interfaces;

namespace HeroesPowerPlant.RemoteControl
{
    public unsafe class Server
    {
        private int Port => _simpleHost.NetManager.LocalPort;
        private readonly SimpleHost<MessageType> _simpleHost;

        private MessageQueue _queue;
        private MemoryMappedFile _serverLocator; // Not unused. Used by client to locate this server.
        private ILogger _logger;

        /* Server Setup */
        public Server(ILogger logger)
        {
            _simpleHost     = new SimpleHost<MessageType>(true);
            _logger         = logger;
            RegisterFunctions();

            _simpleHost.NetManager.UnsyncedEvents = false;
            _simpleHost.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            _queue          = new MessageQueue(this);
            _serverLocator  = ServerLocator.RegisterPortForServer(Port);

            #if DEBUG
            _simpleHost.NetManager.DisconnectTimeout = Int32.MaxValue;
            #endif
        }

        public void PollEvents()         => _simpleHost.NetManager.PollEvents();
        private void RegisterFunctions() => _simpleHost.MessageHandler.AddOrOverrideHandler<SwapCollision>(HandleSwapCollision);

        /* Event Handlers */
        private void HandleSwapCollision(ref NetMessage<SwapCollision> netMessage)
        {
            var loadCollisionMessage = netMessage.Message;
            var peer                 = netMessage.Peer;

            WriteLine($"[LoadCollision] {netMessage.Message.CollisionFileName}");
            
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
            var message = new Message<MessageType, Acknowledgement>(new Acknowledgement());
            peer.Send(message.Serialize(), DeliveryMethod.ReliableOrdered);
        }

        private void WriteLine(string text) => _logger.WriteLine($"[RemoteControl] {text}", _logger.ColorGreenLight);
    }
}
