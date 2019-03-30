using System.Reflection.Emit;
using Harmony;

namespace NetworkSkins.Patches
{
    public static class PatchUtils
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
    }
}
