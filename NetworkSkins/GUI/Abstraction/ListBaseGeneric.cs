using ColossalFramework.UI;
using NetworkSkins.GUI.UIFastList;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListBase<T> : ListBase where T : PrefabInfo
    {
        protected static List<ListPanelController<T>.Item> favouritesList = new List<ListPanelController<T>.Item>();
        protected static List<ListPanelController<T>.Item> nonFavouritesList = new List<ListPanelController<T>.Item>();
        protected override Vector2 ListSize => new Vector2(390.0f, 500.0f);
        protected override float RowHeight => 50.0f;

        private ListPanelController<T> GetController() {
            switch (PanelType) {
                case PanelType.Trees: {
                    switch (NetworkSkinPanelController.LanePosition) {
                        case Net.LanePosition.Left: return NetworkSkinPanelController.LeftTree as ListPanelController<T>;
                        case Net.LanePosition.Middle: return NetworkSkinPanelController.MiddleTree as ListPanelController<T>;
                        case Net.LanePosition.Right: return NetworkSkinPanelController.RighTree as ListPanelController<T>;
                        default: return null;
                    }
                }
                case PanelType.Lights: return NetworkSkinPanelController.StreetLight as ListPanelController<T>;
                case PanelType.Pillars: {
                    switch (NetworkSkinPanelController.PillarElevationCombination) {
                        case Net.Pillar.Elevated: return NetworkSkinPanelController.ElevatedBridgePillar as ListPanelController<T>;
                        case Net.Pillar.ElevatedMiddle: return NetworkSkinPanelController.ElevatedMiddlePillar as ListPanelController<T>;
                        case Net.Pillar.Bridge: return NetworkSkinPanelController.BridgeBridgePillar as ListPanelController<T>;
                        case Net.Pillar.BridgeMiddle: return NetworkSkinPanelController.BridgeMiddlePillar as ListPanelController<T>;
                        default: return null;
                    }
                }
                case PanelType.Catenary: return NetworkSkinPanelController.Catenary as ListPanelController<T>;
                default: return null;
            }
        }

        protected override void SetupRowsData() {
            int selectedIndex = 0;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            ListPanelController<T> controller = GetController();
            var itemCount = controller.Items.Count;
            fastList.RowsData.SetCapacity(itemCount);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            int index = 0;
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            foreach (ListPanelController<T>.Item item in controller.Items) {
                if (item.Id == "#NONE#" || item.Id == "#DEFAULT#") {
                    ListItem listItem = CreateListItem(item);
                    if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                    fastList.RowsData.Add(listItem);
                    index++;
                    continue;
                } else if (favList.Contains(item.Id)) {
                    favouritesList.Add(item);
                } else nonFavouritesList.Add(item);
            }
            for (int i = 0; i < favouritesList.Count; i++) {
                ListPanelController<T>.Item item = favouritesList[i] as ListPanelController<T>.Item;
                ListItem listItem = CreateListItem(item);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                ListPanelController<T>.Item item = nonFavouritesList[i] as ListPanelController<T>.Item;
                ListItem listItem = CreateListItem(item);

                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            fastList.DisplayAt(selectedIndex);
            fastList.SelectedIndex = selectedIndex;
        }

        protected ListItem CreateListItem(ListPanelController<T>.Item item) {
            string id = item.Id;
            bool isFavourite = IsFavourite(id);
            bool isDefault = IsDefault(id);

            string prefix = isDefault
                ? string.Concat("(", Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT), ") ")
                : string.Empty;

            string name = id == "#NONE#"
               ? Translation.Instance.GetTranslation(TranslationID.LABEL_NONE)
               : item is ListPanelController<T>.SimpleItem si1
               ? si1.Value.GetName()
               : string.Empty;
            string displayName = string.Concat(prefix, name);
            
            Texture2D thumbnail = id == "#NONE#"
                ? UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet")
                : item is ListPanelController<T>.SimpleItem si2
                ? si2.Value.m_Atlas?.GetSpriteTexture(si2.Value.m_Thumbnail)
                : UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("BuildingIcon");

            ItemType type = UIUtil.PanelToItemType(PanelType);
            return new ListItem(id, displayName, thumbnail, isFavourite, type);
        }
    }
}
