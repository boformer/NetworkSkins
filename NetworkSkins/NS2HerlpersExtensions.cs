using ColossalFramework;
using ICities;
using UnityEngine;
using System;

namespace NetworkSkins
{
    internal static class NS2HelpersExtensions
    {
        internal static ref NetNode ToNode(this ushort nodeID) => ref NetManager.instance.m_nodes.m_buffer[nodeID];
        internal static ref NetSegment ToSegment(this ushort nodeID) => ref NetManager.instance.m_segments.m_buffer[nodeID];
    }
}
