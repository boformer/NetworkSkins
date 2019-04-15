using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;

namespace NetworkSkins.GUI.Pillars
{
    public class PillarList : ListBase<BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }

        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsDefault(string itemID) {
            switch (NetworkSkinPanelController.PillarElevationCombination) {
                case Pillar.Elevated: return NetworkSkinPanelController.ElevatedBridgePillar.DefaultItem.Id == itemID;
                case Pillar.ElevatedMiddle: return NetworkSkinPanelController.ElevatedMiddlePillar.DefaultItem.Id == itemID;
                case Pillar.Bridge: return NetworkSkinPanelController.BridgeBridgePillar.DefaultItem.Id == itemID;
                case Pillar.BridgeMiddle: return NetworkSkinPanelController.BridgeMiddlePillar.DefaultItem.Id == itemID;
                default: return false;
            }
        }
    }
}