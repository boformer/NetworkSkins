using Harmony;

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            // 0 is this method
            // 1 is CreateSegment_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(2).GetMethod();
            var caller2 = new System.Diagnostics.StackFrame(3).GetMethod();
            var caller3 = new System.Diagnostics.StackFrame(4).GetMethod();

            // TODO test if it is compatible
            // Support for ParallelRoadTool
            if (caller1.Name == "CreateSegmentOriginal" && caller2.Name == "CreateSegment")
            {
                caller1 = new System.Diagnostics.StackFrame(4).GetMethod();
                caller2 = new System.Diagnostics.StackFrame(5).GetMethod();
                caller3 = new System.Diagnostics.StackFrame(6).GetMethod();
            }

            switch (caller1.Name)
            {
                case "CreateNode":

                    //var caller2 = new System.Diagnostics.StackFrame(2).GetMethod().Name;
                    //Debug.Log("... called by " + caller2);

                    if (caller2.Name == "CreateNode") 
                    {
                        // check that caller was called by NetTool
                        var caller3Type = caller3.DeclaringType;
                        if (caller3Type != null && (typeof(NetTool).IsAssignableFrom(caller3Type))) // new segment created by user, apply selected style

                        {
                            if (__result)
                            {
                                NetManagerHooks.OnSegmentCreate(segment);
                            }

                            // Delete data of deleted segments
                            if (NetManagerHooks.MoveMiddleNode_releasedSegment > 0)
                            {
                                NetManagerHooks.OnSegmentRelease(NetManagerHooks.MoveMiddleNode_releasedSegment);
                            }

                            if (NetManagerHooks.SplitSegment_releasedSegment > 0)
                            {
                                NetManagerHooks.OnSegmentRelease(NetManagerHooks.SplitSegment_releasedSegment);
                            }

                            NetManagerHooks.SplitSegment_releasedSegment = 0;
                            NetManagerHooks.MoveMiddleNode_releasedSegment = 0;
                        }
                    }
                    else if (caller2.Name == "LoadPaths")
                    {
                        // segment created because user placed building with integrated network
                        // currently not doing anything
                    }
                    break;

                case "MoveMiddleNode": // segment that was modified because user added network, apply style of previous segment

                    if (NetManagerHooks.MoveMiddleNode_releasedSegment > 0)
                    {
                        if (__result)
                        {
                            NetManagerHooks.OnSegmentTransferData(NetManagerHooks.MoveMiddleNode_releasedSegment, segment);
                        }

                        // Delete data of previous segment
                        NetManagerHooks.OnSegmentRelease(NetManagerHooks.MoveMiddleNode_releasedSegment);
                        NetManagerHooks.MoveMiddleNode_releasedSegment = 0;
                    }
                    break;

                case "SplitSegment": // segment that was split by new node, apply style of previous segment

                    if (NetManagerHooks.SplitSegment_releasedSegment > 0)
                    {
                        if (__result)
                        {
                            NetManagerHooks.OnSegmentTransferData(NetManagerHooks.SplitSegment_releasedSegment, segment);
                        }
                    }
                    break;

                default: // unknown caller, ignore
                    break;
            }
        }
    }
}
