using System;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBotX.Wow.Hook.Modules
{
    public class TracelineJumpHookModule : RunAsmHookModule
    {
        /// <summary>
        /// Initializes a new instance of the TracelineJumpHookModule class.
        /// </summary>
        /// <param name="onUpdate">The action to be called when the module is updated.</param>
        /// <param name="tick">The action to be called when the module ticks.</param>
        /// <param name="memory">The WowMemoryApi used for hooking.</param>
        public TracelineJumpHookModule(Action<IntPtr> onUpdate, Action<IHookModule> tick, WowMemoryApi memory) : base(onUpdate, tick, memory, 256)
        {
        }

        /// <summary>
        /// Destructor for the TracelineJumpHookModule class.
        /// Frees the memory allocated by the ExecuteAddress member if it is not zero.
        /// </summary>
        ~TracelineJumpHookModule()
        {
            if (ExecuteAddress != IntPtr.Zero) { Memory.FreeMemory(ExecuteAddress); }
        }

        /// <summary>
        /// Gets or sets the address of the command.
        /// </summary>
        public IntPtr CommandAddress { get; private set; }

        /// <summary>Gets or sets the memory address of the data.</summary>
        public IntPtr DataAddress { get; private set; }


        /// <summary>
        /// Gets or sets the memory address to execute the code.
        /// </summary>
        public IntPtr ExecuteAddress { get; private set; }

        /// <summary>
        /// Returns the memory address of the data.
        /// </summary>
        public override IntPtr GetDataPointer()
        {
            return DataAddress;
        }

        /// <summary>
        /// Overrides the base class method and prepares the assembly code for execution, returning a boolean value indicating if the preparation was successful. It also provides the prepared assembly code as an output parameter.
        /// </summary>
        /// <param name="assembly">The prepared assembly code as a collection of strings.</param>
        /// <returns>True if the assembly preparation is successful, false otherwise.</returns>
        protected override bool PrepareAsm(out IEnumerable<string> assembly)
        {
            byte[] luaJumpBytes = Encoding.ASCII.GetBytes("JumpOrAscendStart();AscendStop()");

            uint memoryNeeded = (uint)(4 + 40 + luaJumpBytes.Length + 1);

            if (Memory.AllocateMemory(memoryNeeded, out IntPtr memory))
            {
                ExecuteAddress = memory;
                CommandAddress = ExecuteAddress + 4;
                DataAddress = CommandAddress + 40;

                Memory.WriteBytes(CommandAddress, luaJumpBytes);

                IntPtr distancePointer = DataAddress;
                IntPtr startPointer = IntPtr.Add(distancePointer, 0x4);
                IntPtr endPointer = IntPtr.Add(startPointer, 0xC);
                IntPtr resultPointer = IntPtr.Add(endPointer, 0xC);

                assembly = new List<string>()
                {
                    "X:",
                    $"TEST DWORD [{ExecuteAddress}], 1",
                    "JE @out",
                    "PUSH 0",
                    "PUSH 0x120171",
                    $"PUSH {distancePointer}",
                    $"PUSH {resultPointer}",
                    $"PUSH {endPointer}",
                    $"PUSH {startPointer}",
                    $"CALL {Memory.Offsets.FunctionTraceline}",
                    "ADD ESP, 0x18",
                    "TEST AL, 1",
                    "JE @out",
                    "PUSH 0",
                    $"PUSH {CommandAddress}",
                    $"PUSH {CommandAddress}",
                    $"CALL {Memory.Offsets.FunctionLuaDoString}",
                    "ADD ESP, 0xC",
                    $"MOV DWORD [{ExecuteAddress}], 0",
                    "@out:",
                    "RET"
                };

                return true;
            }

            assembly = Array.Empty<string>();
            return false;
        }
    }
}