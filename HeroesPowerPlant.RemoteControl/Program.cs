using System;
using System.Diagnostics;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace HeroesPowerPlant.RemoteControl
{
    public class Program : IMod
    {
        public static IModLoader        ModLoader;
        public static IReloadedHooks    Hooks { get; private set; }

        private static Server           _server;

        public void Start(IModLoaderV1 loader)
        {
            ModLoader = (IModLoader)loader;
            ModLoader.GetController<IReloadedHooks>().TryGetTarget(out var hooks);
            Hooks = hooks;

            /* Your mod code starts here. */
            var logger = (ILogger) ModLoader.GetLogger();
            _server = new Server(logger);
        }

        /* Mod loader actions. */

        /* No Unloading: Sewer is "lazy", being worn out porting all mods at once. */
        public void Suspend() { }
        public void Resume()  { }
        public void Unload()  { }

        public bool CanUnload()  => false;
        public bool CanSuspend() => false;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }
    }
}
