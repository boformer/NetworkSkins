using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class PanelBase : UIPanel
    {
        public Layout Layout { get; set; }
        public int Spacing { get; set; }
        public static Color32 GUIColor { get; set; } = new Color32(128, 128, 128, 255);
        public static Color32 FocusedColor = new Color32(88, 181, 205, 255);
        private NetToolMonitor Monitor => NetToolMonitor.Instance;


        public override void Awake() {
            base.Awake();
            Monitor.EventPrefabChanged += OnPrefabChanged;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            Monitor.EventPrefabChanged -= OnPrefabChanged;
        }
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
            button.normalFgSprite = foregroundSprite;
            button.hoveredFgSprite = string.Concat(foregroundSprite, "Hovered");
            button.pressedFgSprite = string.Concat(foregroundSprite, "Pressed");
            button.focusedFgSprite = string.Concat(foregroundSprite, isFocusable ? "Focused" : "");
            if (atlas != null) button.atlas = atlas;
            
            return button;
        }

        protected abstract void RefreshUI(NetInfo netInfo);

        protected virtual void RefreshAfterBuild() {
            RefreshUI(Monitor.Prefab);
        }

        private void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI(netInfo);
        }
    }
}
