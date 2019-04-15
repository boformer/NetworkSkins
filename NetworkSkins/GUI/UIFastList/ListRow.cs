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
        public delegate void SelectedChangedEventHandler(string itemID, bool selected);
        public event SelectedChangedEventHandler EventSelectedChanged;

        public delegate void FavouriteChangedEventHandler(string itemID, bool favourite);
        public event FavouriteChangedEventHandler EventFavouriteChanged;

        private static string[] enumNames = Enum.GetNames(typeof(Surface));

        private UIPanel thumbnailPanel;
        private UITextureSprite thumbnailSprite;
        private UILabel nameLabel;
        private UIPanel checkboxPanel;
        private UICheckBox favouriteCheckbox;
        private ListItem itemData;
        private Color32 thumbnailBackgroundColor = new Color32(131, 141, 145, 255);
        private Color32 evenColor = new Color32(67, 76, 80, 255); 
        private Color32 oddColor = new Color32(57, 67, 70, 255);
        private Color32 hoverColor = new Color32(131, 141, 145, 255);
        private Color32 selectedColor = FocusedColor;
        private bool isRowOdd;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            backgroundSprite = "WhiteRect";
            CreateThumbnail();
            CreateLabel();
            UIUtil.CreateSpace(5.0f, 30.0f, this);
            CreateCheckbox();
            UIUtil.CreateSpace(5.0f, 30.0f, this);
            eventMouseEnter += OnMouseEnterEvent;
            eventMouseLeave += OnMouseLeaveEvent;
        }

        private void CreateThumbnail() {
            thumbnailPanel = AddUIComponent<UIPanel>();
            thumbnailPanel.size = new Vector2(33.0f, 30.0f);
            thumbnailPanel.backgroundSprite = "WhiteRect";
            thumbnailPanel.color = thumbnailBackgroundColor;
            thumbnailSprite = thumbnailPanel.AddUIComponent<UITextureSprite>();
            thumbnailSprite.size = new Vector2(31.0f, 28.0f);
            thumbnailSprite.relativePosition = new Vector2(1.0f, 1.0f);
        }

        private void CreateLabel() {
            nameLabel = AddUIComponent<UILabel>();
            nameLabel.autoSize = false;
            nameLabel.size = new Vector2(253.0f, 30.0f);
            nameLabel.padding = new RectOffset(0, 0, 8, 0);
            nameLabel.font = UIUtil.Font;
        }

        private void CreateCheckbox() {
            checkboxPanel = AddUIComponent<UIPanel>();
            checkboxPanel.size = new Vector2(32.0f, 30.0f);
            favouriteCheckbox = checkboxPanel.AddUIComponent<UICheckBox>();
            favouriteCheckbox.size = new Vector2(22f, 22f);
            favouriteCheckbox.relativePosition = new Vector3(9.0f, 4.0f);
            UISprite uncheckedSprite = favouriteCheckbox.AddUIComponent<UISprite>();
            uncheckedSprite.atlas = Resources.Atlas;
            uncheckedSprite.spriteName = Resources.StarOutline;
            uncheckedSprite.size = favouriteCheckbox.size;
            uncheckedSprite.relativePosition = Vector3.zero;
            UISprite checkedSprite = uncheckedSprite.AddUIComponent<UISprite>();
            checkedSprite.atlas = Resources.Atlas;
            checkedSprite.spriteName = Resources.Star;
            checkedSprite.size = favouriteCheckbox.size;
            checkedSprite.relativePosition = Vector2.zero;
            favouriteCheckbox.checkedBoxObject = checkedSprite;
            favouriteCheckbox.eventCheckChanged += OnFavouriteCheckboxCheckChanged;
        }

        public override void Awake() {
            base.Awake();
            Build(PanelType.None, new Layout(new Vector2(380.0f, 50.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
        }

        public override void OnDestroy() {
            base.OnDestroy();
            if (favouriteCheckbox != null)
                favouriteCheckbox.eventCheckChanged -= OnFavouriteCheckboxCheckChanged;
            eventMouseEnter -= OnMouseEnterEvent;
            eventMouseLeave -= OnMouseLeaveEvent;
        }

        public void Select(bool isRowOdd) {
            color = selectedColor;
            EventSelectedChanged?.Invoke(itemData.ID, true);
        }

        public void Deselect(bool isRowOdd) {
            color = isRowOdd ? oddColor : evenColor;
            EventSelectedChanged?.Invoke(itemData.ID, false);
        }

        public void Display(object data, bool isRowOdd) {
            if (data is ListItem item) {
                itemData = item;
                this.isRowOdd = isRowOdd;
                DisplayItem(isRowOdd);
            }
        }

        private void DisplayItem(bool isRowOdd) {
            color = NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type) ? selectedColor : isRowOdd ? oddColor : evenColor;
            thumbnailSprite.texture = itemData.Thumbnail;
            nameLabel.text = itemData.DisplayName;
            favouriteCheckbox.isChecked = itemData.IsFavourite;
            favouriteCheckbox.isVisible = true;
            for (int i = 0; i < enumNames.Length; i++) {
                if (itemData.ID == enumNames[i]) {
                    favouriteCheckbox.isVisible = false;
                }
            }
            if (itemData.ID == "#DEFAULT#" || itemData.ID == "#NONE#") favouriteCheckbox.isVisible = false;
            UpdateCheckboxTooltip();
        }

        private void UpdateCheckboxTooltip() {
            favouriteCheckbox.tooltip = itemData.IsFavourite
                            ? Translation.Instance.GetTranslation(TranslationID.TOOLTIP_REMOVEFAVOURITE)
                            : Translation.Instance.GetTranslation(TranslationID.TOOLTIP_ADDFAVOURITE);
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

        private void OnFavouriteCheckboxCheckChanged(UIComponent component, bool value) {
            itemData.IsFavourite = value;
            UpdateCheckboxTooltip();
            EventFavouriteChanged?.Invoke(itemData.ID, value);
        }

        protected override void RefreshUI(NetInfo netInfo) {
            if (itemData != null) {
                color = NetworkSkinPanelController.IsSelected(itemData.ID, itemData.Type) ? selectedColor : isRowOdd ? oddColor : evenColor;
            }
        }
    }
}
