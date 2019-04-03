using System;

// ReSharper disable InconsistentNaming

namespace NetworkSkins
{
    public static class BuildingAiRenderMeshesPatch
    {
        public static bool Prefix(BuildingAI __instance, RenderManager.CameraInfo cameraInfo, ushort buildingID, ref Building data, int layerMask, ref RenderManager.Instance instance)
        {
            __instance.m_info.m_rendered = true;
            if (__instance.m_info.m_mesh != null)
            {
                BuildingAI.RenderMesh(cameraInfo, buildingID, ref data, __instance.m_info, ref instance);
            }
            if (__instance.m_info.m_subMeshes != null)
            {
                for (int i = 0; i < __instance.m_info.m_subMeshes.Length; i++)
                {
                    BuildingInfo.MeshInfo meshInfo = __instance.m_info.m_subMeshes[i];
                    if (((meshInfo.m_flagsRequired | meshInfo.m_flagsForbidden) & data.m_flags) == meshInfo.m_flagsRequired)
                    {
                        BuildingInfoSub buildingInfoSub = meshInfo.m_subInfo as BuildingInfoSub;
                        buildingInfoSub.m_rendered = true;
                        if (buildingInfoSub.m_subMeshes != null && buildingInfoSub.m_subMeshes.Length != 0)
                        {
                            for (int j = 0; j < buildingInfoSub.m_subMeshes.Length; j++)
                            {
                                BuildingInfo.MeshInfo meshInfo2 = buildingInfoSub.m_subMeshes[j];
                                if (((meshInfo2.m_flagsRequired | meshInfo2.m_flagsForbidden) & data.m_flags) == meshInfo2.m_flagsRequired)
                                {
                                    BuildingInfoSub buildingInfoSub2 = meshInfo2.m_subInfo as BuildingInfoSub;
                                    buildingInfoSub2.m_rendered = true;
                                    BuildingAI.RenderMesh(cameraInfo, __instance.m_info, buildingInfoSub2, meshInfo.m_matrix, ref instance);
                                }
                            }
                        }
                        else
                        {
                            BuildingAI.RenderMesh(cameraInfo, __instance.m_info, buildingInfoSub, meshInfo.m_matrix, ref instance);
                        }
                    }
                }
            }
            return false;
        }
    }
}
