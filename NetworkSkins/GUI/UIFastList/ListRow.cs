using System;
using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.UIFastList
{
    public class ListRow : PanelBase, IUIFastListRow
    {
        //public delegate void SelectedChangedEventHandler(string itemID, bool selected);
        //public event SelectedChangedEventHandler EventSelectedChanged;

        public delegate void FavouriteChangedEventHandler(string itemID, bool favourite);
        public event FavouriteChangedEventHandler EventFavouriteChanged;

        public delegate void BlacklistedChangedEventHandler(string itemID, bool blacklisted);
        public event BlacklistedChangedEventHandler EventBlacklistedChanged;

        private static readonly string[] enumNames = Enum.GetNames(typeof(Surface));

        private UIPanel thumbnailPanel;
        private UITextureSprite thumbnailSprite;
        private UILabel nameLabel;
        private UIPanel checkboxPanel;
        private UIPanel lightColorPanel;
        private UICheckBox favouriteCheckbox;
        private UISprite checkedSprite;
        private UISprite uncheckedSprite;
        private ListItem itemData;
        private Color32 thumbnailBackgroundColor = new Color32(131, 141, 145, 255);
        private Color32 evenColor = new Color32(67, 76, 80, 255); 
        private Color32 oddColor = new Color32(57, 67, 70, 255);
        private Color32 selectedColor = FocusedColor;
        private bool isRowOdd;

        public override void Awake() {
            base.Awake();
            Build(PanelType.None, new Layout(new Vector2(380.0f, 50.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
        }

        public override void OnDestroy() {
            if (favouriteCheckbox != null) {
                favouriteCheckbox.eventClicked -= OnFavouriteCheckboxMouseUp;
            }
            eventMouseEnter -= OnMouseEnterEvent;
            eventMouseLeave -= OnMouseLeaveEvent;
            EventFavouriteChanged = null;
            //EventSelectedChanged = null;
            EventBlacklistedChanged = null;
            base.OnDestroy();
        }

        public void Select(bool isRowOdd) {
            color = selectedColor;
            //EventSelectedChanged?.Invoke(itemData.ID, true);
        }

        public void Deselect(bool isRowOdd) {
            color = isRowOdd ? oddColor : evenColor;
            //EventSelectedChanged?.Invoke(itemData.ID, false);
        }

        public void Display(object data, bool isRowOdd) {
            if (data is ListItem item) {
                itemData = item;
                this.isRowOdd = isRowOdd;
                DisplayItem(isRowOdd);
            }
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            backgroundSprite = "WhiteRect";
            CreateThumbnail();
            CreateLabel();
            CreateLightColorPanel();
            CreateCheckbox();
            UIUtil.CreateSpace(5.0f, 30.0f, this);
            eventMouseEnter += OnMouseEnterEvent;
            eventMouseLeave += OnMouseLeaveEvent;
        }

        protected override void RefreshUI(NetInfo netInfo) {
            if (itemData != null) {
                color = NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type) ? selectedColor : isRowOdd ? oddColor : evenColor;
            }
        }

        private void CreateThumbnail() {
            thumbnailPanel = AddUIComponent<UIPanel>();
            thumbnailPanel.size = new Vector2(33.0f, 30.0f);
            thumbnailPanel.backgroundSprite = "WhiteRect";
            thumbnailPanel.color = thumbnailBackgroundColor;
            thumbnailPanel.atlas = NetworkSkinsMod.defaultAtlas;
            thumbnailSprite = thumbnailPanel.AddUIComponent<UITextureSprite>();
            thumbnailSprite.size = new Vector2(31.0f, 28.0f);
            thumbnailSprite.relativePosition = new Vector2(1.0f, 1.0f);
        }

        private void CreateLabel() {
            nameLabel = AddUIComponent<UILabel>();
            nameLabel.autoSize = false;
            nameLabel.size = new Vector2(255.0f, 30.0f);
            nameLabel.padding = new RectOffset(0, 0, 8, 0);
            nameLabel.textScale = 0.8f;
            nameLabel.font = UIUtil.Font;
        }

        private void CreateLightColorPanel() {
            UIPanel lightPanel = AddUIComponent<UIPanel>();
            lightPanel.size = new Vector2(12.0f, 30.0f);
            lightColorPanel = lightPanel.AddUIComponent<UIPanel>();
            lightColorPanel.size = new Vector2(12.0f, 12.0f);
            lightColorPanel.atlas = Resources.DefaultAtlas;
            lightColorPanel.backgroundSprite = "PieChartWhiteFg";
            lightColorPanel.relativePosition = new Vector2(0.0f, 9.0f);
        }

        private void CreateCheckbox() {
            checkboxPanel = AddUIComponent<UIPanel>();
            checkboxPanel.size = new Vector2(22.0f, 30.0f);
            favouriteCheckbox = checkboxPanel.AddUIComponent<UICheckBox>();
            favouriteCheckbox.size = new Vector2(22f, 22f);
            favouriteCheckbox.relativePosition = new Vector3(0.0f, 4.0f);
            uncheckedSprite = favouriteCheckbox.AddUIComponent<UISprite>();
            uncheckedSprite.atlas = Resources.Atlas;
            uncheckedSprite.spriteName = Resources.StarOutline;
            uncheckedSprite.size = favouriteCheckbox.size;
            uncheckedSprite.relativePosition = Vector3.zero;
            checkedSprite = uncheckedSprite.AddUIComponent<UISprite>();
            checkedSprite.atlas = Resources.Atlas;
            checkedSprite.spriteName = Resources.Star;
            checkedSprite.size = favouriteCheckbox.size;
            checkedSprite.relativePosition = Vector2.zero;
            favouriteCheckbox.checkedBoxObject = checkedSprite;
            favouriteCheckbox.eventMouseUp += OnFavouriteCheckboxMouseUp;
        }

        private void OnFavouriteCheckboxMouseUp(UIComponent component, UIMouseEventParameter eventParam) {
            if (eventParam.buttons == UIMouseButton.Right) {
                if (itemData.IsDefault) return;
                bool blackListed = !itemData.IsBlacklisted;
                itemData.IsBlacklisted = blackListed;
                if (blackListed) {
                    favouriteCheckbox.isChecked = true;
                    checkedSprite.spriteName = Resources.Blacklisted;
                    uncheckedSprite.spriteName = "";
                    if (itemData.IsFavourite) {
                        itemData.IsFavourite = false;
                        EventFavouriteChanged?.Invoke(itemData.ID, false);
                    }
                } else {
                    if (!itemData.IsFavourite) {
                        favouriteCheckbox.isChecked = false;
                    }
                    uncheckedSprite.spriteName = Resources.StarOutline;
                }
                EventBlacklistedChanged?.Invoke(itemData.ID, blackListed);
            } else if (eventParam.buttons == UIMouseButton.Left) {
                bool favourite = !itemData.IsFavourite;
                itemData.IsFavourite = favourite;
                if (favourite) {
                    favouriteCheckbox.isChecked = true;
                    checkedSprite.spriteName = Resources.Star;
                    uncheckedSprite.spriteName = Resources.StarOutline;
                    if (itemData.IsBlacklisted) {
                        itemData.IsBlacklisted = false;
                        EventBlacklistedChanged?.Invoke(itemData.ID, false);
                    }
                } else {
                    if (!itemData.IsBlacklisted) {
                        favouriteCheckbox.isChecked = false;
                    }
                }
                EventFavouriteChanged?.Invoke(itemData.ID, favourite);
            }
            UpdateCheckboxTooltip();
        }

        private void DisplayItem(bool isRowOdd) {
            color = NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type) ? selectedColor : isRowOdd ? oddColor : evenColor;
            thumbnailSprite.texture = itemData.Thumbnail;
            nameLabel.text = itemData.DisplayName;
            lightColorPanel.color = itemData.LightColor;
            favouriteCheckbox.isChecked = itemData.IsFavourite || IsBlacklisted();
            checkedSprite.spriteName = IsBlacklisted() ? Resources.Blacklisted : Resources.Star;
            uncheckedSprite.spriteName = IsBlacklisted() ? "" :  Resources.StarOutline;
            favouriteCheckbox.isVisible = true;
            for (int i = 0; i < enumNames.Length; i++) {
                if (itemData.ID == enumNames[i]) {
                    favouriteCheckbox.isVisible = false;
                }
            }
            if (itemData.ID == "#DEFAULT#" || itemData.ID == "#NONE#") favouriteCheckbox.isVisible = false;
            UpdateCheckboxTooltip();
        }

        private bool IsBlacklisted() {
            return itemData.IsBlacklisted && !itemData.IsDefault;
        }

        private void UpdateCheckboxTooltip() {
            favouriteCheckbox.tooltip = itemData.IsFavourite
                            ? Translation.Instance.GetTranslation(TranslationID.TOOLTIP_REMOVEFAVOURITE)
                            : itemData.IsBlacklisted
                            ? Translation.Instance.GetTranslation(TranslationID.TOOLTIP_REMOVEBLACKLIST)
                            : Translation.Instance.GetTranslation(TranslationID.TOOLTIP_ADDFAVOURITE_ADDBLACKLIST);
            favouriteCheckbox.RefreshTooltip();
        }

        private void OnMouseLeaveEvent(UIComponent component, UIMouseEventParameter eventParam) {
            if (itemData != null) {
                color = NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type) ? selectedColor : isRowOdd ? oddColor : evenColor;
            }
        }

        private void OnMouseEnterEvent(UIComponent component, UIMouseEventParameter eventParam) {
            if (itemData != null) {
                if (!NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type)) color = new Color32((byte)(oddColor.r + 25), (byte)(oddColor.g + 25), (byte)(oddColor.b + 25), 255);
            }
        }
    }
}
