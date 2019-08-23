using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Net;
using HeroesPowerPlant.RemoteControl.Functions;
using HeroesPowerPlant.RemoteControl.Shared;
using HeroesPowerPlant.RemoteControl.Shared.Messages;
using LiteNetLib;
using Reloaded.Messaging;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Structs;
using Reloaded.Mod.Interfaces;

namespace HeroesPowerPlant.RemoteControl
{
    public class Server
    {
        private int Port => _simpleHost.NetManager.LocalPort;
        private readonly SimpleHost<MessageType> _simpleHost;

        private MessageQueue _queue;
        private MemoryMappedFile _serverLocator;
        private ILogger _logger;

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

        public void PollEvents()
        {
            _simpleHost.NetManager.PollEvents();
        }

        private void RegisterFunctions()
        {
            _simpleHost.MessageHandler.AddOrOverrideHandler<SwapCollision>(HandleSwapCollision);
        }


        /* Event Handlers */

        private unsafe void HandleSwapCollision(ref NetMessage<SwapCollision> netMessage)
        {
            var loadCollisionMessage = netMessage.Message;
            var peer                 = netMessage.Peer;

            var collisionPtr        = (int*) 0x00A77684;
            var collisionWaterPtr   = (int*) 0x00A77688;
            var collisionDeathPtr   = (int*) 0x00A7768C;

            WriteLine($"[LoadCollision] {netMessage.Message.CollisionFileName}");
            _queue.Queue.Enqueue(() =>
            {
                // Unload existing collision.
                if (*collisionPtr != 0)
                {
                    CollisionUnloader.DestroyCollisionFunction(*collisionPtr);
                    CStdLibFree.FreeFunction((void*) *collisionPtr);
                }

                if (*collisionWaterPtr != 0)
                {
                    CollisionUnloader.DestroyCollisionFunction(*collisionWaterPtr);
                    CStdLibFree.FreeFunction((void*) *collisionWaterPtr);
                }

                if (*collisionDeathPtr != 0)
                {
                    CollisionUnloader.DestroyCollisionFunction(*collisionDeathPtr);
                    CStdLibFree.FreeFunction((void*) *collisionDeathPtr);
                }

                // Load new collision.
                CollisionReloader.InitCollisionFunction(loadCollisionMessage.CollisionFileName);
                SendAck(peer);
            });
        }

        /* Helpers */
        private void SendAck(NetPeer peer)
        {
            var message = new Message<MessageType, Acknowledgement>(new Acknowledgement());
            peer.Send(message.Serialize(), DeliveryMethod.ReliableOrdered);
        }

        private void WriteLine(string text)
        {
            _logger.WriteLine($"[RemoteControl] {text}", _logger.ColorGreenLight);
        }
    }
}
