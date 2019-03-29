// TODO results in crash
/*
using System;
using System.Reflection;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    [HarmonyPatch]
    public static class RoadBaseAiGetNodeColorPatch
    {
        public static MethodBase TargetMethod()
        {
            // GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
            return typeof(NetAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(NetNode).MakeByRefType(), typeof(InfoManager.InfoMode) });
        }

        public static void Prefix(ref RoadBaseAI __instance, ushort nodeID, out Color? __state)
        {
            __state = ColorPatcher.Apply(__instance.m_info, NetworkSkinManager.NodeSkins[nodeID]);
        }
        
        public static void Postfix(ref RoadBaseAI __instance, Color? __state)
        {
            ColorPatcher.Revert(__instance.m_info, __state);
        }
    }
}
*/