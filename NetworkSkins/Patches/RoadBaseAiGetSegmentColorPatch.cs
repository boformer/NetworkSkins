// TODO maybe remove
/*
using System;
using System.Reflection;
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    [HarmonyPatch]
    public class RoadBaseAiGetSegmentColorPatch
    {
        static MethodBase TargetMethod()
        {
            // GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
            return typeof(RoadBaseAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(NetSegment).MakeByRefType(), typeof(InfoManager.InfoMode) });
        }

        static void Prefix(ref RoadBaseAI __instance, ushort segmentID, out Color? __state)
        {
            __state = ColorPatcher.Apply(__instance.m_info, NetworkSkinManager.NodeSkins[segmentID]);
        }

        static void Postfix(ref RoadBaseAI __instance, ref Color? __state)
        {
            ColorPatcher.Revert(__instance.m_info, __state);
        }
    }
}
*/