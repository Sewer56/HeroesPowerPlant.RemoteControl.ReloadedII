using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Heroes.SDK.API;
using Heroes.SDK.Classes.PseudoNativeClasses;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.ReloadedII.Interfaces;

namespace HeroesPowerPlant.RemoteControl
{
    /// <summary>
    /// Provides the implementation of a message queue, which executes messages
    /// before the game draws the HUD. Synchronous message execution is implemented to
    /// ensure actively used data/code is not altered.
    /// </summary>
    public class MessageQueue
    {
        /// <summary>
        /// The queue of functions to be executed as the game draws the HUD.
        /// </summary>
        public ConcurrentQueue<Action> Queue = new ConcurrentQueue<Action>();

        private readonly object _lock = new object();
        private Server _server;

        public MessageQueue(Server server)
        {
            _server = server;
            Event.AfterSleep += AfterFrame;
        }

        private void AfterFrame()
        {
            _server.PollEvents();
            lock (_lock)
            {
                while (Queue.TryDequeue(out Action item))
                {
                    item();
                }
            }
        }
    }
}
