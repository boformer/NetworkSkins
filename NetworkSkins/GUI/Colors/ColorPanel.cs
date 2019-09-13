using System.Collections.Generic;
using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;
using static NetworkSkins.Persistence.PersistenceService;

namespace NetworkSkins.GUI.Colors
{
    public class ColorPanel : PanelBase
    {
        private PanelBase rgbPanel;
        private UIPanel colorPanel;
        private PanelBase swatchesPanel;
        private UIColorPicker colorPicker; // nullable
        private UITextField redTextField;
        private UITextField greenTextField;
        private UITextField blueTextField;
        private PanelBase buttonsPanel;
        private ButtonPanel resetButton;
        private ButtonPanel saveButton;
        private PanelBase savedSwatchesPanel;
        private Color32 currentColor;
        private bool updateNeeded;
        private List<SavedSwatch> savedSwatches;
        private const int MAX_SAVED_SWATCHES = 10;
 
        public override void Update() {
            base.Update();
            if (updateNeeded && Input.GetMouseButtonUp(0)) {
                ColorChanged();
                updateNeeded = false;
            }
            if (savedSwatches.Count == MAX_SAVED_SWATCHES || savedSwatches.Find(s => s.Color == NetworkSkinPanelController.Color.SelectedColor) != null) {
                if (saveButton.isEnabled) saveButton.Disable();
                if (savedSwatches.Count == MAX_SAVED_SWATCHES) {
                    saveButton.tooltip = Translation.Instance.GetTranslation(TranslationID.TOOLTIP_BUTTON_SAVE_MAXREACHED);
                } else if (savedSwatches.Find(s => s.Color == NetworkSkinPanelController.Color.SelectedColor) != null) {
                    saveButton.tooltip = Translation.Instance.GetTranslation(TranslationID.TOOLTIP_BUTTON_SAVE_COLOREXISTS);
                }
            } else if (savedSwatches.Count < MAX_SAVED_SWATCHES) {
                if (!saveButton.isEnabled) saveButton.Enable();
                saveButton.tooltip = Translation.Instance.GetTranslation(TranslationID.TOOLTIP_BUTTON_SAVE);
            }
        }

        public override void OnDestroy() {
            redTextField.eventGotFocus -= OnGotFocus;
            redTextField.eventKeyPress -= OnKeyPress;
            redTextField.eventTextChanged -= OnTextChanged;
            redTextField.eventTextSubmitted -= OnTextSubmitted;
            greenTextField.eventGotFocus -= OnGotFocus;
            greenTextField.eventKeyPress -= OnKeyPress;
            greenTextField.eventTextChanged -= OnTextChanged;
            greenTextField.eventTextSubmitted -= OnTextSubmitted;
            blueTextField.eventGotFocus -= OnGotFocus;
            blueTextField.eventKeyPress -= OnKeyPress;
            blueTextField.eventTextChanged -= OnTextChanged;
            blueTextField.eventTextSubmitted -= OnTextSubmitted;
            if(colorPicker != null) colorPicker.eventColorUpdated -= OnColorUpdated;
            NetworkSkinPanelController.Color.EventColorUsedInSegment -= OnColorUsed;
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            savedSwatches = Persistence.GetSavedSwatches();
            if(NetworkSkinsMod.InGame) {
                CreateColorPicker();
            }
            CreateRGBPanel();
            CreateButtonsPanel();
            RefreshSwatchesPanel();
            RefreshSavedSwatchesPanel();
            UIUtil.CreateSpace(255.0f, 11.0f, this);
            NetworkSkinPanelController.Color.EventColorUsedInSegment += OnColorUsed;
            RefreshColors();
            color = GUIColor;
            padding = new RectOffset(1, 0, 0, 0);
            autoFitChildrenHorizontally = true;
        }

