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
        protected static List<ListPanelController<T>.Item> FavouritesList = new List<ListPanelController<T>.Item>();
        protected static List<ListPanelController<T>.Item> Blacklist = new List<ListPanelController<T>.Item>();
        protected static List<ListPanelController<T>.Item> NormalList = new List<ListPanelController<T>.Item>();
        protected override Vector2 ListSize => new Vector2(390.0f, 500.0f);
        protected override float RowHeight => 50.0f;

        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }

        private ListPanelController<T> GetController() {
            switch (PanelType) {
                case PanelType.Trees: {
                    switch (NetworkSkinPanelController.LanePosition) {
                        case LanePosition.Left: return NetworkSkinPanelController.LeftTree as ListPanelController<T>;
                        case LanePosition.Middle: return NetworkSkinPanelController.MiddleTree as ListPanelController<T>;
                        case LanePosition.Right: return NetworkSkinPanelController.RighTree as ListPanelController<T>;
                        default: return null;
                    }
                }
                case PanelType.Lights: return NetworkSkinPanelController.StreetLight as ListPanelController<T>;
                case PanelType.Pillars: {
                    switch (NetworkSkinPanelController.Pillar) {
                        case Pillar.Elevated: return NetworkSkinPanelController.ElevatedBridgePillar as ListPanelController<T>;
                        case Pillar.ElevatedMiddle: return NetworkSkinPanelController.ElevatedMiddlePillar as ListPanelController<T>;
                        case Pillar.Bridge: return NetworkSkinPanelController.BridgeBridgePillar as ListPanelController<T>;
                        case Pillar.BridgeMiddle: return NetworkSkinPanelController.BridgeMiddlePillar as ListPanelController<T>;
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
            FavouritesList.Clear();
            Blacklist.Clear();
            NormalList.Clear();
            int index = 0;
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            List<string> blacklist = Persistence.GetBlacklisted(UIUtil.PanelToItemType(PanelType));
            foreach (ListPanelController<T>.Item item in controller.Items) {
                if (item.Id == "#NONE#" || item.Id == "#DEFAULT#") {
                    ListItem listItem = CreateListItem(item);
                    if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                    fastList.RowsData.Add(listItem);
                    index++;
                    continue;
                } else if (favList.Contains(item.Id)) {
                    FavouritesList.Add(item);
                } else if (blacklist.Contains(item.Id) && !IsDefault(item.Id)) {
                    Blacklist.Add(item);
                } else NormalList.Add(item);
            }
            for (int i = 0; i < FavouritesList.Count; i++) {
                ListPanelController<T>.Item item = FavouritesList[i] as ListPanelController<T>.Item;
                ListItem listItem = CreateListItem(item);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < NormalList.Count; i++) {
                ListPanelController<T>.Item item = NormalList[i] as ListPanelController<T>.Item;
                ListItem listItem = CreateListItem(item);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            if (!Persistence.HideBlacklisted) {
                for (int i = 0; i < Blacklist.Count; i++) {
                    ListPanelController<T>.Item item = Blacklist[i] as ListPanelController<T>.Item;
                    ListItem listItem = CreateListItem(item);
                    if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                    fastList.RowsData.Add(listItem);
                    index++;
                }
            }
            fastList.DisplayAt(Persistence.DisplayAtSelected ? selectedIndex : -1);
            fastList.SelectedIndex = selectedIndex;
        }

        protected ListItem CreateListItem(ListPanelController<T>.Item item) {
            string id = item.Id;
            bool isFavourite = IsFavourite(id);
            bool isBlacklisted = IsBlacklisted(id);
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
                ? Sprites.NietIcon
                : item is ListPanelController<T>.SimpleItem si2
                ? si2.Value.GetThumbnail()
                : Sprites.DefaultIcon;

            ItemType type = UIUtil.PanelToItemType(PanelType);

            Color color = id == "#NONE#"
                ? default
                : item is ListPanelController<T>.SimpleItem si3
                ? si3.Value.GetLightColor()
                : default;

            return new ListItem(id, displayName, thumbnail, isFavourite, isBlacklisted, isDefault, type, color);
        }
    }
}
