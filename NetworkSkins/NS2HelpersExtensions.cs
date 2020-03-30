using ColossalFramework;
using ICities;
using UnityEngine;
using System;

namespace NetworkSkins
{
    internal static class NS2HelpersExtensions
    {
        public static ushort GetFirstSegment(ushort nodeID)
        {
            ref global::NetNode node = ref global::NetManager.instance.m_nodes.m_buffer[nodeID];
            ushort ret = 0;
            for (int i = 0; i < 8; ++i)
            {
                ret = node.GetSegment(i);
                if (ret > 0)
                    break;
            }
            return ret;
        }

        internal static void Assert(bool con, string m)
        {
            if (!con)
            {
                Debug.LogError("Assertion failed: " + m + "\n" + Environment.StackTrace);
            }
        }

        internal static bool InSimulationThread() => 
            System.Threading.Thread.CurrentThread == SimulationManager.instance.m_simulationThread;
    }
}