        private void CreateColorPicker() {
            UIColorField field = UITemplateManager.Get<UIPanel>("LineTemplate").Find<UIColorField>("LineColor");
            field = Instantiate<UIColorField>(field);
            UIColorPicker picker = Instantiate<UIColorPicker>(field.colorPicker);
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
            rgbPanel = AddUIComponent<PanelBase>();
            rgbPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 35.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            rgbPanel.padding = new RectOffset(10, 0, 5, 0);

            colorPanel = rgbPanel.AddUIComponent<UIPanel>();
            colorPanel.backgroundSprite = "WhiteRect";
            colorPanel.size = new Vector2(28.0f, 25.0f);
            colorPanel.color = NetworkSkinPanelController.Color.SelectedColor;
            colorPanel.atlas = NetworkSkinsMod.defaultAtlas;

            Color32 color32 = colorPanel.color;
            CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_RED));
            redTextField = CreateTextfield(color32.r.ToString());
            CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_GREEN));
            greenTextField = CreateTextfield(color32.g.ToString());
            CreateLabel(Translation.Instance.GetTranslation(TranslationID.LABEL_BLUE));
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
            label.atlas = NetworkSkinsMod.defaultAtlas;
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
            textField.eventKeyPress += OnKeyPress;
            textField.eventTextChanged += OnTextChanged;
            textField.eventTextSubmitted += OnTextSubmitted;
            textField.text = text;
            textField.atlas = NetworkSkinsMod.defaultAtlas;
            return textField;
        }

        private void RefreshSwatchesPanel() {
            if (swatchesPanel != null) Destroy(swatchesPanel.gameObject);
            swatchesPanel = AddUIComponent<PanelBase>();
            swatchesPanel.zOrder = 0;
            swatchesPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), false, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            swatchesPanel.autoFitChildrenHorizontally = false;
            swatchesPanel.autoLayout = true;
            swatchesPanel.width = 256.0f;
            swatchesPanel.wrapLayout = true;
            swatchesPanel.autoFitChildrenVertically = true;
            swatchesPanel.padding = new RectOffset(10, 0, 10, 0);
            foreach (var swatch in NetworkSkinPanelController.Color.Swatches) {
                AddSwatch(swatch);
            }
        }

        private void RefreshSavedSwatchesPanel() {
            if (savedSwatchesPanel != null) Destroy(savedSwatchesPanel.gameObject);
            savedSwatchesPanel = AddUIComponent<PanelBase>();
            savedSwatchesPanel.Build(PanelType.None, new Layout(new Vector2(256.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
            savedSwatchesPanel.padding = new RectOffset(5, 0, 5, 0);
            foreach (var savedSwatch in savedSwatches) {
                AddSavedSwatch(savedSwatch);
            }
        }

        private void AddSavedSwatch(SavedSwatch savedSwatch) {
            SavedSwatchPanel savedSwatchPanel = savedSwatchesPanel.AddUIComponent<SavedSwatchPanel>();
            savedSwatchPanel.Build(PanelType.None, new Layout(new Vector2(256.0f, 24.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0), savedSwatch);
            savedSwatchPanel.autoLayoutPadding = new RectOffset(5, 0, 5, 0);
            savedSwatchPanel.EventSwatchClicked += OnSavedSwatchClicked;
            savedSwatchPanel.EventRemoveSwatch += OnSavedSwatchRemoved;
            savedSwatchPanel.EventSwatchRenamed += OnSavedSwatchRenamed;
        }

        private void OnSavedSwatchRenamed(SavedSwatch savedSwatch) {
            //
        }

        private void OnSavedSwatchRemoved(SavedSwatchPanel savedSwatchPanel) {
            if (savedSwatchPanel != null) {
                savedSwatches.Remove(savedSwatchPanel.savedSwatch);
                Persistence.UpdateSavedSwatches(savedSwatches);
                Destroy(savedSwatchPanel.gameObject);
            }
        }

        private void OnSavedSwatchClicked(Color32 color) {
            UpdateColor(color);
        }

        private void AddSwatch(Color32 color) {
            SwatchButton button = swatchesPanel.AddUIComponent<SwatchButton>();
            button.Build(color);
            button.EventSwatchClicked += OnSwatchClicked;
        }
        private void CreateButtonsPanel() {
            buttonsPanel = AddUIComponent<PanelBase>();
            buttonsPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 34.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            buttonsPanel.padding = new RectOffset(10, 0, 5, 0);
            CreateResetButton();
            CreateSaveButton();
        }

        private void CreateResetButton() {
            resetButton = buttonsPanel.AddUIComponent<ButtonPanel>();
            resetButton.Build(PanelType.None, new Layout(new Vector2(0.0f, 40.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            resetButton.SetAnchor(UIAnchorStyle.Left | UIAnchorStyle.CenterVertical);
            resetButton.SetText(Translation.Instance.GetTranslation(TranslationID.BUTTON_RESET));
            resetButton.EventButtonClicked += OnResetClicked;
        }

        private void CreateSaveButton() {
            saveButton = buttonsPanel.AddUIComponent<ButtonPanel>();
            saveButton.Build(PanelType.None, new Layout(new Vector2(0.0f, 40.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            saveButton.SetAnchor(UIAnchorStyle.Left | UIAnchorStyle.CenterVertical);
            saveButton.SetText(Translation.Instance.GetTranslation(TranslationID.BUTTON_SAVE));
            saveButton.EventButtonClicked += OnSaveClicked;
            if (savedSwatches.Count == MAX_SAVED_SWATCHES) saveButton.Disable();
        }

        private void OnSaveClicked() {
            if (savedSwatches.Find(s => s.Color.r == currentColor.r && s.Color.g == currentColor.g && s.Color.b == currentColor.b) == null) {
                SavedSwatch newSwatch = new SavedSwatch() { Name = Translation.Instance.GetTranslation(TranslationID.LABEL_NEW_SWATCH), Color = currentColor };
                AddSavedSwatch(newSwatch);
                savedSwatches.Add(newSwatch);
                Persistence.UpdateSavedSwatches(savedSwatches);
            }
        }

        private void OnResetClicked() {
            NetworkSkinPanelController.Color.Reset();
            RefreshColors();
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
            return value == "" ? value : GetClampedFloat(value).ToString("F0");
        }

        private float GetClampedFloat(string value) {
            if (!float.TryParse(value, out float number)) {
                return 0.0f;
            }
            return Mathf.Clamp(number, 0, 255);
        }

        private void OnKeyPress(UIComponent component, UIKeyEventParameter parameter) {
            char ch = parameter.character;
            if (!char.IsControl(ch) && !char.IsDigit(ch)) {
                parameter.Use();
            }
        }

        private void OnTextSubmitted(UIComponent component, string value) {
            UITextField textField = component as UITextField;
            Color32 color32 = currentColor;
            if (textField == redTextField) {
                color32 = new Color32((byte)GetClampedFloat(value), color32.g, color32.b, 255);
            } else if (textField == greenTextField) {
                color32 = new Color32(color32.r, (byte)GetClampedFloat(value), color32.b, 255);
            } else if (textField == blueTextField) {
                color32 = new Color32(color32.r, color32.g, (byte)GetClampedFloat(value), 255);
            }

            UpdateColor(color32);
        }

        private void OnSwatchClicked(Color color, UIMouseEventParameter eventParam, UIComponent component) {
            UpdateColor(color);
        }

        private void OnColorUsed() {
            RefreshSwatchesPanel();
        }

        private void ColorChanged() {
            NetworkSkinPanelController.Color.SetColor(currentColor);
        }

        private void UpdateColor(Color value) {
            if(colorPicker != null) {
                colorPicker.color = value;
            } else {
                OnColorUpdated(value);
            }
        }

        private void OnColorUpdated(Color value) {
            currentColor = value;
            if (colorPanel != null) colorPanel.color = value;
            UpdateTextfields();
            updateNeeded = true;
        }

        private void UpdateTextfields() {
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
        }

        protected override void RefreshUI(NetInfo netInfo) {
            RefreshColors();
        }

        private void RefreshColors() {
            currentColor = colorPanel.color = NetworkSkinPanelController.Color.SelectedColor;
            if (colorPicker != null) {
                colorPicker.eventColorUpdated -= OnColorUpdated;
                colorPicker.color = NetworkSkinPanelController.Color.SelectedColor;
                colorPicker.eventColorUpdated += OnColorUpdated;
            }
            UpdateTextfields();
        }
    }
}
