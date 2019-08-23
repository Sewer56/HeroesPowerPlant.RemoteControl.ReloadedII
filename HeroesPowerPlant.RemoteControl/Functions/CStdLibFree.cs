using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions.X86;

namespace HeroesPowerPlant.RemoteControl.Functions
{
    /// <summary>
    /// C Standard Library free function.
    /// </summary>
    public unsafe class CStdLibFree
    {
        /// <summary>
        /// Delegate to call the internal <see cref="Free"/> function.
        /// </summary>
        public static Free FreeFunction;

        /// <summary>
        /// Pointer to the <see cref="Free"/> function.
        /// </summary>
        private const int FreePtr = 0x0067B35D;

        static CStdLibFree()
        {
            FreeFunction = Program.Hooks.CreateWrapper<Free>(FreePtr, out var wrapperAddress);
        }

        [Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Free(void* memoryAddress);
    }
}
