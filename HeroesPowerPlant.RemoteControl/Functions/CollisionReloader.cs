using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions.X86;
using static Reloaded.Hooks.Definitions.X86.FunctionAttribute;

namespace HeroesPowerPlant.RemoteControl.Functions
{
    public unsafe class CollisionReloader
    {
        /// <summary>
        /// Delegate to call the internal <see cref="InitCollision"/> function.
        /// </summary>
        public static InitCollision InitCollisionFunction;

        /// <summary>
        /// Pointer to the <see cref="InitCollision"/> function.
        /// </summary>
        private const int InitCollisionPtr = 0x00425500;

        /// <summary>
        /// Pointer to the "LandManager" responsible for handling the currently loaded in stage.
        /// </summary>
        private const int LandManagerPtr = 0x00A792D0;

        static CollisionReloader()
        {
            InitCollisionFunction = Program.Hooks.CreateWrapper<InitCollision>(InitCollisionPtr, out var wrapperAddress);
        }

        /// <summary>
        /// Loads a given collision file.
        /// </summary>
        /// <param name="landManagerPtr">Pointer to the land manager.</param>
        /// <param name="stringPtr">Pointer to name of the file.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [Function(new[] { Register.esi, Register.edi }, Register.eax, StackCleanup.None)]
        public delegate int InitCollision([MarshalAs(UnmanagedType.LPStr)] string stringPtr, int landManagerPtr = LandManagerPtr);

        // Shame void* cannot be compile time constant.
    }
}
