using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI.Catenaries
{
    public class CatenaryList : ListBase<PropInfo>
    {
        protected override Vector2 ListSize => NetworkSkinPanelController.Catenary.Items.Count < 10 ? new Vector2(base.ListSize.x - 12.0f, RowHeight * NetworkSkinPanelController.Catenary.Items.Count) : base.ListSize;
        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }
        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsDefault(string itemID) {
            return NetworkSkinPanelController.Catenary.DefaultItem.Id == itemID;
        }
    }
}