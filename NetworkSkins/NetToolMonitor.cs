using ColossalFramework;
using ICities;
using NetworkSkins.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NetworkSkins
{
    public class NetToolMonitor : MonoBehaviour
    {
        public static NetToolMonitor Instance;

        public delegate void ToolStateChangedEventHandler(bool state);
        public event ToolStateChangedEventHandler EventToolStateChanged;
        public delegate void PrefabChangedEventHandler(NetInfo netInfo);
        public event PrefabChangedEventHandler EventPrefabChanged;

        public bool NetInfoHasTrees => NetUtil.HasTrees(Prefab);
        public bool NetInfoHasCatenaries => NetUtil.HasCatenaries(Prefab);
        public bool NetInfoHasPillars => NetUtil.HasPillars(Prefab);
        public bool NetInfoIsColorable => NetUtil.IsColorable(Prefab);
        public bool NetInfoHasSurfaces => NetUtil.HasSurfaces(Prefab);
        public bool NetInfoHasStreetLights => NetUtil.HasStreetLights(Prefab);

        public NetInfo Prefab { get; private set; }

        private bool isNetToolEnabled;

        private void Awake() {
            Instance = this;
        }

        private void Update() {
            if (ToolsModifierControl.toolController.CurrentTool is NetTool netTool) {
                if(!isNetToolEnabled) {
                    isNetToolEnabled = true;
                    EventToolStateChanged?.Invoke(true);
                }
                if (netTool.Prefab != null && Prefab != netTool.Prefab) {
                    Prefab = netTool.m_prefab;
                    EventPrefabChanged?.Invoke(Prefab);
                }
            } else {
                if (isNetToolEnabled) {
                    isNetToolEnabled = false;
                    EventToolStateChanged?.Invoke(false);
                }
            }
        }
    }
}
