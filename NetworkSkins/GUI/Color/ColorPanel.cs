using System;
using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ColorPanel : PanelBase
    {
        private RGBPanel rgbPanel;
        private UIPanel colorPanel;
        private SwatchesPanel swatchesPanel1;
        private SwatchesPanel swatchesPanel2;
        private UIColorPicker colorPicker;
        private UILabel redLabel;
        private UILabel greenLabel;
        private UILabel blueLabel;
        private UITextField redTextField;
        private UITextField greenTextField;
        private UITextField blueTextField;
        private Swatches swatches;
        private Color32 currentColor;
        private bool updateNeeded;

        public override void Update() {
            base.Update();
            if (updateNeeded && Input.GetMouseButtonUp(0)) {
                ColorChanged();
                updateNeeded = false;
            }
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            CreateColorPicker();
            CreateRGBPanel();
            CreateSwatchesPanel();
            currentColor = SkinController.Color.SelectedColor;
        }

        private void CreateRGBPanel() {
            rgbPanel = AddUIComponent<RGBPanel>();
            rgbPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 35.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));

            colorPanel = rgbPanel.AddUIComponent<UIPanel>();
            colorPanel.backgroundSprite = "WhiteRect";
            colorPanel.size = new Vector2(35.0f, 25.0f);
            colorPanel.color = SkinController.Color.SelectedColor;

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
            label.padding = new RectOffset();
            return label;
        }

        private UITextField CreateTextfield(string text) {
            UITextField textField = rgbPanel.AddUIComponent<UITextField>();
            textField.size = new Vector2(45.0f, 25.0f);
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

        internal void OnSwatchClicked(Color32 color, UIMouseEventParameter eventParam, UIComponent component) {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) {
                Persistence.RemoveSwatch(color);
                Destroy(component.gameObject);
            } else {
                this.currentColor = color;
                SkinController.Color.SetColor(color);
            }
        }

        private void ColorChanged() {
            swatches.AddSwatch(this, swatches.Count < 10 ? swatchesPanel1 : swatchesPanel2, currentColor);
            SkinController.Color.SetColor(color);
        }

        private void CreateSwatchesPanel() {
            swatchesPanel1 = AddUIComponent<SwatchesPanel>();
            swatchesPanel1.Build(PanelType.None, new Layout(new Vector2(0.0f, 25.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            swatchesPanel1.autoFitChildrenHorizontally = true;
            swatchesPanel2 = AddUIComponent<SwatchesPanel>();
            swatchesPanel2.Build(PanelType.None, new Layout(new Vector2(0.0f, 30.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            swatchesPanel2.autoFitChildrenHorizontally = true;
            swatches = new Swatches(20, Persistence);
            int count = 0;
            foreach (Color32 swatch in Persistence.GetSwatches()) {
                swatches.AddSwatch(this, count < 10 ? swatchesPanel1 : swatchesPanel2, swatch);
                count++;
            }
            UIUtil.CreateSpace(0.1f, 30.0f, swatchesPanel2);
        }

        private void CreateColorPicker() {
            UIColorField field = UITemplateManager.Get<UIPanel>("LineTemplate").Find<UIColorField>("LineColor");
            field = GameObject.Instantiate<UIColorField>(field);
            UIColorPicker picker = GameObject.Instantiate<UIColorPicker>(field.colorPicker);
            picker.eventColorUpdated += OnColorUpdated;
            picker.color = SkinController.Color.SelectedColor;
            picker.component.color = GUIColor;
            UIPanel pickerPanel = picker.component as UIPanel;
            pickerPanel.backgroundSprite = "";
            picker.component.size = new Vector2(254f, 217f);
            //picker.m_HSBField.size = new Vector2(180.0f, 180.0f);
            //picker.m_HueSlider.size = new Vector2(16.2f, 180.0f);
            //picker.m_HueSlider.relativePosition = new Vector2(201.0f, picker.m_HueSlider.relativePosition.y);
            //picker.m_Indicator.size = new Vector2(14.4f, 14.4f);
            AttachUIComponent(picker.gameObject);
            colorPicker = picker;
        }

        private void OnColorUpdated(Color value) {
            currentColor = value;
            if(colorPanel != null) colorPanel.color = value;
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
