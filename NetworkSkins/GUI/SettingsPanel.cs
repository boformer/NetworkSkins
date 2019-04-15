using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class SettingsPanel : PanelBase
    {
        private CheckboxPanel activeSelectionCheckbox;
        private ButtonPanel resetButton;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            UIUtil.CreateSpace(1.0f, 3.0f, this);
            CreateActiveSelectionCheckbox();
            CreateResetButton();
            UIUtil.CreateSpace(1.0f, 5.0f, this);
        }
        private void CreateResetButton() {
            resetButton = AddUIComponent<ButtonPanel>();
            resetButton.Build(PanelType.None, new Layout(new Vector2(width, 40.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            resetButton.padding = new RectOffset(10, 0, 5, 0);
            resetButton.SetAnchor(UIAnchorStyle.Left | UIAnchorStyle.CenterVertical);
            resetButton.SetText(Translation.Instance.GetTranslation(TranslationID.BUTTON_RESET), Translation.Instance.GetTranslation(TranslationID.TOOLTIP_RESETCURRENT));
            resetButton.EventButtonClicked += OnResetClicked;
        }

        private void OnResetClicked() {
            NetworkSkinPanelController.OnReset();
        }

        private void CreateActiveSelectionCheckbox() {
            activeSelectionCheckbox = AddUIComponent<CheckboxPanel>();
            activeSelectionCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            activeSelectionCheckbox.Initialize(
                Persistence.SaveActiveSelectionGlobally, 
                Translation.Instance.GetTranslation(TranslationID.LABEL_GLOBAL_SELECTION_DATA), 
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_GLOBAL_SELECTION_DATA));
            activeSelectionCheckbox.EventCheckboxStateChanged += OnActiveSelectionOptionChanged;
        }

        private void OnActiveSelectionOptionChanged(bool value) {
            Persistence.SaveActiveSelectionGlobally = value;
        }
    }
}
