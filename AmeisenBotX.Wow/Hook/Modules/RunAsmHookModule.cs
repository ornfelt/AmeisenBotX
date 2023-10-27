using System;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Hook.Modules
{
    /// <summary>
    /// Frees the memory allocated for the Assembly Hook Module.
    /// </summary>
    public abstract class RunAsmHookModule : IHookModule
    {
        ///<summary>
        /// Initializes a new instance of the RunAsmHookModule class with the specified parameters.
        ///</summary>
        public RunAsmHookModule(Action<IntPtr> onUpdate, Action<IHookModule> tick, WowMemoryApi memory, uint allocSize)
        {
            Memory = memory;
            AllocSize = allocSize;
            OnDataUpdate = onUpdate;
            Tick = tick;
        }

        /// <summary>
        /// Frees the memory allocated for the Assembly Hook Module.
        /// </summary>
        ~RunAsmHookModule()
        {
            if (AsmAddress != IntPtr.Zero) { Memory.FreeMemory(AsmAddress); }
        }

        /// <summary>
        /// Gets or sets the memory address represented as an IntPtr.
        /// </summary>
        public IntPtr AsmAddress { get; set; }

        /// <summary>
        /// Gets or sets the action to be executed when data is updated.
        /// </summary>
        public Action<IntPtr> OnDataUpdate { get; set; }

        /// <summary>
        /// Gets or sets the Tick action which is used to hook a module.
        /// The action should accept an instance of IHookModule as a parameter.
        /// </summary>
        public Action<IHookModule> Tick { get; set; }

        /// <summary>
        /// Gets the allocation size.
        /// </summary>
        protected uint AllocSize { get; }

        /// <summary>
        /// Gets or sets the WowMemoryApi used for accessing memory in the game.
        /// </summary>
        protected WowMemoryApi Memory { get; }

        /// <summary>
        /// Returns the pointer to the data.
        /// </summary>
        public abstract IntPtr GetDataPointer();

        /// <summary>
        /// Attempts to inject an assembly into the current process's memory.
        /// </summary>
        /// <returns>True if the injection was successful, false otherwise.</returns>
        public virtual bool Inject()
        {
            if (PrepareAsm(out IEnumerable<string> assembly)
                && Memory.AllocateMemory(AllocSize, out IntPtr address))
            {
                AsmAddress = address;
                return Memory.InjectAssembly(assembly, address);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is an abstract implementation that prepares the assembly and returns a boolean value indicating if the preparation was successful.
        /// It also outputs a collection of strings representing the prepared assembly.
        /// </summary>
        /// <param name="assembly">The output parameter that will hold the prepared assembly</param>
        /// <returns>True if the assembly was prepared successfully, otherwise false</returns>
        protected abstract bool PrepareAsm(out IEnumerable<string> assembly);
    }
}