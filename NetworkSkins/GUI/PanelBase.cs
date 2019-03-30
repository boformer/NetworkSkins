using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class PanelBase : UIPanel
    {
        public Layout Layout { get; set; }
        public int Spacing { get; set; }

        public virtual void Build(Layout layout) {
            Layout = layout;
            Layout.Apply(this);
        }

        protected UIButton MakeButton(Vector2 size, string text = "", string tooltip = "", string foregroundSprite = "", string backgroundSprite = "ButtonSmall", bool isFocusable = false, UITextureAtlas atlas = null) {
            UIButton button = AddUIComponent<UIButton>();
            button.size = size;
            button.text = text;
            button.tooltip = tooltip;
            button.textPadding = new RectOffset(0, 0, 3, 0);
            button.normalBgSprite = backgroundSprite;
            button.hoveredBgSprite = string.Concat(backgroundSprite, "Hovered");
            button.pressedBgSprite = string.Concat(backgroundSprite, "Pressed");
            button.focusedBgSprite = string.Concat(backgroundSprite, isFocusable ? "Focused" : "");
            button.disabledBgSprite = string.Concat(backgroundSprite, "Disabled");
            button.normalFgSprite = foregroundSprite;
            button.hoveredFgSprite = string.Concat(foregroundSprite, "Hovered");
            button.pressedFgSprite = string.Concat(foregroundSprite, "Pressed");
            button.focusedFgSprite = string.Concat(foregroundSprite, isFocusable ? "Focused" : "");
            button.disabledFgSprite = string.Concat(foregroundSprite, "Disabled");
            if (atlas != null) button.atlas = atlas;
            return button;
        }

        protected virtual void RefreshUI() {

        }
    }
}
