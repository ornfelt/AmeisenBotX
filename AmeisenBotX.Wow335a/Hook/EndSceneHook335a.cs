using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Hook;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AmeisenBotX.Wow335a.Hook
{
    /// <summary>
    /// Gets the WowMemoryApi instance.
    /// </summary>
    public class EndSceneHook335a : GenericEndSceneHook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndSceneHook335a"/> class.
        /// </summary>
        /// <param name="memory">The WowMemoryApi object used for memory manipulation.</param>
        public EndSceneHook335a(WowMemoryApi memory)
                    : base(memory)
        {
            Memory = memory;
            OriginalFunctionBytes = new();
        }

        /// <summary>
        /// Gets the WowMemoryApi instance.
        /// </summary>
        private WowMemoryApi Memory { get; }

        /// <summary>
        /// Used to save the old render flags of wow.
        /// </summary>
        private int OldRenderFlags { get; set; }

        /// <summary>
        /// Used to save the original instruction when a function get disabled.
        /// </summary>
        private Dictionary<IntPtr, byte> OriginalFunctionBytes { get; }

        /// <summary>
        /// Calls a function on an object.
        /// </summary>
        /// <param name="objectBaseAddress">The base address of the object.</param>
        /// <param name="functionAddress">The address of the function to be called.</param>
        /// <param name="args">A list of arguments to be passed to the function. (optional)</param>
        /// <returns>True if the function was called successfully, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CallObjectFunction(IntPtr objectBaseAddress, IntPtr functionAddress, List<object> args = null)
        {
            return CallObjectFunction(objectBaseAddress, functionAddress, args, false, out _);
        }

        /// <summary>
        /// Use this to call a thiscall function of a wowobject
        /// </summary>
        /// <param name="objectBaseAddress">Object base</param>
        /// <param name="functionAddress">Function to call</param>
        /// <param name="args">Arguments, can be null</param>
        /// <param name="readReturnBytes">Whether to read the retunr address or not</param>
        /// <param name="returnAddress">Return address</param>
        /// <returns>True if everything went right, false if not</returns>
        public bool CallObjectFunction(IntPtr objectBaseAddress, IntPtr functionAddress, List<object> args, bool readReturnBytes, out IntPtr returnAddress)
        {
#if DEBUG
            if (objectBaseAddress == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(objectBaseAddress), "objectBaseAddress is an invalid pointer"); }
            if (functionAddress == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(functionAddress), "functionAddress is an invalid pointer"); }
