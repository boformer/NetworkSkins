using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches.NetNode
{
    /// <summary>
    /// Used by terrain surface.
    /// Mix of Transpiler/Prefix/Postfix.
    /// This is all here to apply the correct terrain surface when networks with different skins are joined together
    /// The difficult case to handle is the case when multiple networks of the same NetInfo, but with different skins are joined together.
    /// Take a look at NetNodeTerrainUpdatedPatch.txt file to see what the transpiler is supposed to do!
    /// If an update breaks the transpiler, it should fail gracefully. In that case some crossings will no longer have the correct ground textures.
    /// </summary>
    [HarmonyPatch(typeof(global::NetNode), "TerrainUpdated")]
    public static class NetNodeTerrainUpdatedPatch
    {
        public static void Prefix(ref global::NetNode __instance, ushort nodeID, out TerrainSurfacePatcherState __state)
        {
            // Apply the ground texture patch to the NetInfo of the node
            // This is important for loose road ends without junctions
            __state = TerrainSurfacePatcher.Apply(__instance.Info, NetworkSkinManager.NodeSkins[nodeID]);
        }

        public static void Postfix(ref global::NetNode __instance, TerrainSurfacePatcherState __state)
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
                Debug.LogError("NetNodeTerrainUpdatedPatch: Necessary field and methods not found. Cancelling transpiler!");
                return originalInstructions;
            }


            var codes = new List<CodeInstruction>(originalInstructions);

            var patcherStateLocalVar = il.DeclareLocal(typeof(TerrainSurfacePatcherState));
            patcherStateLocalVar.SetLocalSymInfo("patcherState");

            int index = 0;

            CodeInstruction num14LocalVarLdLoc = null;
            CodeInstruction num13LocalVarLdLoc = null;
            CodeInstruction info4LocalVarLdLoc = null;
            CodeInstruction netInfo2LocalVarLdLoc = null;
            CodeInstruction segment7LocalVarLdLoc = null;
            CodeInstruction num6LocalVarLdLoc = null;
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
                    && TranspilerUtils.IsLdLoc(codes[index - 3])
                    && TranspilerUtils.IsLdLoc(codes[index - 2])
                    && codes[index - 1].opcode == OpCodes.Ldc_I4_1
                    // shr
                    && codes[index + 1].opcode == OpCodes.Bgt
                    && TranspilerUtils.IsLdLoc(codes[index + 2])
                    && codes[index + 3].opcode == OpCodes.Br
                    && TranspilerUtils.IsLdLoc(codes[index + 4])
                    && TranspilerUtils.IsStLoc(codes[index + 5])
                    )
                {
                    TranspilerUtils.LogDebug("Found NetInfo netInfo2 = (num14 > num13 >> 1) ? netInfo : info4;");
                    num14LocalVarLdLoc = TranspilerUtils.BuildLdLocFromLdLoc(codes[index - 3]);
                    num13LocalVarLdLoc = TranspilerUtils.BuildLdLocFromLdLoc(codes[index - 2]);
                    info4LocalVarLdLoc = TranspilerUtils.BuildLdLocFromLdLoc(codes[index + 2]); // 40
                    var netInfoLocalVarLdLoc = TranspilerUtils.BuildLdLocFromLdLoc(codes[index + 4]); // 48
                    netInfo2LocalVarLdLoc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 5]); // 75

                    var findIndex = 0;
                    segment7LocalVarLdLoc = FindSegment7LocalVar(codes, TranspilerUtils.BuildStLocFromLdLoc(info4LocalVarLdLoc), ref findIndex, index - 3); // 38
                    num6LocalVarLdLoc = FindNum6LocalVar(codes, TranspilerUtils.BuildStLocFromLdLoc(netInfoLocalVarLdLoc), ref findIndex, index - 3); // 51
                    break;
                }
            }



            if (num14LocalVarLdLoc == null || num13LocalVarLdLoc == null || info4LocalVarLdLoc == null || netInfo2LocalVarLdLoc == null || segment7LocalVarLdLoc == null || num6LocalVarLdLoc == null)
            {
                Debug.LogError("NetNodeTerrainUpdatedPatch: Some local variables not found! Cancelling transpiler!");
                Debug.LogError($"num14: {num14LocalVarLdLoc}, num13: {num13LocalVarLdLoc}, info4: {info4LocalVarLdLoc}, netInfo2: {netInfo2LocalVarLdLoc}, segment7: {segment7LocalVarLdLoc}, num6: {num6LocalVarLdLoc}");

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
                if (TranspilerUtils.IsSameInstruction(codes[index], netInfo2LocalVarLdLoc)
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoCreatePavementField)
                {
                    TranspilerUtils.LogDebug("Found info4.m_createPavement");

                    var ldLocSegment7Label = il.DefineLabel();
                    var ldElemtRefLabel = il.DefineLabel();

                    // TerrainSurfacePatcherState patcherState = TerrainSurfacePatcher.Apply(netInfo, NetworkSkinManager.SegmentSkins[(int)((num14 > num13 >> 1) ? num6 : segment7)]);
                    var apply1Instructions = new[]
                    {
                        new CodeInstruction(netInfo2LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                        new CodeInstruction(num14LocalVarLdLoc),
                        new CodeInstruction(num13LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Shr),
                        new CodeInstruction(OpCodes.Bgt, ldLocSegment7Label),
                        new CodeInstruction(segment7LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Br, ldElemtRefLabel),
                        new CodeInstruction(num6LocalVarLdLoc), // ldLocSegment7Label
                        new CodeInstruction(OpCodes.Ldelem_Ref), // ldElemtRefLabel
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherApplyMethod),
                        new CodeInstruction(OpCodes.Stloc, patcherStateLocalVar)
                    };
                    apply1Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    apply1Instructions[9].labels.Add(ldLocSegment7Label);
                    apply1Instructions[10].labels.Add(ldElemtRefLabel);

                    codes.InsertRange(index, apply1Instructions);
                    TranspilerUtils.LogDebug("Apply 1 inserted");

                    apply1Inserted = true;
                    index += apply1Instructions.Length;
                    break;
                }
            }

            if (!apply1Inserted)
            {
                Debug.LogError("NetNodeTerrainUpdatedPatch: Apply Insertion 1 failed! Cancelling transpiler!");
                return originalInstructions;
            }


            // NetworkSkins.TerrainSurfacePatcher.Revert(netInfo2, patcherState);
            // bool flag12 = netInfo2.m_flattenTerrain || (netInfo2.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
            var revert1Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag12 = netInfo2.m_flattenTerrain || (netInfo2.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
                if (TranspilerUtils.IsSameInstruction(codes[index], netInfo2LocalVarLdLoc)
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoFlattenTerrainField)
                {
                    TranspilerUtils.LogDebug("Found info4.m_flattenTerrain");

                    var revert1Instructions = new[]
                    {
                        new CodeInstruction(netInfo2LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldloc, patcherStateLocalVar),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherRevertMethod),
                    };

                    revert1Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, revert1Instructions);
                    TranspilerUtils.LogDebug("Revert 1 inserted");

                    revert1Inserted = true;
                    index += revert1Instructions.Length;
                    break;
                }
            }

            if (!revert1Inserted)
            {
                Debug.LogError("NetNodeTerrainUpdatedPatch: Revert Insertion 1 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            // patcherState = NetworkSkins.TerrainSurfacePatcher.Apply(info4, NetworkSkins.Skins.NetworkSkinManager.SegmentSkins[segment7]);
            // bool flag13 = info4.m_createPavement && (!info4.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
            var apply2Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag13 = info4.m_createPavement && (!info4.m_lowerTerrain || (m_flags & Flags.OnGround) != Flags.None);
                if (TranspilerUtils.IsSameInstruction(codes[index], info4LocalVarLdLoc)
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoCreatePavementField)
                {
                    TranspilerUtils.LogDebug("Found info4.m_createPavement");

                    // patcherState = NetworkSkins.TerrainSurfacePatcher.Apply(info4, NetworkSkins.Skins.NetworkSkinManager.SegmentSkins[segment7]);
                    var apply2Instructions = new[]
                    {
                        new CodeInstruction(info4LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                        new CodeInstruction(segment7LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldelem_Ref),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherApplyMethod),
                        new CodeInstruction(OpCodes.Stloc, patcherStateLocalVar)
                    };

                    apply2Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, apply2Instructions);
                    TranspilerUtils.LogDebug("Apply 2 inserted");

                    apply2Inserted = true;
                    index += apply2Instructions.Length;
                    break;
                }
            }

            if (!apply2Inserted)
            {
                Debug.LogError("NetNodeTerrainUpdatedPatch: Apply Insertion 2 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            // NetworkSkins.TerrainSurfacePatcher.Revert(info4, patcherState);
            // bool flag17 = info4.m_flattenTerrain || (info4.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
            var revert2Inserted = false;
            for (; index < codes.Count; index++)
            {
                // bool flag17 = info4.m_flattenTerrain || (info4.m_netAI.FlattenGroundNodes() && (m_flags & Flags.OnGround) != Flags.None);
                if (TranspilerUtils.IsSameInstruction(codes[index], info4LocalVarLdLoc)
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoFlattenTerrainField)
                {
                    TranspilerUtils.LogDebug("Found info4.m_flattenTerrain");

                    // NetworkSkins.TerrainSurfacePatcher.Revert(info4, patcherState);
                    var revert2Instructions = new[]
                    {
                        new CodeInstruction(info4LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Ldloc, patcherStateLocalVar),
                        new CodeInstruction(OpCodes.Call, terrainSurfacePatcherRevertMethod),
                    };

                    revert2Instructions[0].labels.AddRange(codes[index].labels);
                    codes[index].labels.Clear();

                    codes.InsertRange(index, revert2Instructions);
                    TranspilerUtils.LogDebug("Revert 2 inserted");

                    revert2Inserted = true;
                    index += revert2Instructions.Length;
                    break;
                }
            }

            if (!revert2Inserted)
            {
                Debug.LogError("NetNodeTerrainUpdatedPatch: Revert Insertion 2 failed! Cancelling transpiler!");
                return originalInstructions;
            }

            return codes;
        }

        private static CodeInstruction FindSegment7LocalVar(List<CodeInstruction> codes, CodeInstruction info4LocalVarStLoc, ref int index, int endIndex)
        {
            if (!TranspilerUtils.IsStLoc(info4LocalVarStLoc))
            {
                Debug.LogError("info4LocalVarStLoc is not stloc! Cancelling transpiler!");
                return null;
            }

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
            for (; index < endIndex; index++)
            {
                // IL_05cd: stloc.s 40
                if (TranspilerUtils.IsSameInstruction(codes[index], info4LocalVarStLoc))
                {
                    TranspilerUtils.LogDebug("Found info4 = ...");

                    // // IL_05b8: ldloc.s 38
                    if (TranspilerUtils.IsLdLoc(codes[index - 6]))
                    {
                        TranspilerUtils.LogDebug("Found segment7");
                        return TranspilerUtils.BuildLdLocFromLdLoc(codes[index - 6]);
                    }
                }
            }

            Debug.LogError("Unable to find segment7. Cancelling transpiler!");
            return null;
        }

        private static CodeInstruction FindNum6LocalVar(List<CodeInstruction> codes, CodeInstruction netInfoLocalVarStLoc, ref int index, int endIndex)
        {
            if (!TranspilerUtils.IsStLoc(netInfoLocalVarStLoc))
            {
                Debug.LogError("netInfoLocalVarStLoc is not stloc! Cancelling transpiler!");
                return null;
            }

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
            for (; index < endIndex; index++)
            {
                // IL_07f0: stloc.s 48
                if (TranspilerUtils.IsSameInstruction(codes[index], netInfoLocalVarStLoc))
                {
                    TranspilerUtils.LogDebug("Found netInfo = ...");

                    // IL_07db: ldloc.s 51
                    if (TranspilerUtils.IsLdLoc(codes[index - 6]))
                    {
                        TranspilerUtils.LogDebug("Found num6");
                        return TranspilerUtils.BuildLdLocFromLdLoc(codes[index - 6]);
                    }
                }
            }

            Debug.LogError("Unable to find num6. Cancelling transpiler!");
            return null;
        }
    }
}