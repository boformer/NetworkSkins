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
        private CheckboxPanel hideBlacklistedCheckbox;
        private CheckboxPanel displayAtSelectedCheckbox;
        private ButtonPanel resetButton;

        public override void OnDestroy() {
            resetButton.EventButtonClicked -= OnResetClicked;
            activeSelectionCheckbox.EventCheckboxStateChanged -= OnActiveSelectionOptionChanged;
            hideBlacklistedCheckbox.EventCheckboxStateChanged -= OnHideBlacklistedOptionChanged;
            displayAtSelectedCheckbox.EventCheckboxStateChanged -= OnDisplayAtSelectedOptionChanged;
            base.OnDestroy();
        }
        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            color = GUIColor;
            UIUtil.CreateSpace(1.0f, 3.0f, this);
            CreateActiveSelectionCheckbox();
            CreateHideBlacklistedCheckbox();
            CreateDisplayAtSelectedCheckbox();
            CreateResetButton();
            UIUtil.CreateSpace(1.0f, 5.0f, this);
            autoFitChildrenHorizontally = true;
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

        private void CreateHideBlacklistedCheckbox() {
            hideBlacklistedCheckbox = AddUIComponent<CheckboxPanel>();
            hideBlacklistedCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            hideBlacklistedCheckbox.Initialize(
                Persistence.HideBlacklisted,
                Translation.Instance.GetTranslation(TranslationID.LABEL_HIDEBLACKLISTED),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_HIDEBLACKLISTED));
            hideBlacklistedCheckbox.EventCheckboxStateChanged += OnHideBlacklistedOptionChanged;
        }

        private void CreateDisplayAtSelectedCheckbox() {
            displayAtSelectedCheckbox = AddUIComponent<CheckboxPanel>();
            displayAtSelectedCheckbox.Build(PanelType.None, new Layout(new Vector2(0.0f, 28.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 10));
            displayAtSelectedCheckbox.Initialize(
                Persistence.DisplayAtSelected,
                Translation.Instance.GetTranslation(TranslationID.LABEL_DISPLAYATSELECTED),
                Translation.Instance.GetTranslation(TranslationID.TOOLTIP_DISPLAYATSELECTED));
            displayAtSelectedCheckbox.EventCheckboxStateChanged += OnDisplayAtSelectedOptionChanged;
        }

        private void OnActiveSelectionOptionChanged(bool value) {
            Persistence.SaveActiveSelectionGlobally = value;
        }

        private void OnHideBlacklistedOptionChanged(bool value) {
            Persistence.HideBlacklisted = value;
        }

        private void OnDisplayAtSelectedOptionChanged(bool value) {
            Persistence.HideBlacklisted = value;
        }
    }
}
