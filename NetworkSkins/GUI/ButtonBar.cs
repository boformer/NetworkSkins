using ColossalFramework;
using ColossalFramework.UI;
using NetworkSkins.GUI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ButtonBar : PanelBase
    {
        public delegate void PanelButtonClickedEventHandler(PanelType type);
        public event PanelButtonClickedEventHandler EventPanelButtonClicked;

        private PanelButton[] buttons;

        public void SetVisiblePanelButtons(PanelType visibleTypes)
        {
            foreach (var button in buttons)
            {
                button.isVisible = visibleTypes.IsFlagSet(button.type);
            }
        }

        public void SetSelectedPanelButton(PanelType type)
        {
            foreach (PanelButton button in buttons)
            {
                if (button.type == type)
                {
                    SetButtonFocused(button);
                }
                else
                {
                    SetButtonUnfocused(button);
                }
            }
        }

        protected override void RefreshUI(NetInfo netInfo)
        {
            // TODO not necessary? this is handled by the parent component
        }

        public override void Build(Layout layout)
        {
            base.Build(layout);

            CreateButtons();

            UIPanel space = AddUIComponent<UIPanel>();
            space.size = new Vector2(width, 0.1f);

            SetVisiblePanelButtons(PanelType.None);
        }

        private void CreateButtons()
        {
            buttons = new PanelButton[]
            {
                CreatePanelButton(PanelType.Trees, Resources.Tree, TranslationID.TOOLTIP_TREES),
                CreatePanelButton(PanelType.Lights, Resources.Light, TranslationID.TOOLTIP_LIGHTS),
                CreatePanelButton(PanelType.Surfaces, Resources.Surface, TranslationID.TOOLTIP_SIDEWALKS),
                CreatePanelButton(PanelType.Pillars, Resources.Pillar, TranslationID.TOOLTIP_PILLARS),
                CreatePanelButton(PanelType.Catenary, Resources.Catenary, TranslationID.TOOLTIP_CATENARY),
                CreatePanelButton(PanelType.Color, Resources.Color, TranslationID.TOOLTIP_COLOR),
                CreatePanelButton(PanelType.Extras, Resources.Settings, TranslationID.TOOLTIP_EXTRAS)
            };
        }

        private PanelButton CreatePanelButton(PanelType type, string backgroundSprite, string tooltipId)
        {
            PanelButton button = AddUIComponent<PanelButton>();
            button.type = type;
            button.size = new Vector2(Layout.Size.x - Layout.Spacing * 2, size.x - Layout.Spacing * 2);
            button.tooltip = Translation.Instance.GetTranslation(tooltipId);
            button.textPadding = new RectOffset(0, 0, 3, 0);
            button.normalBgSprite = backgroundSprite;
            button.hoveredBgSprite = string.Concat(backgroundSprite, "Hovered");
            button.pressedBgSprite = string.Concat(backgroundSprite, "Pressed");
            button.focusedBgSprite = string.Concat(backgroundSprite, "Focused");
            button.atlas = Resources.Atlas;
            button.eventClicked += (component, e) => EventPanelButtonClicked?.Invoke(type);
            return button;
        }

        private static void SetButtonFocused(UIButton button)
        {
            button.normalBgSprite = button.focusedBgSprite = button.hoveredBgSprite = string.Concat(button.normalBgSprite.Replace("Focused", ""), "Focused");
        }

        private static void SetButtonUnfocused(UIButton button)
        {
            button.normalBgSprite = button.focusedBgSprite = button.normalBgSprite.Replace("Focused", "");
            button.hoveredBgSprite = button.hoveredBgSprite.Replace("Focused", "Hovered");
        }
    }
}

public class PanelButton : UIButton
{
    public PanelType type;
}