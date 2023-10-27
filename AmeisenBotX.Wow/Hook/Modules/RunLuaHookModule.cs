using System;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBotX.Wow.Hook.Modules
{
    /// <summary>
    /// Initializes a new instance of the RunLuaHookModule class.
    /// </summary>
    /// <param name="onUpdate">The action to perform when the module is updated.</param>
    /// <param name="tick">The action to perform on each tick of the module.</param>
    /// <param name="memory">An instance of the WowMemoryApi class.</param>
    /// <param name="lua">The Lua code to be executed.</param>
    /// <param name="varName">The name of the variable to be assigned the Lua code result.</param>
    /// <param name="alloc">The allocation size for the hook.</param>
    public class RunLuaHookModule : RunAsmHookModule
    {
        /// <summary>
        /// Initializes a new instance of the RunLuaHookModule class.
        /// </summary>
        /// <param name="onUpdate">The action to perform when the module is updated.</param>
        /// <param name="tick">The action to perform on each tick of the module.</param>
        /// <param name="memory">An instance of the WowMemoryApi class.</param>
        /// <param name="lua">The Lua code to be executed.</param>
        /// <param name="varName">The name of the variable to be assigned the Lua code result.</param>
        /// <param name="allocSize">The size of the allocated memory (default is 128).</param>
        public RunLuaHookModule(Action<IntPtr> onUpdate, Action<IHookModule> tick, WowMemoryApi memory, string lua, string varName, uint allocSize = 128) : base(onUpdate, tick, memory, allocSize)
        {
            Lua = lua;
            VarName = varName;
        }

        /// <summary>
        /// This method is used to clean up the memory allocated for the Lua hook module before it is unloaded.
        /// It checks if the return address is not zero and if not, frees the allocated memory.
        /// </summary>
        ~RunLuaHookModule()
        {
            if (ReturnAddress != IntPtr.Zero) { Memory.FreeMemory(ReturnAddress); }
        }

        /// <summary>
        /// Gets or sets the memory address of the command.
        /// </summary>
        public IntPtr CommandAddress { get; private set; }

        /// <summary>
        /// Gets or sets the memory address of the object being returned.
        /// </summary>
        public IntPtr ReturnAddress { get; private set; }

        /// <summary>
        /// Gets or sets the memory address of the variable.
        /// </summary>
        public IntPtr VarAddress { get; private set; }

        /// <summary>
        /// The Lua property representing a string.
        /// </summary>
        private string Lua { get; }

        /// <summary>
        /// Gets or sets the value of the VarName property.
        /// </summary>
        private string VarName { get; }

        /// <summary>
        /// Retrieves the data pointer.
        /// </summary>
        /// <returns>The data pointer or IntPtr.Zero if it cannot be retrieved.</returns>
        public override IntPtr GetDataPointer()
        {
            if (Memory.Read(ReturnAddress, out IntPtr pString))
            {
                return pString;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Overrides the base class method to prepare the assembly code and retrieves the allocated memory.
        /// </summary>
        /// <param name="assembly">Output parameter that contains the generated assembly code.</param>
        /// <returns>Returns a boolean value indicating whether the preparation of the assembly was successful.</returns>
        protected override bool PrepareAsm(out IEnumerable<string> assembly)
        {
            byte[] luaBytes = Encoding.ASCII.GetBytes(Lua);
            byte[] luaVarBytes = Encoding.ASCII.GetBytes(VarName);

            uint memoryNeeded = (uint)(4 + luaBytes.Length + 1 + luaVarBytes.Length + 1);

            if (Memory.AllocateMemory(memoryNeeded, out IntPtr memory))
            {
                ReturnAddress = memory;
                CommandAddress = ReturnAddress + 4;
                VarAddress = CommandAddress + luaBytes.Length + 1;

                Memory.WriteBytes(CommandAddress, luaBytes);
                Memory.WriteBytes(VarAddress, luaVarBytes);

                assembly = new List<string>()
                {
                    "X:",
                    "PUSH 0",
                    $"PUSH {CommandAddress}",
                    $"PUSH {CommandAddress}",
                    $"CALL {Memory.Offsets.FunctionLuaDoString}",
                    "ADD ESP, 0xC",
                    $"CALL {Memory.Offsets.FunctionGetActivePlayerObject}",
                    "MOV ECX, EAX",
                    "PUSH -1",
                    $"PUSH {VarAddress}",
                    $"CALL {Memory.Offsets.FunctionGetLocalizedText}",
                    $"MOV DWORD [{ReturnAddress}], EAX",
                    $"RET"
                };

                return true;
            }

            assembly = Array.Empty<string>();
            return false;
        }
    }
}