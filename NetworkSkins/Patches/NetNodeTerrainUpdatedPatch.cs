using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Mix of Transpiler/Prefix/Postfix
    /// This is all here to apply the correct terrain surface when networks with different skins are joined together
    /// The difficult case to handle is the case when multiple networks of the same NetInfo, but with different skins are joined together.
    /// Take a look at NetNodeTerrainUpdatedPatch.txt file to see what the transpiler is supposed to do!
    /// If an update breaks the transpiler, it should fail gracefully. In that case some crossings will no longer have the correct ground textures.
    /// </summary>
    [HarmonyPatch(typeof(NetNode), "TerrainUpdated")]
    public static class NetNodeTerrainUpdatedPatch
    {
        public static void Prefix(ref NetNode __instance, ushort nodeID, out TerrainSurfacePatcherState __state)
        {
            // Apply the ground texture patch to the NetInfo of the node
            // This is important for loose road ends without junctions
            __state = TerrainSurfacePatcher.Apply(__instance.Info, NetworkSkinManager.NodeSkins[nodeID]);
        }

        public static void Postfix(ref NetNode __instance, TerrainSurfacePatcherState __state)
        {
            TerrainSurfacePatcher.Revert(__instance.Info, __state);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var originalInstructions = new List<CodeInstruction>(instructions);

            var netInfoCreatePavementField = typeof(NetInfo).GetField("m_createPavement");
            var netInfoFlattenTerrainField = typeof(NetInfo).GetField("m_flattenTerrain");
            var segmentSkinsField = typeof(NetworkSkinManager).GetField("SegmentSkins", BindingFlags.Static | BindingFlags.Public);
            var terrainSurfacePatcherApplyMethod = typeof(TerrainSurfacePatcher).GetMethod("Apply");
            var terrainSurfacePatcherRevertMethod = typeof(TerrainSurfacePatcher).GetMethod("Revert");
            if (netInfoCreatePavementField == null || netInfoFlattenTerrainField == null || segmentSkinsField == null || terrainSurfacePatcherApplyMethod == null || terrainSurfacePatcherRevertMethod == null)
            {
                Debug.LogError("Necessary field and methods not found. Cancelling transpiler!");
                return originalInstructions;
            }


            var codes = new List<CodeInstruction>(originalInstructions);

            var patcherStateLocalVar = il.DeclareLocal(typeof(TerrainSurfacePatcherState));
            patcherStateLocalVar.SetLocalSymInfo("patcherState");
            
            int index = 0;

            object num14LocalVar = null;
            object num13LocalVar = null;
            object info4LocalVar = null;
            object netInfo2LocalVar = null;
            object segment7LocalVar = null;
            object num6LocalVar = null;
            for (; index < codes.Count; index++)
            {
                // NetInfo netInfo2 = (num14 > num13 >> 1) ? netInfo : info4;
                //IL_0a41: ldloc.s 74 (num14)
                //IL_0a43: ldloc.s 71 (num13)
                //IL_0a45: ldc.i4.1
                //IL_0a46: shr
                //IL_0a47: bgt IL_0a53
                //IL_0a4c: ldloc.s 40 (info4)
                //IL_0a4e: br IL_0a55
                //IL_0a53: ldloc.s 48 (netInfo)
                //IL_0a55: stloc.s 75 (netInfo2)
                if (codes[index].opcode == OpCodes.Shr 
                    && codes[index - 3].opcode == OpCodes.Ldloc_S
                    && codes[index - 2].opcode == OpCodes.Ldloc_S 
                    && codes[index - 1].opcode == OpCodes.Ldc_I4_1
                    // shr
                    && codes[index + 1].opcode == OpCodes.Bgt
                    && codes[index + 2].opcode == OpCodes.Ldloc_S
                    && codes[index + 3].opcode == OpCodes.Br
                    && codes[index + 4].opcode == OpCodes.Ldloc_S
                    && codes[index + 5].opcode == OpCodes.Stloc_S
                    )
                {
                    Debug.Log("Found NetInfo netInfo2 = (num14 > num13 >> 1) ? netInfo : info4;");
                    num14LocalVar = codes[index - 3].operand;
                    num13LocalVar = codes[index - 2].operand;
                    info4LocalVar = codes[index + 2].operand; // 40
                    var netInfoLocalVar = codes[index + 4].operand; // 48
                    netInfo2LocalVar = codes[index + 5].operand; // 75

                    var findIndex = 0;
                    segment7LocalVar = FindSegment7LocalVar(codes, info4LocalVar, ref findIndex, index - 3); // 38
                    num6LocalVar = FindNum6LocalVar(codes, netInfoLocalVar, ref findIndex, index - 3); // 51
                    break;
                }
            }

            Debug.Log($"num14: {num14LocalVar}, num13: {num13LocalVar}, info4: {info4LocalVar}, netInfo2: {netInfo2LocalVar}, segment7: {segment7LocalVar}, num6: {num6LocalVar}");

            if (num14LocalVar == null || num13LocalVar == null || info4LocalVar== null || netInfo2LocalVar == null || segment7LocalVar == null ||
                num6LocalVar == null)
            {
                Debug.LogError("Some local variables not found! Cancelling transpiler!");

                return originalInstructions;
            }

            // segment7 is the segmentID of info4
            // num6 is the segmentID of netInfo

            // TerrainSurfacePatcherState patcherState = TerrainSurfacePatcher.Apply(netInfo, NetworkSkinManager.SegmentSkins[(int)((num14 > num13 >> 1) ? num6 : segment7)]);
            // bool flag8 = netInfo2.m_createPavement && (!netInfo2.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
            var apply1Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag8 = netInfo2.m_createPavement && (!netInfo2.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
                if (codes[index].opcode == OpCodes.Ldloc_S && codes[index].operand == netInfo2LocalVar 
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoCreatePavementField)
                {
                    Debug.Log("Found info4.m_createPavement");

                    var ldLocSegment7Label = il.DefineLabel();
                    var ldElemtRefLabel = il.DefineLabel();

                    // TerrainSurfacePatcherState patcherState = TerrainSurfacePatcher.Apply(netInfo, NetworkSkinManager.SegmentSkins[(int)((num14 > num13 >> 1) ? num6 : segment7)]);
                    var apply1Instructions = new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, netInfo2LocalVar),
                        new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                        new CodeInstruction(OpCodes.Ldloc_S, num14LocalVar),
                        new CodeInstruction(OpCodes.Ldloc_S, num13LocalVar),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Shr),
                        new CodeInstruction(OpCodes.Bgt, ldLocSegment7Label),
                        new CodeInstruction(OpCodes.Ldloc_S, segment7LocalVar),
                        new CodeInstruction(OpCodes.Br, ldElemtRefLabel),
                        new CodeInstruction(OpCodes.Ldloc_S, num6LocalVar), // ldLocSegment7Label
                        new CodeInstruction(OpCodes.Ldelem_Ref), // ldElemtRefLabel
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherApplyMethod), 
                        new CodeInstruction(OpCodes.Stloc, patcherStateLocalVar)
                    };
                    apply1Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    apply1Instructions[9].labels.Add(ldLocSegment7Label);
                    apply1Instructions[10].labels.Add(ldElemtRefLabel);

                    codes.InsertRange(index, apply1Instructions);
                    Debug.Log("Apply 1 inserted");

                    apply1Inserted = true;
                    index += apply1Instructions.Length;
                    break;
                }
            }

            if (!apply1Inserted)
            {
                Debug.LogError("Apply Insertion 1 failed! Cancelling transpiler!");
                return originalInstructions;
            }


            // NetworkSkins.TerrainSurfacePatcher.Revert(netInfo2, patcherState);
            // bool flag12 = netInfo2.m_flattenTerrain || (netInfo2.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
            var revert1Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag12 = netInfo2.m_flattenTerrain || (netInfo2.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
                if (codes[index].opcode == OpCodes.Ldloc_S && codes[index].operand == netInfo2LocalVar
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoFlattenTerrainField)
                {
                    Debug.Log("Found info4.m_flattenTerrain");

                    var revert1Instructions = new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc, netInfo2LocalVar),
                        new CodeInstruction(OpCodes.Ldloc, patcherStateLocalVar),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherRevertMethod),
                    };

                    revert1Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, revert1Instructions);
                    Debug.Log("Revert 1 inserted");

                    revert1Inserted = true;
                    index += revert1Instructions.Length;
                    break;
                }
            }

            if (!revert1Inserted)
            {
                Debug.LogError("Revert Insertion 1 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            // patcherState = NetworkSkins.TerrainSurfacePatcher.Apply(info4, NetworkSkins.Skins.NetworkSkinManager.SegmentSkins[segment7]);
            // bool flag13 = info4.m_createPavement && (!info4.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
            var apply2Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag13 = info4.m_createPavement && (!info4.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
                if (codes[index].opcode == OpCodes.Ldloc_S && codes[index].operand == info4LocalVar
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoCreatePavementField)
                {
                    Debug.Log("Found info4.m_createPavement");

                    // patcherState = NetworkSkins.TerrainSurfacePatcher.Apply(info4, NetworkSkins.Skins.NetworkSkinManager.SegmentSkins[segment7]);
                    var apply2Instructions = new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, info4LocalVar),
                        new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                        new CodeInstruction(OpCodes.Ldloc_S, segment7LocalVar),
                        new CodeInstruction(OpCodes.Ldelem_Ref),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherApplyMethod),
                        new CodeInstruction(OpCodes.Stloc, patcherStateLocalVar)
                    };

                    apply2Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, apply2Instructions);
                    Debug.Log("Apply 2 inserted");

                    apply2Inserted = true;
                    index += apply2Instructions.Length;
                    break;
                }
            }

            if (!apply2Inserted)
            {
                Debug.LogError("Apply Insertion 2 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            // NetworkSkins.TerrainSurfacePatcher.Revert(info4, patcherState);
            // bool flag17 = info4.m_flattenTerrain || (info4.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
            var revert2Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag17 = info4.m_flattenTerrain || (info4.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
                if (codes[index].opcode == OpCodes.Ldloc_S && codes[index].operand == info4LocalVar
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoFlattenTerrainField)
                {
                    Debug.Log("Found info4.m_flattenTerrain");

                    // NetworkSkins.TerrainSurfacePatcher.Revert(info4, patcherState);
                    var revert2Instructions = new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, info4LocalVar),
                        new CodeInstruction(OpCodes.Ldloc, patcherStateLocalVar),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherRevertMethod),
                    };

                    revert2Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, revert2Instructions);
                    Debug.Log("Revert 2 inserted");

                    revert2Inserted = true;
                    index += revert2Instructions.Length;
                    break;
                }
            }

            if (!revert2Inserted)
            {
                Debug.LogError("Revert Insertion 2 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            return codes;
        }

        private static object FindSegment7LocalVar(List<CodeInstruction> codes, object info4LocalVar, ref int index, int endIndex)
        {
            for (; index < endIndex; index++)
            {
                // NetSegment netSegment3 = Singleton<NetManager>.instance.m_segments.m_buffer[segment7];
                // IL_05a9: call !0 class [ColossalManaged]ColossalFramework.Singleton`1<class NetManager>::get_instance()
                // IL_05ae: ldfld class Array16`1<valuetype NetSegment> NetManager::m_segments
                // IL_05b3: ldfld !0[] class Array16`1<valuetype NetSegment>::m_buffer
                // IL_05b8: ldloc.s 38
                // IL_05ba: ldelema NetSegment
                // IL_05bf: ldobj NetSegment
                // IL_05c4: stloc.s 39
                // NetInfo info4 = netSegment3.Info;
                // IL_05c6: ldloca.s 39
                // IL_05c8: call instance class NetInfo NetSegment::get_Info()
                // IL_05cd: stloc.s 40
                if (codes[index].opcode == OpCodes.Stloc_S && codes[index].operand == info4LocalVar)
                {
                    Debug.Log("Found info4 = ...");

                    if (codes[index - 6].opcode == OpCodes.Ldloc_S)
                    {
                        Debug.Log("Found segment7");
                        return codes[index - 6].operand;
                    }
                }
            }

            Debug.LogError("Unable to find segment7. Cancelling transpiler!");
            return null;
        }

        private static object FindNum6LocalVar(List<CodeInstruction> codes, object netInfoLocalVar, ref int index, int endIndex)
        {
            for (; index < endIndex; index++)
            {
                // NetSegment netSegment5 = Singleton<NetManager>.instance.m_segments.m_buffer[num6];
                // IL_07cc: call !0 class [ColossalManaged] ColossalFramework.Singleton`1<class NetManager>::get_instance()
                // IL_07d1: ldfld class Array16`1<valuetype NetSegment> NetManager::m_segments
                // IL_07d6: ldfld !0[] class Array16`1<valuetype NetSegment>::m_buffer
                // IL_07db: ldloc.s 51
                // IL_07dd: ldelema NetSegment
                // IL_07e2: ldobj NetSegment
                // IL_07e7: stloc.s 61
                // netInfo = netSegment5.Info;
                // IL_07e9: ldloca.s 61
                // IL_07eb: call instance class NetInfo NetSegment::get_Info()
                // IL_07f0: stloc.s 48
                if (codes[index].opcode == OpCodes.Stloc_S && codes[index].operand == netInfoLocalVar)
                {
                    Debug.Log("Found netInfo = ...");

                    if (codes[index - 6].opcode == OpCodes.Ldloc_S)
                    {
                        Debug.Log("Found num6");
                        return codes[index - 6].operand;
                    }
                }
            }

            Debug.LogError("Unable to find num6. Cancelling transpiler!");
            return null;
        }
    }
}