using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

        /* Function Addresses */
        private const int DrawHudAddress = 0x0041DFD0;
        private readonly object _lockObject = new object();
        private IHook<DrawHUD> _drawHudHook;
        private Server _server;

        /// <summary>
        /// Hooks the game Draw HUD function
        /// </summary>
        public MessageQueue(Server server)
        {
            _server = server;
            _drawHudHook = Program.Hooks.CreateHook<DrawHUD>(DrawHudHookImpl, DrawHudAddress);
            _drawHudHook.Activate();
        }

        private int DrawHudHookImpl()
        {
            _server.PollEvents();
            lock (_lockObject)
            {
                while (Queue.TryDequeue(out Action item))
                {
                    item();
                }

                return _drawHudHook.OriginalFunction();
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [Function(CallingConventions.Cdecl)]
        private delegate int DrawHUD();
    }
}