#endif
            List<string> asm = new() { $"MOV ECX, {objectBaseAddress}" };

            if (args != null)
            {
                for (int i = 0; i < args.Count; ++i)
                {
                    asm.Add($"PUSH {args[i]}");
                }
            }

            asm.Add($"CALL {functionAddress}");
            asm.Add("RET");

            if (readReturnBytes)
            {
                bool status = InjectAndExecute(asm, readReturnBytes, out returnAddress);
                return status;
            }

            returnAddress = IntPtr.Zero;
            return InjectAndExecute(asm, readReturnBytes, out _);
        }

        /// <summary>
        /// Abandons quests that are not in the provided list of quest names.
        /// </summary>
        /// <param name="questNames">A collection of quest names.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LuaAbandonQuestsNotIn(IEnumerable<string> questNames)
        {
            if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetNumQuestLogEntries()"), out string r1)
                && int.TryParse(r1, out int numQuestLogEntries))
            {
                for (int i = 1; i <= numQuestLogEntries; i++)
                {
                    if (ExecuteLuaAndRead(BotUtils.ObfuscateLua($"{{v:0}}=GetQuestLogTitle({i})"), out string questLogTitle) && !questNames.Contains(questLogTitle))
                    {
                        LuaDoString($"SelectQuestLogEntry({i});SetAbandonQuest();AbandonQuest()");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Executes a Lua string command.
        /// </summary>
        /// <param name="command">The Lua command to execute.</param>
        /// <returns>True if the command was successfully executed, otherwise false.</returns>
        public bool LuaDoString(string command)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentOutOfRangeException(nameof(command), "command is empty"); }
#endif
            AmeisenLogger.I.Log("335aHook", $"LuaDoString: {command}", LogLevel.Verbose);

            byte[] bytes = Encoding.UTF8.GetBytes(command + "\0");

            if (Memory.AllocateMemory((uint)bytes.Length, out IntPtr memAlloc))
            {
                try
                {
                    if (Memory.WriteBytes(memAlloc, bytes))
                    {
                        return InjectAndExecute(new string[]
                        {
                            "PUSH 0",
                            $"PUSH {memAlloc}",
                            $"PUSH {memAlloc}",
                            $"CALL {Memory.Offsets.FunctionLuaDoString}",
                            "ADD ESP, 0xC",
                            "RET",
                        });
                    }
                }
                finally
                {
                    Memory.FreeMemory(memAlloc);
                }
            }

            return false;
        }

        /// <summary>
        /// Performs a right click action on the specified object.
        /// </summary>
        /// <param name="objectBase">The base address of the object.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ObjectRightClick(IntPtr objectBase)
        {
            CallObjectFunction(objectBase, Memory.Offsets.FunctionGameobjectOnRightClick);
        }

        /// <summary>
        /// Sets the facing angle of the unit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFacing(IntPtr unitBase, float angle, bool smooth = false)
        {
            // smooth not supported for now
            CallObjectFunction(unitBase, Memory.Offsets.FunctionUnitSetFacing, new()
            {
                angle.ToString(CultureInfo.InvariantCulture).Replace(',', '.'),
                Environment.TickCount
            });
        }

        /// <summary>
        /// Sets the rendering state based on the specified flag. If rendering is enabled, the WorldFrame and UIParent are shown and certain functions are enabled.
        /// If rendering is disabled, the WorldFrame and UIParent are hidden, and certain functions are disabled.
        /// </summary>
        /// <param name="renderingEnabled">Flag indicating whether rendering should be enabled or disabled.</param>
        public void SetRenderState(bool renderingEnabled)
        {
            if (renderingEnabled)
            {
                LuaDoString("WorldFrame:Show();UIParent:Show()");
            }

            Memory.SuspendMainThread();

            if (renderingEnabled)
            {
                EnableFunction(Memory.Offsets.FunctionWorldRender);
                EnableFunction(Memory.Offsets.FunctionWorldRenderWorld);
                EnableFunction(Memory.Offsets.FunctionWorldFrame);

                Memory.Write(Memory.Offsets.RenderFlags, OldRenderFlags);
            }
            else
            {
                if (Memory.Read(Memory.Offsets.RenderFlags, out int renderFlags))
                {
                    OldRenderFlags = renderFlags;
                }

                DisableFunction(Memory.Offsets.FunctionWorldRender);
                DisableFunction(Memory.Offsets.FunctionWorldRenderWorld);
                DisableFunction(Memory.Offsets.FunctionWorldFrame);

                Memory.Write(Memory.Offsets.RenderFlags, 0);
            }

            Memory.ResumeMainThread();

            if (!renderingEnabled)
            {
                LuaDoString("WorldFrame:Hide();UIParent:Hide()");
            }
        }

        /// <summary>
        /// Sets the target GUID.
        /// </summary>
        /// <param name="guid">The GUID to set as the target.</param>
        public void TargetGuid(ulong guid)
        {
            byte[] guidBytes = BitConverter.GetBytes(guid);

            InjectAndExecute(new string[]
            {
                $"PUSH {BitConverter.ToUInt32(guidBytes, 4)}",
                $"PUSH {BitConverter.ToUInt32(guidBytes, 0)}",
                $"CALL {Memory.Offsets.FunctionSetTarget}",
                "ADD ESP, 0x8",
                "RET"
            });
        }

        /// <summary>
        /// Traces a line from the start position to the end position with specified flags.
        /// </summary>
        /// <param name="start">The starting position of the line.</param>
        /// <param name="end">The ending position of the line.</param>
        /// <param name="flags">Flags used for the trace line.</param>
        /// <returns>True if the trace line was successful, false otherwise.</returns>
        public bool TraceLine(Vector3 start, Vector3 end, uint flags)
        {
            if (Memory.AllocateMemory(40, out IntPtr tracelineCodecave))
            {
                try
                {
                    (float, Vector3, Vector3) tracelineCombo = (1.0f, start, end);

                    IntPtr distancePointer = tracelineCodecave;
                    IntPtr startPointer = IntPtr.Add(distancePointer, 0x4);
                    IntPtr endPointer = IntPtr.Add(startPointer, 0xC);
                    IntPtr resultPointer = IntPtr.Add(endPointer, 0xC);

                    if (Memory.Write(distancePointer, tracelineCombo))
                    {
                        string[] asm = new string[]
                        {
                            "PUSH 0",
                            $"PUSH {flags}",
                            $"PUSH {distancePointer}",
                            $"PUSH {resultPointer}",
                            $"PUSH {endPointer}",
                            $"PUSH {startPointer}",
                            $"CALL {Memory.Offsets.FunctionTraceline}",
                            "ADD ESP, 0x18",
                            "RET",
                        };

                        if (InjectAndExecute(asm, true, out IntPtr returnAddress))
                        {
                            return returnAddress != IntPtr.Zero && (returnAddress.ToInt32() & 0xFF) == 0;
                        }
                    }
                }
                finally
                {
                    Memory.FreeMemory(tracelineCodecave);
                }
            }

            return false;
        }

        /// <summary>
        /// Interacts with the unit by calling the appropriate object function
        /// to perform a right click action.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InteractWithUnit(IntPtr unitBase)
        {
            CallObjectFunction(unitBase, Memory.Offsets.FunctionUnitOnRightClick);
        }

        /// <summary>
        /// This method clicks on the terrain at the specified position by allocating memory, 
        /// writing the position to the allocated memory, and then injecting and executing 
        /// the necessary instructions.
        /// </summary>
        /// <param name="position">The position on the terrain where the click should occur.</param>
        public void ClickOnTerrain(Vector3 position)
        {
            if (Memory.AllocateMemory(20, out IntPtr codeCaveVector3))
            {
                try
                {
                    if (Memory.Write(IntPtr.Add(codeCaveVector3, 8), position))
                    {
                        InjectAndExecute(new string[]
                        {
                            $"PUSH {codeCaveVector3.ToInt32()}",
                            $"CALL {Memory.Offsets.FunctionHandleTerrainClick}",
                            "ADD ESP, 0x4",
                            "RET",
                        });
                    }
                }
                finally
                {
                    Memory.FreeMemory(codeCaveVector3);
                }
            }
        }

        /// <summary>
        /// Moves the player to the specified position by injecting code into the game's memory.
        /// </summary>
        /// <param name="playerBase">The base address of the player object.</param>
        /// <param name="position">The target position to move the player to.</param>
        public void ClickToMove(IntPtr playerBase, Vector3 position)
        {
            if (Memory.AllocateMemory(12, out IntPtr codeCaveVector3))
            {
                try
                {
                    if (Memory.Write(codeCaveVector3, position))
                    {
                        CallObjectFunction(playerBase, Memory.Offsets.FunctionPlayerClickToMove, new() { codeCaveVector3 });
                    }
                }
                finally
                {
                    Memory.FreeMemory(codeCaveVector3);
                }
            }
        }

        /// <summary>
        /// Executes a Lua command and reads the result.
        /// </summary>
        /// <param name="cmdVarTuple">A tuple containing the Lua command and a variable name.</param>
        /// <param name="result">The result of executing the Lua command.</param>
        /// <returns>True if the Lua command execution is successful, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteLuaAndRead((string, string) cmdVarTuple, out string result)
        {
            return ExecuteLuaAndRead(cmdVarTuple.Item1, cmdVarTuple.Item2, out result);
        }

        /// <summary>
        /// Executes a Lua command and reads the result.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="variable">The variable to use for the command.</param>
        /// <param name="result">The output result of the command execution.</param>
        /// <returns>True if the command was successfully executed and the result was read, false otherwise.</returns>
        public bool ExecuteLuaAndRead(string command, string variable, out string result)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentOutOfRangeException(nameof(command), "command is empty"); }
            if (string.IsNullOrWhiteSpace(variable)) { throw new ArgumentOutOfRangeException(nameof(variable), "variable is empty"); }
