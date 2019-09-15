using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.SimulationManager
{
    [HarmonyPatch(typeof(global::SimulationManager), "Managers_UpdateData")]
    public static class SimulationManagerManagersUpdateDataPatch
    {
        public static void Prefix(global::SimulationManager.UpdateMode mode)
        {
            NetworkSkinManager.instance.OnPreUpdateData(mode);
        }
    }
}
