using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ColorPanel : PanelBase
    {
        private ColorFieldPanel colorFieldPanel;
        private SwatchesPanel swatchesPanel1;
        private SwatchesPanel swatchesPanel2;
        private UIColorField colorField;
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

        public override void OnDestroy() {
            base.OnDestroy();
            colorField.eventColorPickerOpen -= OnColorPickerOpen;
            colorField.eventSelectedColorChanged -= OnColorChanged;
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateColorFieldPanel();
            CreateSwatchesPanel();
        }

        internal void OnSwatchClicked(Color32 color, UIMouseEventParameter eventParam, UIComponent component) {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) {
                Persistence.RemoveSwatch(color);
                UnityEngine.Object.Destroy(component.gameObject);
            } else {
                this.currentColor = color;
                SkinController.SetColor(color);
            }
        }

        private void ColorChanged() {
            swatches.AddSwatch(this, swatches.Count < 10 ? swatchesPanel1 : swatchesPanel2, currentColor);
            SkinController.SetColor(currentColor);
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
        }

        private void CreateColorFieldPanel() {
            colorFieldPanel = AddUIComponent<ColorFieldPanel>();
            colorFieldPanel.Build(PanelType.None, new Layout(new Vector2(0.0f, 45.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            UIColorField field = UITemplateManager.Get<UIPanel>("LineTemplate").Find<UIColorField>("LineColor");
            colorField = GameObject.Instantiate<UIColorField>(field);
            colorFieldPanel.AttachUIComponent(colorField.gameObject);
            colorField.eventColorPickerOpen += OnColorPickerOpen;
            colorField.eventSelectedColorChanged += OnColorChanged;
            colorField.size = new Vector2(50.0f, 50.0f);
        }

        private void OnColorChanged(UIComponent component, Color value) {
            currentColor = value;
            updateNeeded = true;
        }

        private void OnColorPickerOpen(UIColorField dropdown, UIColorPicker popup, ref bool overridden) {

        }

        protected override void RefreshUI(NetInfo netInfo) {

        }

        public class ColorFieldPanel : PanelBase
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
