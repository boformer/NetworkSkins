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
        }

        public void Initialize(bool state, string text, string tooltip) {
            checkbox.isChecked = state;
            label.text = text;
            label.tooltip = checkbox.tooltip = tooltip;
        }

        private void CreateCheckbox() {
            checkbox = AddUIComponent<UICheckBox>();
            checkbox.size = new Vector2(12.0f, 12.0f);
            var sprite = checkbox.AddUIComponent<UISprite>();
            sprite.spriteName = "AchievementCheckedFalse";
            sprite.size = checkbox.size;
            sprite.transform.parent = checkbox.transform;
            sprite.transform.localPosition = Vector3.zero;
            checkbox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkbox.checkedBoxObject).spriteName = "AchievementCheckedTrue";
            checkbox.checkedBoxObject.size = checkbox.size;
            checkbox.checkedBoxObject.relativePosition = Vector3.zero;
            checkbox.eventCheckChanged += OnCheckboxStateChanged;
        }

        private void OnCheckboxStateChanged(UIComponent component, bool value) {
            EventCheckboxStateChanged?.Invoke(value);
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}
