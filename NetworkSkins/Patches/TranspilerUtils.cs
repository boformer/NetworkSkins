using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace NetworkSkins.Patches
{
    public static class TranspilerUtils
    {
        // nullable
        public static CodeInstruction GetLdLocForStLoc(CodeInstruction instruction)
        {
            if (instruction.opcode == OpCodes.Stloc_0)
            {
                return new CodeInstruction(OpCodes.Ldloc_0);
            }
            else if (instruction.opcode == OpCodes.Stloc_1)
            {
                return new CodeInstruction(OpCodes.Ldloc_1);
            }
            else if (instruction.opcode == OpCodes.Stloc_2)
            {
                return new CodeInstruction(OpCodes.Ldloc_2);
            }
            else if (instruction.opcode == OpCodes.Stloc_3)
            {
                return new CodeInstruction(OpCodes.Ldloc_3);
            }
            else if (instruction.opcode == OpCodes.Stloc_S)
            {
                return new CodeInstruction(OpCodes.Ldloc_S, instruction.operand);
            }
            else if (instruction.opcode == OpCodes.Stloc)
            {
                return new CodeInstruction(OpCodes.Ldloc, instruction.operand);
            }
            else
            {
                return null;
            }
        }

        public static bool IsSameInstruction(CodeInstruction a, CodeInstruction b, bool debug = false)
        {
            if (a.opcode == b.opcode)
            {
                if (a.operand == b.operand)
                {
                    return true;
                }

                if (debug)
                {
                    Debug.Log($"IsSameInstruction {a.operand} {a.operand?.GetType()} == {b.operand} {b.operand?.GetType()}?");
                }

                // This special code is needed for some reason because the != operator doesn't work on System.Byte
                return (a.operand is byte aByte && b.operand is byte bByte && aByte == bByte);
            }
            else
            {
                return false;
            }
        }
    }
}
