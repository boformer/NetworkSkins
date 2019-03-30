using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Special check for overhead wires on junctions and bends.
    /// Wires are only render when both connected segments have wires.
    /// </summary>
    public static class NetNodeRenderPatcher
    {
        private static Shader _electricityNetShader;

        public static bool ShouldRenderJunctionNode(NetInfo.Node node, ushort segment1, ushort segment2)
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
    }
}
