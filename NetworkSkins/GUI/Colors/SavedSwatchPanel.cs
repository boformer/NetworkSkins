using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;
using static NetworkSkins.Persistence.PersistenceService;

namespace NetworkSkins.GUI.Colors
{
    public class SavedSwatchPanel : PanelBase
    {
        public delegate void SwatchClickedEventHandler(Color32 color);
        public event SwatchClickedEventHandler EventSwatchClicked;
        public delegate void RemoveSwatchEventHandler(SavedSwatchPanel savedSwatchPanel);
        public event RemoveSwatchEventHandler EventRemoveSwatch;
        public delegate void SwatchRenamedEventHandler(SavedSwatch savedSwatch);
        public event SwatchRenamedEventHandler EventSwatchRenamed;
        public SavedSwatch savedSwatch;
        private SwatchButton swatchButton;
        private UITextField textField;
        private UIButton deleteButton;

        public override void OnDestroy() {
            swatchButton.EventSwatchClicked -= OnSwatchClicked;
            textField.eventTextChanged -= OnTextChanged;
            deleteButton.eventClicked -= OnDeleteClicked;
            eventMouseEnter -= OnMouseEnter;
            eventMouseLeave -= OnMouseLeave;
            deleteButton.eventMouseEnter -= OnMouseEnter;
            deleteButton.eventMouseLeave -= OnMouseLeave;
            EventSwatchClicked = null;
            base.OnDestroy();
        }

        public void Build(PanelType panelType, Layout layout, SavedSwatch savedSwatch) {
            base.Build(panelType, layout);
            this.savedSwatch = savedSwatch;
            swatchButton = AddUIComponent<SwatchButton>();
            swatchButton.Build(savedSwatch.Color);
            textField = AddUIComponent<UITextField>();
            textField.normalBgSprite = "";
            textField.hoveredBgSprite = "TextFieldPanelHovered";
            textField.focusedBgSprite = "TextFieldPanel";
            textField.size = new Vector2(187.0f, 19.0f);
            textField.font = UIUtil.Font;
            textField.textScale = 0.8f;
            textField.horizontalAlignment = UIHorizontalAlignment.Left;
            textField.padding = new RectOffset(5, 0, 4, 1);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.selectionSprite = "EmptySprite";
            textField.selectOnFocus = true;
            textField.text = savedSwatch.Name;
            deleteButton = AddUIComponent<UIButton>();
            deleteButton.normalBgSprite = "";
            deleteButton.hoveredBgSprite = "DeleteLineButtonHover";
            deleteButton.pressedBgSprite = "DeleteLineButtonPressed";
            deleteButton.size = new Vector2(19.0f, 19.0f);
            swatchButton.EventSwatchClicked += OnSwatchClicked;
            textField.eventTextChanged += OnTextChanged;
            deleteButton.eventClicked += OnDeleteClicked;
            deleteButton.eventMouseEnter += OnMouseEnter;
            deleteButton.eventMouseLeave += OnMouseLeave;
            eventMouseEnter += OnMouseEnter;
            eventMouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(UIComponent component, UIMouseEventParameter eventParam) {
            deleteButton.normalBgSprite = "";
        }

        private void OnMouseEnter(UIComponent component, UIMouseEventParameter eventParam) {
            deleteButton.normalBgSprite = "DeleteLineButton";
        }

        private void OnDeleteClicked(UIComponent component, UIMouseEventParameter eventParam) {
            EventRemoveSwatch?.Invoke(this);
        }

        private void OnTextChanged(UIComponent component, string value) {
            savedSwatch.Name = value;
            EventSwatchRenamed?.Invoke(savedSwatch);
        }

        private void OnSwatchClicked(Color32 color, UIMouseEventParameter eventParam, UIComponent component) {
            EventSwatchClicked?.Invoke(color);
        }
    }
}
