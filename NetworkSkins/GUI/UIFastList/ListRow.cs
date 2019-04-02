using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ListRow : PanelBase, IUIFastListRow
    {
        public delegate void SelectedChangedEventHandler(string itemID, bool selected);
        public event SelectedChangedEventHandler EventSelectedChanged;

        public delegate void FavouriteChangedEventHandler(string itemID, bool favourite);
        public event FavouriteChangedEventHandler EventFavouriteChanged;

        private static string[] enumNames = Enum.GetNames(typeof(NetworkGroundType));

        private UIPanel thumbnailPanel;
        private UITextureSprite thumbnailSprite;
        private UILabel nameLabel;
        private UIPanel checkboxPanel;
        private UICheckBox favouriteCheckbox;
        private ListItem itemData;
        private Color thumbnailBackgroundColor = new Color32(131, 141, 145, 255);
        private Color evenColor = new Color32(67, 76, 80, 255); 
        private Color oddColor = new Color32(57, 67, 70, 255);
        private Color hoverColor = new Color32(131, 141, 145, 255);
        private Color selectedColor = new Color32(88, 181, 205, 255);
        private bool isRowOdd;

        public override void Build(Layout layout) {
            base.Build(layout);
            backgroundSprite = "WhiteRect";
            CreateThumbnail();
            CreateLabel();
            CreateSpace(5.0f, 30.0f);
            CreateCheckbox();
            CreateSpace(5.0f, 30.0f);
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
        }

        private void CreateCheckbox() {
            checkboxPanel = AddUIComponent<UIPanel>();
            checkboxPanel.size = new Vector2(32.0f, 30.0f);
            favouriteCheckbox = checkboxPanel.AddUIComponent<UICheckBox>();
            favouriteCheckbox.size = new Vector2(32f, 22f);
            favouriteCheckbox.relativePosition = new Vector3(0.0f, 4.0f);
            UISprite uncheckedSprite = favouriteCheckbox.AddUIComponent<UISprite>();
            uncheckedSprite.spriteName = "UpgradeIconGrey";
            uncheckedSprite.size = favouriteCheckbox.size;
            uncheckedSprite.relativePosition = Vector3.zero;
            UISprite checkedSprite = uncheckedSprite.AddUIComponent<UISprite>();
            checkedSprite.spriteName = "UpgradeIcon";
            checkedSprite.size = favouriteCheckbox.size;
            checkedSprite.relativePosition = Vector2.zero;
            favouriteCheckbox.checkedBoxObject = checkedSprite;
            favouriteCheckbox.eventCheckChanged += OnFavouriteCheckboxCheckChanged;
        }

        public override void Awake() {
            base.Awake();
            Build(new Layout(new Vector2(380.0f, 50.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
        }

        public override void OnDestroy() {
            base.OnDestroy();
            if (favouriteCheckbox != null)
                favouriteCheckbox.eventCheckChanged -= OnFavouriteCheckboxCheckChanged;
            eventMouseEnter -= OnMouseEnterEvent;
            eventMouseLeave -= OnMouseLeaveEvent;
        }

        public void Select(bool isRowOdd) {
            itemData.IsSelected = true;
            color = selectedColor;
            EventSelectedChanged?.Invoke(itemData.ID, true);
        }

        public void Deselect(bool isRowOdd) {
            itemData.IsSelected = false;
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
            color = itemData.IsSelected ? selectedColor : isRowOdd ? oddColor : evenColor;
            thumbnailSprite.texture = itemData.Thumbnail;
            nameLabel.text = itemData.DisplayName;
            favouriteCheckbox.isChecked = itemData.IsFavourite;
            favouriteCheckbox.isVisible = true;
            for (int i = 0; i < enumNames.Length; i++) {
                if (itemData.ID == enumNames[i]) {
                    favouriteCheckbox.isVisible = false;
                }
            }
            UpdateCheckboxTooltip();
        }

        private void UpdateCheckboxTooltip() {
            favouriteCheckbox.tooltip = itemData.IsFavourite
                            ? Translation.Instance.GetTranslation(TranslationID.TOOLTIP_REMOVEFAVOURITE)
                            : Translation.Instance.GetTranslation(TranslationID.TOOLTIP_ADDFAVOURITE);
            favouriteCheckbox.RefreshTooltip();
        }

        private void OnMouseLeaveEvent(UIComponent component, UIMouseEventParameter eventParam) {
            color = itemData.IsSelected ? selectedColor : isRowOdd ? oddColor :  evenColor;
        }

        private void OnMouseEnterEvent(UIComponent component, UIMouseEventParameter eventParam) {
            if (!itemData.IsSelected) color = hoverColor;
        }

        private void OnFavouriteCheckboxCheckChanged(UIComponent component, bool value) {
            itemData.IsFavourite = value;
            UpdateCheckboxTooltip();
            EventFavouriteChanged?.Invoke(itemData.ID, value);
        }

        protected override void RefreshUI(NetInfo netInfo) {
            throw new System.NotImplementedException();
        }
    }
}
