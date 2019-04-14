using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.Colors
{
    public class ColorPanel : PanelBase
    {
        private RGBPanel rgbPanel;
        private UIPanel colorPanel;
        private SwatchesPanel swatchesPanel;
        private UIColorPicker colorPicker;
        private UILabel redLabel;
        private UILabel greenLabel;
        private UILabel blueLabel;
        private UITextField redTextField;
        private UITextField greenTextField;
        private UITextField blueTextField;
        private Color32 currentColor;
        private bool updateNeeded;

        public override void Update() {
            base.Update();
            if (updateNeeded && Input.GetMouseButtonUp(0)) {
                ColorChanged();
                updateNeeded = false;
            }
        }

        public override void OnDestroy() {
            NetworkSkinPanelController.Color.EventColorUsedInSegment -= OnColorUsed;
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            CreateColorPicker();
            CreateRGBPanel();
            RefreshSwatchesPanel();
            NetworkSkinPanelController.Color.EventColorUsedInSegment += OnColorUsed;
            RefreshColors();
        }

        private void CreateColorPicker() {
            UIColorField field = UITemplateManager.Get<UIPanel>("LineTemplate").Find<UIColorField>("LineColor");
            field = GameObject.Instantiate<UIColorField>(field);
            UIColorPicker picker = GameObject.Instantiate<UIColorPicker>(field.colorPicker);
            picker.eventColorUpdated += OnColorUpdated;
            picker.color = NetworkSkinPanelController.Color.SelectedColor;
            picker.component.color = GUIColor;
            UIPanel pickerPanel = picker.component as UIPanel;
            pickerPanel.backgroundSprite = "";
            picker.component.size = new Vector2(254f, 217f);
            AttachUIComponent(picker.gameObject);
            colorPicker = picker;
        }

        private void CreateRGBPanel() {
            rgbPanel = AddUIComponent<RGBPanel>();
            rgbPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 35.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            rgbPanel.padding = new RectOffset(10, 0, 5, 0);

            colorPanel = rgbPanel.AddUIComponent<UIPanel>();
            colorPanel.backgroundSprite = "WhiteRect";
            colorPanel.size = new Vector2(25.0f, 25.0f);
            colorPanel.color = NetworkSkinPanelController.Color.SelectedColor;

            Color32 color32 = colorPicker.color;
            redLabel = CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_RED));
            redTextField = CreateTextfield(color32.r.ToString());
            greenLabel = CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_GREEN));
            greenTextField = CreateTextfield(color32.g.ToString());
            blueLabel = CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_BLUE));
            blueTextField = CreateTextfield(color32.b.ToString());
        }

        private UILabel CreateLabel(string text) {
            UILabel label = rgbPanel.AddUIComponent<UILabel>();
            label.font = UIUtil.Font;
            label.textColor = UIUtil.TextColor;
            label.text = text;
            label.autoSize = false;
            label.autoHeight = false;
            label.size = new Vector2(15.0f, 25.0f);
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.textAlignment = UIHorizontalAlignment.Right;
            label.padding = new RectOffset(0, 0, 5, 0);
            return label;
        }

        private UITextField CreateTextfield(string text) {
            UITextField textField = rgbPanel.AddUIComponent<UITextField>();
            textField.size = new Vector2(44.0f, 25.0f);
            textField.padding = new RectOffset(6, 6, 6, 6);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanelHovered";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(80, 80, 80, 128);
            textField.color = new Color32(255, 255, 255, 255);
            textField.eventGotFocus += OnGotFocus;
            textField.eventLostFocus += OnLostFocus;
            textField.eventKeyPress += OnKeyPress;
            textField.eventTextChanged += OnTextChanged;
            textField.eventTextSubmitted += OnTextSubmitted;
            textField.text = text;
            return textField;
        }

        private void RefreshSwatchesPanel() {
            if (swatchesPanel != null) Destroy(swatchesPanel.gameObject);
            swatchesPanel = AddUIComponent<SwatchesPanel>();
            swatchesPanel.Build(PanelType.None, new Layout(new Vector2(254.0f, 30.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 4));
            swatchesPanel.padding = new RectOffset(11, 0, 5, 0);
            foreach (var swatch in NetworkSkinPanelController.Color.Swatches) {
                AddSwatch(swatch);
            }
        }

        private void AddSwatch(Color32 color) {
            SwatchButton button = swatchesPanel.AddUIComponent<SwatchButton>();
            button.size = new Vector2(15.5f, 15.0f);
            button.atlas = Resources.Atlas;
            button.normalBgSprite = Resources.Swatch;
            button.hoveredColor = new Color32((byte)Mathf.Min((color.r + 32), 255), (byte)Mathf.Min((color.g + 32), 255), (byte)Mathf.Min((color.b + 32), 255), 255);
            button.pressedColor = new Color32((byte)Mathf.Min((color.r + 64), 255), (byte)Mathf.Min((color.g + 64), 255), (byte)Mathf.Min((color.b + 64), 255), 255);
            button.focusedColor = color;
            button.color = color;
            button.swatch = color;
            button.EventSwatchClicked += OnSwatchClicked;
        }

        private void OnLostFocus(UIComponent component, UIFocusEventParameter eventParam) {
            UITextField textField = component as UITextField;
        }

        private void OnGotFocus(UIComponent component, UIFocusEventParameter eventParam) {
            UITextField textField = component as UITextField;
            textField.SelectAll();
        }

        private void OnTextChanged(UIComponent component, string value) {
            UITextField textField = component as UITextField;
            textField.eventTextChanged -= OnTextChanged;
            textField.text = GetClampedString(value);
            textField.eventTextChanged += OnTextChanged;
        }

        private string GetClampedString(string value) {
            return GetClampedFloat(value).ToString();
        }

        private float GetClampedFloat(string value) {
            if (!float.TryParse(value, out float number)) {
                return 0.0f;
            }
            return Mathf.Clamp(number, 0, 256);
        }

        private void OnKeyPress(UIComponent component, UIKeyEventParameter parameter) {
            char ch = parameter.character;
            if (!char.IsControl(ch) && !char.IsDigit(ch)) {
                parameter.Use();
            }
        }

        private void OnTextSubmitted(UIComponent component, string value) {
            UITextField textField = component as UITextField;
            Color32 color32 = colorPicker.color;
            if (component == redTextField) {
                colorPicker.color = new Color32((byte)GetClampedFloat(value), color32.g, color32.b, 255);
            } else if (component == greenTextField) {
                colorPicker.color = new Color32(color32.r, (byte)GetClampedFloat(value), color32.b, 255);
            } else if (component == blueTextField) {
                colorPicker.color = new Color32(color32.r, color32.g, (byte)GetClampedFloat(value), 255);
            }
        }

        private void OnSwatchClicked(Color32 color, UIMouseEventParameter eventParam, UIComponent component) {
            colorPicker.color = color;
        }

        private void OnColorUsed() {
            RefreshSwatchesPanel();
        }

        private void ColorChanged() {
            NetworkSkinPanelController.Color.SetColor(currentColor);
        }

        private void OnColorUpdated(UnityEngine.Color value) {
            currentColor = value;
            if (colorPanel != null) colorPanel.color = value;
            if (redTextField != null) {
                redTextField.eventTextChanged -= OnTextChanged;
                redTextField.text = currentColor.r.ToString();
                redTextField.eventTextChanged += OnTextChanged;
            }
            if (greenTextField != null) {
                greenTextField.eventTextChanged -= OnTextChanged;
                greenTextField.text = currentColor.g.ToString();
                greenTextField.eventTextChanged += OnTextChanged;
            }
            if (blueTextField != null) {
                blueTextField.eventTextChanged -= OnTextChanged;
                blueTextField.text = currentColor.b.ToString();
                blueTextField.eventTextChanged += OnTextChanged;
            }
            updateNeeded = true;
        }

        protected override void RefreshUI(NetInfo netInfo) {
            RefreshColors();
        }

        private void RefreshColors() {
            colorPicker.eventColorUpdated -= OnColorUpdated;
            currentColor = colorPicker.color = NetworkSkinPanelController.Color.SelectedColor;
            colorPicker.eventColorUpdated += OnColorUpdated;
        }

        public class RGBPanel : PanelBase
        {
            protected override void RefreshUI(NetInfo netInfo) {
            }
        }

        public class SwatchesPanel : PanelBase
        {
            protected override void RefreshUI(NetInfo netInfo) {

            }
        }
    }
}
