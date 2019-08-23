using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions.X86;
using static Reloaded.Hooks.Definitions.X86.FunctionAttribute;

namespace HeroesPowerPlant.RemoteControl.Functions
{
    public unsafe class CollisionUnloader
    {
        /// <summary>
        /// Delegate to call the internal <see cref="DestroyCollision"/> function.
        /// </summary>
        public static DestroyCollision DestroyCollisionFunction;

        /// <summary>
        /// Pointer to the <see cref="DestroyCollision"/> function.
        /// </summary>
        private const int DestroyCollisionPtr = 0x0042D1E0;

        static CollisionUnloader()
        {
            DestroyCollisionFunction = Program.Hooks.CreateWrapper<DestroyCollision>(DestroyCollisionPtr, out var wrapperAddress);
        }

        /// <summary>
        /// Unloads a given collision file.
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [Function(new[] { Register.esi }, Register.eax, StackCleanup.None)]
        public delegate int DestroyCollision(long quadTreePtr);

        // Shame void* cannot be compile time constant.
    }
}
