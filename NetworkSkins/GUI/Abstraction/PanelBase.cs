using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class PanelBase : UIPanel
    {
        public Layout Layout { get; set; }
        public int Spacing { get; set; }

        public virtual void Build(Layout layout) {
            Layout = layout;
            Layout.Apply(this);
        }

        public void CreateSpace(float width, float height) {
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(width, height);
        }

        protected UIButton CreateButton(Vector2 size, string text = "", string tooltip = "", string foregroundSprite = "", string backgroundSprite = "ButtonSmall", bool isFocusable = false, UITextureAtlas atlas = null, UIComponent parentComponent = null) {
            UIButton button = parentComponent != null ? parentComponent.AddUIComponent<UIButton>() : AddUIComponent<UIButton>();
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

        protected abstract void RefreshUI(NetInfo netInfo);
    }
}
