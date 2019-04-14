using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SettingsPanel : PanelBase
    {
        private CheckboxPanel activeSelectionCheckbox;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            UIUtil.CreateSpace(1.0f, 3.0f, this);
            CreateActiveSelectionCheckbox();
        }

        private void CreateActiveSelectionCheckbox() {
            activeSelectionCheckbox = AddUIComponent<CheckboxPanel>();
            activeSelectionCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 5));
            activeSelectionCheckbox.Initialize(
                Persistence.SaveActiveSelectionGlobally, 
                Translation.Instance.GetTranslation(TranslationID.LABEL_GLOBAL_SELECTION_DATA), 
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_GLOBAL_SELECTION_DATA));
            activeSelectionCheckbox.EventCheckboxStateChanged += OnActiveSelectionOptionChanged;
        }

        private void OnActiveSelectionOptionChanged(bool value) {
            Persistence.SaveActiveSelectionGlobally = value;
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }
    }
}
