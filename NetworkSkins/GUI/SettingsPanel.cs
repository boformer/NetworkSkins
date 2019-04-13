using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SettingsPanel : PanelBase
    {
        private UICheckBox activeSelectionCheckbox;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            CreateActiveSelectionCheckbox();
        }

        private void CreateActiveSelectionCheckbox() {
            activeSelectionCheckbox = AddUIComponent<UICheckBox>();
            activeSelectionCheckbox.size = new Vector2(12.0f, 12.0f);
            var sprite = activeSelectionCheckbox.AddUIComponent<UISprite>();
            sprite.spriteName = "AchievementCheckedFalse";
            sprite.size = activeSelectionCheckbox.size;
            sprite.transform.parent = activeSelectionCheckbox.transform;
            sprite.transform.localPosition = Vector3.zero;
            activeSelectionCheckbox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)activeSelectionCheckbox.checkedBoxObject).spriteName = "AchievementCheckedTrue";
            activeSelectionCheckbox.checkedBoxObject.size = activeSelectionCheckbox.size;
            activeSelectionCheckbox.checkedBoxObject.relativePosition = Vector3.zero;
            activeSelectionCheckbox.isChecked = Persistence.SaveActiveSelectionGlobally;
            activeSelectionCheckbox.eventCheckChanged += OnActiveSelectionOptionChanged; ;
        }

        private void OnActiveSelectionOptionChanged(UIComponent component, bool value) {
            Persistence.SaveActiveSelectionGlobally = value;
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}
