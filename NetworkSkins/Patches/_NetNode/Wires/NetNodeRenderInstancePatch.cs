using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace NetworkSkins.Patches._NetNode.Wires {

    [HarmonyPatch2(delcaringType: typeof(NetNode), delegateType: typeof(RenderInstance))]
    public static class NetNodeRenderInstancePatch {
        delegate void RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, NetNode.FlagsLong flags, ref uint instanceIndex, ref RenderManager.Instance data);

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {
            var codes = instructions.ToList();
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 1, 2); //DC
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 4, 2); // DC bend
            return codes;
        }
    }
}