#endif
            AmeisenLogger.I.Log("335aHook", $"WowExecuteLuaAndRead: {command}", LogLevel.Verbose);

            byte[] commandBytes = Encoding.UTF8.GetBytes(command + "\0");
            byte[] variableBytes = Encoding.UTF8.GetBytes(variable + "\0");

            if (Memory.AllocateMemory((uint)commandBytes.Length + (uint)variableBytes.Length, out IntPtr memAllocCmdVar))
            {
                try
                {
                    byte[] bytesToWrite = new byte[commandBytes.Length + variableBytes.Length];

                    Array.Copy(commandBytes, bytesToWrite, commandBytes.Length);
                    Array.Copy(variableBytes, 0, bytesToWrite, commandBytes.Length, variableBytes.Length);

                    Memory.WriteBytes(memAllocCmdVar, bytesToWrite);

                    string[] asm = new string[]
                    {
                        "PUSH 0",
                        $"PUSH {memAllocCmdVar}",
                        $"PUSH {memAllocCmdVar}",
                        $"CALL {Memory.Offsets.FunctionLuaDoString}",
                        "ADD ESP, 0xC",
                        $"CALL {Memory.Offsets.FunctionGetActivePlayerObject}",
                        "MOV ECX, EAX",
                        "PUSH -1",
                        $"PUSH {memAllocCmdVar + commandBytes.Length}",
                        $"CALL {Memory.Offsets.FunctionGetLocalizedText}",
                        "RET",
                    };

                    if (InjectAndExecute(asm, true, out IntPtr returnAddress)
                        && Memory.ReadString(returnAddress, Encoding.UTF8, out result))
                    {
                        return !string.IsNullOrWhiteSpace(result);
                    }
                }
                finally
                {
                    Memory.FreeMemory(memAllocCmdVar);
                }
            }

            result = string.Empty;
            return false;
        }

        /// <summary>
        /// Faces the specified position by setting the facing angle of the player.
        /// </summary>
        /// <param name="playerBase">The base address of the player.</param>
        /// <param name="playerPosition">The current position of the player.</param>
        /// <param name="positionToFace">The position to face.</param>
        /// <param name="smooth">Specifies whether the facing angle should be set smoothly or not.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FacePosition(IntPtr playerBase, Vector3 playerPosition, Vector3 positionToFace, bool smooth = false)
        {
            SetFacing(playerBase, BotMath.GetFacingAngle(playerPosition, positionToFace), smooth);
        }

        /// <summary>
        /// Retrieves the localized text for a given variable.
        /// </summary>
        /// <param name="variable">The variable for which to retrieve the localized text.</param>
        /// <param name="result">The localized text for the variable.</param>
        /// <returns>True if the localized text was successfully retrieved, false otherwise.</returns>
        public bool GetLocalizedText(string variable, out string result)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(variable)) { throw new ArgumentOutOfRangeException(nameof(variable), "variable is empty"); }
