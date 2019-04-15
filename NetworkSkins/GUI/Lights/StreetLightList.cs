using NetworkSkins.GUI.Abstraction;

namespace NetworkSkins.GUI.Lights
{
    public class StreetLightList : ListBase<PropInfo>
    {
        public void RefreshRowsData() {
            SetupRowsData();
        }

        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }

        protected override bool IsDefault(string itemID) {
            return NetworkSkinPanelController.StreetLight.DefaultItem.Id == itemID;
        }
    }
}
