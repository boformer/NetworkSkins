using HarmonyLib;
using NetworkSkins.Patches.NetTool;
using NetworkSkins.Skins;
using System.Reflection;

// ReSharper disable InconsistentNaming

/*
 * 1- Make it work with name cheking (partially only)
 * 2- patch MoveMiddleNode() and SplitSegment()
 * 3- if 2 is unsucsseful complete 1.
 */

/*
0 at NetworkSkins.Patches.NetManager.NetManagerCreateSegmentPatch.Postfix(UInt16 ByRef segment, .NetInfo info, Boolean __result)
1 at NetManager.DMD<DMD<CreateSegment_Patch1>?1966313088::CreateSegment_Patch1>(.NetManager , UInt16 ByRef , Randomizer ByRef , .NetInfo , UInt16 , UInt16 , Vector3 , Vector3 , UInt32 , UInt32 , Boolean )
2 caller1 at NetTool.SplitSegment(UInt16 segment, UInt16 ByRef node, Vector3 position)
3 caller2 at NetTool.DMD<DMD<CreateNode_Patch0>?-1137352960::CreateNode_Patch0>(.NetInfo , ControlPoint , ControlPoint , ControlPoint , .FastList`1 , Int32 , Boolean , Boolean , Boolean , Boolean , Boolean , Boolean , Boolean , UInt16 , UInt16 ByRef , UInt16 ByRef , UInt16 ByRef , Int32 ByRef , Int32 ByRef )
4 caller3 at NetTool.CreateNode(.NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, .FastList`1 nodeBuffer, Int32 maxSegments, Boolean test, Boolean visualize, Boolean autoFix, Boolean needMoney, Boolean invert, Boolean switchDir, UInt16 relocateBuildingID, UInt16 ByRef node, UInt16 ByRef segment, Int32 ByRef cost, Int32 ByRef productionRate)
at NetTool.CreateNodeImpl(.NetInfo info, Boolean needMoney, Boolean switchDirection, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint)
at NetTool.CreateNodeImpl(Boolean switchDirection)
at NetTool+<CreateNode>c__Iterator0.MoveNext()
at AsyncTask`1[[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].Execute()
at SimulationManager.SimulationStep()
at SimulationManager.SimulationThread()

0 at NetworkSkins.Patches.NetManager.NetManagerCreateSegmentPatch.Postfix(UInt16 ByRef segment, .NetInfo info, Boolean __result)
1 at NetManager.DMD<DMD<CreateSegment_Patch1>?17123584::CreateSegment_Patch1>(.NetManager , UInt16 ByRef , Randomizer ByRef , .NetInfo , UInt16 , UInt16 , Vector3 , Vector3 , UInt32 , UInt32 , Boolean )
2 caller1 at NetTool.DMD<DMD<CreateNode_Patch0>?-1474291328::CreateNode_Patch0>(.NetInfo , ControlPoint , ControlPoint , ControlPoint , .FastList`1 , Int32 , Boolean , Boolean , Boolean , Boolean , Boolean , Boolean , Boolean , UInt16 , UInt16 ByRef , UInt16 ByRef , UInt16 ByRef , Int32 ByRef , Int32 ByRef )
3 caller2 at NetTool.CreateNode(.NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, .FastList`1 nodeBuffer, Int32 maxSegments, Boolean test, Boolean visualize, Boolean autoFix, Boolean needMoney, Boolean invert, Boolean switchDir, UInt16 relocateBuildingID, UInt16 ByRef node, UInt16 ByRef segment, Int32 ByRef cost, Int32 ByRef productionRate)
4 caller3 at NetTool.CreateNodeImpl(.NetInfo info, Boolean needMoney, Boolean switchDirection, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint)
at NetTool.CreateNodeImpl(Boolean switchDirection)
at NetTool+<CreateNode>c__Iterator0.MoveNext()
at AsyncTask`1[[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]].Execute()
at SimulationManager.SimulationStep()
at SimulationManager.SimulationThread()
 */
namespace NetworkSkins.Patches.NetManager
{
    // TODO check compat with ParallelRoadTool
    [HarmonyPatch(typeof(global::NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            var firstStackFrameIndex = 2;

            // 0 is this method
            // 1 is CreateSegment_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();
            var caller2 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 1).GetMethod();
            var caller3 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 2).GetMethod();
   
            // Support for ParallelRoadTool
            if (caller1.Name == "CreateSegmentOriginal" && caller2.Name == "CreateSegment")
            {
                firstStackFrameIndex += 2;
                caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();
                caller2 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 1).GetMethod();
                caller3 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 2).GetMethod();
            }

            if (TranspilerUtils.CompareMethods(NetToolCreateNodePatch.TargetMethod(), caller1))
            {
                if (TranspilerUtils.CompareMethods("CreateNode", caller2))
                {
                    // check that caller was called by NetTool
                    if (TranspilerUtils.IsMemberOf<global::NetTool>(caller3))
                    {
                        // new segment created by user, apply selected style
                        if (__result)
                        {
                            NetworkSkinManager.instance.OnSegmentPlaced(segment);
                        }

                        // Delete data of deleted segments
                        if (NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment > 0)
                        {
                            NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment);
                        }

                        if (NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment > 0)
                        {
                            NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment);
                        }

                        NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment = 0;
                        NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment = 0;
                    }
                }
                else if (TranspilerUtils.CompareMethods("LoadPaths", caller2))
                {
                    // segment created because user placed building with integrated network
                    // currently not doing anything
                }
            }
            // segment that was modified because user added network, apply style of previous segment
            else if (caller1.Name == "MoveMiddleNode" || caller1.Name.StartsWith("MoveMiddleNode_Patch"))
            {
                if (NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment > 0)
                {
                    if (__result)
                    {
                        NetworkSkinManager.instance.OnSegmentTransferData(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment, segment);
                    }

                    // Delete data of previous segment
                    NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment);
                    NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment = 0;
                }
            }
            // segment that was split by new node, apply style of previous segment
            else if (caller1.Name == "SplitSegment" || caller1.Name.StartsWith("SplitSegment_Patch"))
            {
                if (NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment > 0)
                {
                    if (__result)
                    {
                        NetworkSkinManager.instance.OnSegmentTransferData(NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment, segment);
                    }
                }
            }
        }
    }
}
