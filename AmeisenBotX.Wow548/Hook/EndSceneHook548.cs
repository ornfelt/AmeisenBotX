using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Hook;
using AmeisenBotX.Wow548.Offsets;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Represents a hook that is triggered at the end of a scene in the game, specifically for version 548.
/// </summary>
namespace AmeisenBotX.Wow548.Hook
{
    /// <summary>
    /// Represents a hook that is triggered at the end of a scene in the game, specifically for version 548.
    /// </summary>
    public class EndSceneHook548 : GenericEndSceneHook
    {
        /// <summary>
        /// Initializes a new instance of the EndSceneHook548 class.
        /// </summary>
        /// <param name="memory">The WowMemoryApi object to use for memory operations.</param>
        public EndSceneHook548(WowMemoryApi memory)
                    : base(memory)
        {
            OriginalFunctionBytes = new();
        }

        /// <summary>
        /// Used to save the old render flags of wow.
        /// </summary>
        private int OldRenderFlags { get; set; }

        /// <summary>
        /// Used to save the original instruction when a function get disabled.
        /// </summary>
        private Dictionary<IntPtr, byte> OriginalFunctionBytes { get; }

        ///<summary>
        /// Calls an object function with the specified object base address and function address.
        /// Returns a boolean indicating whether the function call was successful or not.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CallObjectFunction(IntPtr objectBaseAddress, IntPtr functionAddress, List<object>? args = null)
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
        public bool CallObjectFunction(IntPtr objectBaseAddress, IntPtr functionAddress, List<object>? args, bool readReturnBytes, out IntPtr returnAddress)
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
        /// <param name="questNames">The list of quest names.</param>
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
        /// Executes a Lua command statement.
        /// </summary>
        /// <param name="command">The Lua command to execute.</param>
        /// <returns>True if the Lua command was successfully executed, false otherwise.</returns>
        public bool LuaDoString(string command)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentOutOfRangeException(nameof(command), "command is empty"); }
#endif
            AmeisenLogger.I.Log("548Hook", $"LuaDoString: {command}", LogLevel.Verbose);

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
        /// Executes the function GameobjectOnRightClick when an object is right-clicked.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ObjectRightClick(IntPtr objectBase)
        {
            CallObjectFunction(objectBase, Memory.Offsets.FunctionGameobjectOnRightClick);
        }

        /// <summary>
        /// Sets the facing angle of a unit.
        /// </summary>
        /// <param name="unitBase">The base address of the unit.</param>
        /// <param name="angle">The desired facing angle in radians.</param>
        /// <param name="smooth">Optional. Indicates whether to smoothly transition to the new facing angle. Defaults to false.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFacing(IntPtr unitBase, float angle, bool smooth = false)
        {
            CallObjectFunction(unitBase, smooth ? ((OffsetList548)Memory.Offsets).FunctionUnitSetFacingSmooth : Memory.Offsets.FunctionUnitSetFacing, new()
            {
                angle.ToString(CultureInfo.InvariantCulture).Replace(',', '.'),
                Environment.TickCount
            });
        }

        /// <summary>
        /// Sets the rendering state based on the specified boolean value.
        /// If rendering is enabled, the WorldFrame and UIParent are shown.
        /// The main thread is suspended while modifying the rendering state.
        /// If rendering is enabled, specific functions are enabled and the render flags are restored to their original values.
        /// If rendering is disabled, the render flags are stored, specific functions are disabled, and the render flags are set to 0.
        /// The main thread is resumed after modifying the rendering state.
        /// If rendering is disabled, the WorldFrame and UIParent are hidden.
        /// </summary>
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
        /// Sets the target GUID for injection and execution.
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
        /// Traces a line from a start point to an end point with specified flags.
        /// </summary>
        /// <param name="start">The starting point of the line.</param>
        /// <param name="end">The ending point of the line.</param>
        /// <param name="flags">The flags specifying additional tracing options.</param>
        /// <returns>True if the tracing is successful and the return address is not zero, false otherwise.</returns>
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
        /// Interacts with a unit identified by a specified global unique identifier (GUID).
        /// </summary>
        /// <param name="guid">The global unique identifier of the unit.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InteractWithUnit(ulong guid)
        {
            byte[] guidBytes = BitConverter.GetBytes(guid);

            InjectAndExecute(new string[]
            {
                $"PUSH {BitConverter.ToUInt32(guidBytes, 4)}",
                $"PUSH {BitConverter.ToUInt32(guidBytes, 0)}",
                $"CALL {Memory.Offsets.FunctionUnitOnRightClick}",
                "ADD ESP, 0x8",
                "RET"
            });
        }

        /// <summary>
        /// Clicks on the terrain at the specified position.
        /// </summary>
        /// <param name="position">The position on the terrain to click on.</param>
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
        /// Moves the player character to the specified position by allocating memory and calling the appropriate object function.
        /// </summary>
        /// <param name="playerBase">The base address of the player.</param>
        /// <param name="position">The target position to move the player character to.</param>
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
        /// <param name="cmdVarTuple">A tuple containing the command and variable.</param>
        /// <param name="result">The result of the execution.</param>
        /// <returns>True if the execution was successful, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteLuaAndRead((string, string) cmdVarTuple, out string result)
        {
            return ExecuteLuaAndRead(cmdVarTuple.Item1, cmdVarTuple.Item2, out result);
        }

        /// <summary>
        /// Executes a Lua command and reads the result.
        /// </summary>
        /// <param name="command">The Lua command to execute.</param>
        /// <param name="variable">The variable to pass to the Lua command.</param>
        /// <param name="result">The result of the Lua command execution.</param>
        /// <returns>True if the command was executed successfully and a non-empty result was obtained, otherwise false.</returns>
        public bool ExecuteLuaAndRead(string command, string variable, out string result)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentOutOfRangeException(nameof(command), "command is empty"); }
            if (string.IsNullOrWhiteSpace(variable)) { throw new ArgumentOutOfRangeException(nameof(variable), "variable is empty"); }
#endif
            AmeisenLogger.I.Log("548Hook", $"WowExecuteLuaAndRead: {command}", LogLevel.Verbose);

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
        /// FacePosition method is used to make the player face a specific position.
        /// </summary>
        /// <param name="playerBase">The player's base address.</param>
        /// <param name="playerPosition">The current player position.</param>
        /// <param name="positionToFace">The position that the player should face.</param>
        /// <param name="smooth">Determines whether the facing movement should be smooth or instant (default is instant).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FacePosition(IntPtr playerBase, Vector3 playerPosition, Vector3 positionToFace, bool smooth = false)
        {
            SetFacing(playerBase, BotMath.GetFacingAngle(playerPosition, positionToFace), smooth);
        }

        /// <summary>
        /// Retrieves localized text based on the provided variable.
        /// </summary>
        /// <param name="variable">The variable used to retrieve the localized text.</param>
        /// <param name="result">The localized text that is retrieved.</param>
        /// <returns>True if the localized text is successfully retrieved and is not empty; otherwise, false.</returns>
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

        /// <summary>
        /// Gets the reaction of a unit.
        /// </summary>
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
        /// Disables a function by checking if it has already been replaced and if not, saves the original function bytes and patches the memory with a specific opcode.
        /// </summary>
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
        /// Enables a function by restoring its original bytes if the RET opcode is present.
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
        /// <param name="address">The memory address where the function bytes are located.</param>
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