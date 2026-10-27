using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HeroesPowerPlant.RemoteControl.Shared.Messages;
using LiteNetLib;
using Reloaded.Messaging;
using Reloaded.Messaging.Host.LiteNetLib;
using Reloaded.Messaging.Interfaces;
using Reloaded.Messaging.Interfaces.Utilities;
using Reloaded.Messaging.Messages;
using Reloaded.Messaging.Serializer.MessagePack;
using Reloaded.Messaging.Utilities;

namespace HeroesPowerPlant.RemoteControl.Shared
{
    public class Client
    {
        public NetPeer Server => SimpleHost.FirstPeer;
        public LiteNetLibHost<MessageDispatcher<LiteNetLibState>> SimpleHost;
        private int _timeout = 1000;

        /// <summary>
        /// Creates a client capable to communicating with a running remote control host inside a given process.
        /// </summary>
        /// <exception cref="FileNotFoundException">Server not running inside foreign process.</exception>
        public Client(Process process)
        {
            SimpleHost = new LiteNetLibHost<MessageDispatcher<LiteNetLibState>>(false, new MessageDispatcher<LiteNetLibState>(), "");
            SimpleHost.Manager.UnsyncedEvents = true;
            SimpleHost.Manager.Start(IPAddress.Loopback, IPAddress.IPv6Loopback, 0);
            SimpleHost.Manager.Connect(new IPEndPoint(IPAddress.Loopback, ServerLocator.GetPortOfServer(process.Id)), "");

            #if DEBUG
            SimpleHost.Manager.DisconnectTimeout = Int32.MaxValue;
            _timeout = Int32.MaxValue;
            #endif
        }

        /// <summary>
        /// Returns true if client is connected, else false.
        /// </summary>
        public bool IsConnected() => SimpleHost.Manager.ConnectedPeersCount > 0;

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
            where TStruct : struct, IMessage<TStruct, MessagePackSerializer<TStruct>, NullCompressor>
            where TResponse : struct, IMessage<TResponse, MessagePackSerializer<TResponse>, NullCompressor>
        {
            /* Start timeout. */
            var watch = new Stopwatch();
            watch.Start();

            /* Setup response. */
            var handler = new ResponseHandler<TResponse>();
            default(TResponse).AddToDispatcher(handler, ref SimpleHost.Dispatcher);

            /* Send message. */
            using (var data = message.Serialize(ref message))
                Server.Send(data.Span, DeliveryMethod.ReliableOrdered);

            /* Wait loop. */
            while (watch.ElapsedMilliseconds < timeout)
            {
                if (token.IsCancellationRequested)
                    throw new Exception("Task was cancelled.");

                // Return response if available.
                var response = handler.Response;
                if (response != null)
                    return response.Value;

                await Task.Delay(1, token);
            }

            throw new Exception("Timeout to receive response has expired.");
        }

        private class ResponseHandler<TResponse> : IMsgRefAction<TResponse, LiteNetLibState> where TResponse : struct
        {
            public volatile StrongBox<TResponse> Box;
            public TResponse? Response => Box?.Value;

            public void OnMessageReceive(ref TResponse received, ref LiteNetLibState state) => Box = new StrongBox<TResponse>(received);
        }
    }
}
