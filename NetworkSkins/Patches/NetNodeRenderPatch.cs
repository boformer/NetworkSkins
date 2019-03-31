using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Common patch logic for NetNode.RenderInstance, NetNode.CalculateGroupData and NetNode.PopulateGroupData
    /// Special check for overhead wires on junctions and bends.
    /// Wires are only render when both connected segments have wires.
    /// </summary>
    public static class NetNodeRenderPatch
    {
        private static Shader _electricityNetShader;

        public static bool ShouldRenderJunctionNode(NetInfo.Node node, ushort segment1, ushort segment2)
        {
            if (_electricityNetShader == null)
            {
                _electricityNetShader = Shader.Find("Custom/Net/Electricity"); // TODO maybe move this out for better perf?
            }

            if (node.m_material?.shader != _electricityNetShader)
            {
                return true;
            }

            var segmentSkin1 = NetworkSkinManager.SegmentSkins[segment1];
            var segmentSkin2 = NetworkSkinManager.SegmentSkins[segment2];
            return (segmentSkin1 == null || segmentSkin1.m_hasWires) && (segmentSkin2 == null || segmentSkin2.m_hasWires);
        }

        public static bool ShouldRenderBendNode(NetInfo.Node node, ushort segment1, ushort segment2)
        {
            if (_electricityNetShader == null)
            {
                _electricityNetShader = Shader.Find("Custom/Net/Electricity");
            }

            if (node.m_material?.shader != _electricityNetShader)
            {
                return true;
            }

            var segmentSkin1 = NetworkSkinManager.SegmentSkins[segment1];
            var segmentSkin2 = NetworkSkinManager.SegmentSkins[segment2];
            return (segmentSkin1 == null || segmentSkin1.m_hasWires) && (segmentSkin2 == null || segmentSkin2.m_hasWires);
        }

        public static bool ShouldRenderBendNodeLod(ushort nodeId, NetInfo.Node node)
        {
            if (_electricityNetShader == null)
            {
                _electricityNetShader = Shader.Find("Custom/Net/Electricity");
            }

            if (node.m_material?.shader != _electricityNetShader)
            {
                return true;
            }

            // copied from CalculateGroupData
            var netManager = NetManager.instance;
            ushort segmentId1 = 0;
            ushort segmentId2 = 0;
            var segmentId1Found = false;
            var counter = 0;
            for (var s = 0; s < 8; s++)
            {
                var segmentId = netManager.m_nodes.m_buffer[nodeId].GetSegment(s);
                if (segmentId == 0) continue;

                var netSegment = netManager.m_segments.m_buffer[segmentId];
                var first = ++counter == 1;
                var isStartNode = (netSegment.m_startNode == nodeId);
                if (!first && !segmentId1Found)
                {
                    segmentId1Found = true;
                    segmentId1 = segmentId;
                }
                else if (first && !isStartNode)
                {
                    segmentId1Found = true;
                    segmentId1 = segmentId;
                }
                else
                {
                    segmentId2 = segmentId;
                }
            }

            Debug.Log($"segmentId1: {segmentId1} segmentId2: {segmentId2}");

            var segmentSkin1 = NetworkSkinManager.SegmentSkins[segmentId1];
            var segmentSkin2 = NetworkSkinManager.SegmentSkins[segmentId2];
            return (segmentSkin1 == null || segmentSkin1.m_hasWires) && (segmentSkin2 == null || segmentSkin2.m_hasWires);
        }
    }
}
