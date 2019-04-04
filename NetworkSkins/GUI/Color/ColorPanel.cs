using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ColorPanel : PanelBase
    {
        private UIColorField colorField;
        private UITextField redTextField;
        private UITextField greenTextField;
        private UITextField blueTextField;
        private UIPanel[] swatches;

        public override void OnDestroy() {
            base.OnDestroy();
            colorField.eventColorPickerOpen -= OnColorPickerOpen;
            colorField.eventSelectedColorChanged -= OnColorChanged;
        }
        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            wrapLayout = true;
            CreateColorField();
            CreateSwatches();
        }

        private void CreateSwatches() {
            swatches = new UIPanel[20];
            for (int i = 0; i < swatches.Length; i++) {
                UIPanel panel = AddUIComponent<UIPanel>();
                panel.size = new Vector2(20.0f, 20.0f);
                panel.backgroundSprite = "WhiteRect";
            }
        }

        private void CreateColorField() {
            UIColorField field = UITemplateManager.Get<UIPanel>("LineTemplate").Find<UIColorField>("LineColor");
            colorField = GameObject.Instantiate<UIColorField>(field);
            AttachUIComponent(colorField.gameObject);
            colorField.eventColorPickerOpen += OnColorPickerOpen;
            colorField.eventSelectedColorChanged += OnColorChanged;
        }

        private void OnColorChanged(UIComponent component, Color value) {
            SkinController.SetColor(value);
        }

        private void OnColorPickerOpen(UIColorField dropdown, UIColorPicker popup, ref bool overridden) {

        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}
