namespace NetworkSkins {
    using System;
    using UnityEngine;

    public static class NSUtil {
        internal static bool InSimulationThread() =>
            System.Threading.Thread.CurrentThread == SimulationManager.instance.m_simulationThread;
    }
}
