using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class CheckboxPanel : PanelBase
    {
        public delegate void CheckboxStateChangedEventHandler(bool state);
        public event CheckboxStateChangedEventHandler EventCheckboxStateChanged;

        private UICheckBox checkbox;
        private UILabel label;

        public override void OnDestroy() {
            checkbox.eventCheckChanged -= OnCheckboxStateChanged;
            base.OnDestroy();
        }
        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateCheckbox();
            CreateLabel();
            UIUtil.CreateSpace(0.0f, label.height, this);
        }

        private void CreateLabel() {
            label = AddUIComponent<UILabel>();
            label.textAlignment = UIHorizontalAlignment.Left;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.textColor = UIUtil.TextColor;
            label.font = UIUtil.Font;
            label.atlas = Sprites.DefaultAtlas;
        }

        public void Initialize(bool state, string text, string tooltip) {
            checkbox.isChecked = state;
            label.text = text;
            label.tooltip = checkbox.tooltip = tooltip;
        }

        private void CreateCheckbox() {
            checkbox = AddUIComponent<UICheckBox>();
            checkbox.size = new Vector2(15.0f, 15.0f);
            var sprite = checkbox.AddUIComponent<UISprite>();
            sprite.spriteName = "check-unchecked";
            sprite.atlas = Sprites.DefaultAtlas;
            sprite.size = checkbox.size;
            sprite.transform.parent = checkbox.transform;
            sprite.transform.localPosition = Vector3.zero;
            var checkedBoxObj = sprite.AddUIComponent<UISprite>();
            checkedBoxObj.spriteName = "check-checked";
            checkedBoxObj.atlas = Sprites.DefaultAtlas;
            checkedBoxObj.size = checkbox.size;
            checkedBoxObj.relativePosition = Vector3.zero;
            checkbox.checkedBoxObject = checkedBoxObj;
            checkbox.eventCheckChanged += OnCheckboxStateChanged;
        }

        private void OnCheckboxStateChanged(UIComponent component, bool value) {
            EventCheckboxStateChanged?.Invoke(value);
        }
    }
}
