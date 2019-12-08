using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HeroesPowerPlant.RemoteControl.Shared.Messages;
using LiteNetLib;
using Reloaded.Messaging;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Structs;

namespace HeroesPowerPlant.RemoteControl.Shared
{
    public class Client
    {
        private NetPeer Server => _simpleHost.NetManager.FirstPeer;
        private SimpleHost<MessageType> _simpleHost;
        private int _timeout = 1000;

        /// <summary>
        /// Creates a client capable to communicating with a running remote control host inside a given process.
        /// </summary>
        /// <exception cref="FileNotFoundException">Server not running inside foreign process.</exception>
        public Client(Process process)
        {
            _simpleHost = new SimpleHost<MessageType>(false);
            _simpleHost.NetManager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            _simpleHost.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, ServerLocator.GetPortOfServer(process.Id)), "");

            #if DEBUG
            _simpleHost.NetManager.DisconnectTimeout = Int32.MaxValue;
            _timeout = Int32.MaxValue;
            #endif
        }

        /// <summary>
        /// Loads a collision file given the name of the file in the collisions folder minus the name of the extension.
        /// e.g. "s03"
        /// </summary>
        public Task<Acknowledgement> LoadCollision(string collisionFileName, int timeout = -1, CancellationToken token = default)
        {
            if (timeout == -1)
                timeout = _timeout;

            return SendMessageWithResponseAsync<SwapCollision, Acknowledgement>(new SwapCollision(collisionFileName), timeout, token);
        }

        /* Send synchronously with timeout */
        private async Task<TResponse> SendMessageWithResponseAsync<TStruct, TResponse>(TStruct message, int timeout = 1000, CancellationToken token = default)
            where TStruct : IMessage<MessageType>, new()
            where TResponse : struct, IMessage<MessageType>
        {
            /* Start timeout. */
            var watch = new Stopwatch();
            watch.Start();

            /* Setup response. */
            TResponse? response = null;
            void ReceiveMessage(ref NetMessage<TResponse> netMessage)
            {
                response = netMessage.Message;
            }

            _simpleHost.MessageHandler.AddOrOverrideHandler<TResponse>(ReceiveMessage);

            /* Send message. */
            var data = new Message<MessageType, TStruct>(message).Serialize();
            Server.Send(data, DeliveryMethod.ReliableOrdered);

            /* Wait loop. */
            while (watch.ElapsedMilliseconds < timeout)
            {
                if (token.IsCancellationRequested)
                    throw new Exception("Task was cancelled.");

                // Return response if available.
                if (response != null)
                    return response.Value;

                await Task.Delay(1, token);
            }

            throw new Exception("Timeout to receive response has expired.");
        }
    }
}
