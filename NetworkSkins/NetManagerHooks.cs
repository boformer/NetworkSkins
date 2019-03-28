using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins
{
    public class NetManagerHooks
    {
        public delegate void SegmentTransferDataEventHandler(ushort oldSegment, ushort newSegment);
        public static event SegmentTransferDataEventHandler EventSegmentTransferData;

        public delegate void SegmentCreateEventHandler(ushort segment);
        public static event SegmentCreateEventHandler EventSegmentCreate;

        public delegate void SegmentReleaseEventHandler(ushort segment);
        public static event SegmentReleaseEventHandler EventSegmentRelease;

        public delegate void NodeReleaseEventHandler(ushort node);
        public static event NodeReleaseEventHandler EventNodeRelease;

        // reset both on CreateSegment call by CreateNode
        public static ushort SplitSegment_releasedSegment;
        public static ushort MoveMiddleNode_releasedSegment;

        public static void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            Debug.Log($"SegmentTransferData {oldSegment} -> {newSegment}");
            EventSegmentTransferData?.Invoke(oldSegment, newSegment);
        }

        public static void OnSegmentCreate(ushort segment)
        {
            Debug.Log($"SegmentCreate {segment}");
            EventSegmentCreate?.Invoke(segment);
        }

        public static void OnSegmentRelease(ushort segment)
        {
            Debug.Log($"SegmentRelease {segment}");
            EventSegmentRelease?.Invoke(segment);
        }

        public static void OnNodeRelease(ushort node)
        {
            Debug.Log($"NodeRelease {node}");
            EventNodeRelease?.Invoke(node);
        }
    }
}