#endif
            if (!string.IsNullOrWhiteSpace(variable))
            {
                byte[] variableBytes = Encoding.UTF8.GetBytes(variable + "\0");

                if (Memory.AllocateMemory((uint)variableBytes.Length, out IntPtr memAlloc))
                {
                    try
                    {
                        Memory.WriteBytes(memAlloc, variableBytes);

                        string[] asm = new string[]
                        {
                            $"CALL {Memory.Offsets.FunctionGetActivePlayerObject}",
                            "MOV ECX, EAX",
                            "PUSH -1",
                            $"PUSH {memAlloc}",
                            $"CALL {Memory.Offsets.FunctionGetLocalizedText}",
                            "RET",
                        };

                        if (InjectAndExecute(asm, true, out IntPtr returnAddress)
                            && Memory.ReadString(returnAddress, Encoding.UTF8, out result))
                        {
                            return !string.IsNullOrWhiteSpace(result);
                        }
                    }
                    finally
                    {
                        Memory.FreeMemory(memAlloc);
                    }
                }
            }

            result = string.Empty;
            return false;
        }

        ///<summary>
        ///Gets the reaction of a unit with another unit.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUnitReaction(IntPtr a, IntPtr b)
        {
#if DEBUG
            if (a == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(a), "a is no valid pointer"); }
            if (b == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(b), "b is no valid pointer"); }
#endif
            return CallObjectFunction(a, Memory.Offsets.FunctionUnitGetReaction, new() { b }, true, out IntPtr ret)
                && ret != IntPtr.Zero ? ret.ToInt32() : 2;
        }

        /// <summary>
        /// Disables a function by replacing it with a ret opcode if it hasn't already been replaced.
        /// </summary>
        /// <param name="address">The address of the function.</param>
        private void DisableFunction(IntPtr address)
        {
            // check whether we already replaced the function or not
            if (Memory.Read(address, out byte opcode)
                && opcode != 0xC3)
            {
                SaveOriginalFunctionBytes(address);
                Memory.PatchMemory(address, (byte)0xC3);
            }
        }

        /// <summary>
        /// Enables the function at the specified address by checking for the presence of a RET opcode and restoring the original function bytes.
        /// </summary>
        /// <param name="address">The address of the function to enable.</param>
        private void EnableFunction(IntPtr address)
        {
            // check for RET opcode to be present before restoring original function
            if (OriginalFunctionBytes.ContainsKey(address)
                && Memory.Read(address, out byte opcode)
                && opcode == 0xC3)
            {
                Memory.PatchMemory(address, OriginalFunctionBytes[address]);
            }
        }

        /// <summary>
        /// Saves the original function bytes at the specified memory address.
        /// </summary>
        /// <param name="address">The memory address to save the original function bytes.</param>
        private void SaveOriginalFunctionBytes(IntPtr address)
        {
            if (Memory.Read(address, out byte opcode))
            {
                if (!OriginalFunctionBytes.ContainsKey(address))
                {
                    OriginalFunctionBytes.Add(address, opcode);
                }
                else
                {
                    OriginalFunctionBytes[address] = opcode;
                }
            }
        }
    }
}