using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class MainPanel : PanelBase
    {
        public static NetInfo Prefab { get; set; }

        private ToolBar toolBar;
        private TreePanel treesPanel;
        private LightPanel lightsPanel;
        private SurfacePanel surfacePanel;
        private PillarPanel pillarPanel;
        private CatenaryPanel catenaryPanel;
        private ColorPanel colorPanel;

        public override void OnDestroy() {
            UnregisterEvents();
            base.OnDestroy();
        }

        public override void Start() {
            Build(new Layout(new Vector2(0.0f, 234.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            color = PanelBase.GUIColor;
            relativePosition = new Vector3(100.0f, 100.0f);
            autoFitChildrenVertically = true;
            CreateToolBar();
            RegisterEvents();
        }

        protected override void RefreshUI(NetInfo netInfo) {

        }

        private void CreateToolBar() {
            toolBar = AddUIComponent<ToolBar>();
            toolBar.Build(new Layout(new Vector2(40.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0, "GenericPanel"));
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(5.0f, toolBar.height);
        }

        private void CreateTreesPanel() {
            treesPanel = AddUIComponent<TreePanel>();
            treesPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreateLightsPanel() {
            lightsPanel = AddUIComponent<LightPanel>();
            lightsPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreateSurfacePanel() {
            surfacePanel = AddUIComponent<SurfacePanel>();
            surfacePanel.Build(new Layout(new Vector2(388.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreateCatenaryPanel() {
            catenaryPanel = AddUIComponent<CatenaryPanel>();
            catenaryPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreatePillarsPanel() {
            pillarPanel = AddUIComponent<PillarPanel>();
            pillarPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreateColorsPanel() {
            colorPanel = AddUIComponent<ColorPanel>();
            colorPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void RegisterEvents() {
            RegisterClickEvents();
            RegisterVisibilityEvents();
        }

        private void UnregisterEvents() {
            UnregisterClickEvents();
            UnregisterVisibilityEvents();
        }

        private void RegisterClickEvents() {
            toolBar.ButtonBar.EventTreesClicked += OnTreesClicked;
            toolBar.ButtonBar.EventLightsClicked += OnLightsClicked;
            toolBar.ButtonBar.EventSurfacesClicked += OnSurfacesClicked;
            toolBar.ButtonBar.EventPillarsClicked += OnPillarsClicked;
            toolBar.ButtonBar.EventColorClicked += OnColorClicked;
            toolBar.ButtonBar.EventCatenaryClicked += OnCatenaryClicked;
            toolBar.ButtonBar.EventExtrasClicked += OnExtrasClicked;
        }

        private void RegisterVisibilityEvents() {
            toolBar.ButtonBar.EventTreesVisibilityChanged += OnTreesVisibilityChanged;
            toolBar.ButtonBar.EventLightsVisibilityChanged += OnLightsVisibilityChanged;
            toolBar.ButtonBar.EventSurfacesVisibilityChanged += OnSurfacesVisibilityChanged;
            toolBar.ButtonBar.EventPillarsVisibilityChanged += OnPillarsVisibilityChanged;
            toolBar.ButtonBar.EventColorVisibilityChanged += OnColorVisibilityChanged;
            toolBar.ButtonBar.EventCatenaryVisibilityChanged += OnCatenaryVisibilityChanged;
            toolBar.ButtonBar.EventExtrasVisibilityChanged += OnExtrasVisibilityChanged;
        }

        private void UnregisterClickEvents() {
            toolBar.ButtonBar.EventTreesClicked -= OnTreesClicked;
            toolBar.ButtonBar.EventLightsClicked -= OnLightsClicked;
            toolBar.ButtonBar.EventSurfacesClicked -= OnSurfacesClicked;
            toolBar.ButtonBar.EventPillarsClicked -= OnPillarsClicked;
            toolBar.ButtonBar.EventColorClicked -= OnColorClicked;
            toolBar.ButtonBar.EventCatenaryClicked -= OnCatenaryClicked;
            toolBar.ButtonBar.EventExtrasClicked -= OnExtrasClicked;
        }

        private void UnregisterVisibilityEvents() {
            toolBar.ButtonBar.EventTreesVisibilityChanged -= OnTreesVisibilityChanged;
            toolBar.ButtonBar.EventLightsVisibilityChanged -= OnLightsVisibilityChanged;
            toolBar.ButtonBar.EventSurfacesVisibilityChanged -= OnSurfacesVisibilityChanged;
            toolBar.ButtonBar.EventPillarsVisibilityChanged -= OnPillarsVisibilityChanged;
            toolBar.ButtonBar.EventColorVisibilityChanged -= OnColorVisibilityChanged;
            toolBar.ButtonBar.EventCatenaryVisibilityChanged -= OnCatenaryVisibilityChanged;
            toolBar.ButtonBar.EventExtrasVisibilityChanged -= OnExtrasVisibilityChanged;
        }

        private void OnExtrasVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
        }

        private void OnCatenaryVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && catenaryPanel != null) {
                SetButtonUnfocused(button);
                Destroy(catenaryPanel.gameObject);
            }
        }

        private void OnColorVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && colorPanel != null) {
                SetButtonUnfocused(button);
                Destroy(colorPanel.gameObject);
            }
        }

        private void OnPillarsVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && pillarPanel != null) {
                SetButtonUnfocused(button);
                Destroy(pillarPanel.gameObject);
            }
        }

        private void OnSurfacesVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && surfacePanel != null) {
                SetButtonUnfocused(button);
                Destroy(surfacePanel.gameObject);
            }
        }

        private void OnLightsVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && lightsPanel != null) {
                SetButtonUnfocused(button);
                Destroy(lightsPanel.gameObject);
            }
        }

        private void OnTreesVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && treesPanel != null) {
                SetButtonUnfocused(button);
                Destroy(treesPanel.gameObject);
            }
        }

        private void OnExtrasClicked(UIButton button, UIButton[] buttons) {
        }

        private void OnCatenaryClicked(UIButton button, UIButton[] buttons) {
            if (catenaryPanel != null) {
                SetButtonUnfocused(button);
                Destroy(catenaryPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateCatenaryPanel();
            }
        }

        private void OnColorClicked(UIButton button, UIButton[] buttons) {
            if (colorPanel != null) {
                SetButtonUnfocused(button);
                Destroy(colorPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateColorsPanel();
            }
        }

        private void OnSurfacesClicked(UIButton button, UIButton[] buttons) {
            if (surfacePanel != null) {
                SetButtonUnfocused(button);
                Destroy(surfacePanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateSurfacePanel();
            }
        }

        private void OnPillarsClicked(UIButton button, UIButton[] buttons) {
            if (pillarPanel != null) {
                SetButtonUnfocused(button);
                Destroy(pillarPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreatePillarsPanel();
            }
        }

        private void OnLightsClicked(UIButton button, UIButton[] buttons) {
            if (lightsPanel != null) {
                SetButtonUnfocused(button);
                Destroy(lightsPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateLightsPanel();
            }
        }

        private void OnTreesClicked(UIButton button, UIButton[] buttons) {
            if (treesPanel != null) {
                SetButtonUnfocused(button);
                Destroy(treesPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateTreesPanel();
            }
        }

        private void CloseAll() {
            if (treesPanel != null) {
                Destroy(treesPanel.gameObject);
            }
            if (lightsPanel != null) {
                Destroy(lightsPanel.gameObject);
            }
            if (surfacePanel != null) {
                Destroy(surfacePanel.gameObject);
            }
            if (pillarPanel != null) {
                Destroy(pillarPanel.gameObject);
            }
            if (catenaryPanel != null) {
                Destroy(catenaryPanel.gameObject);
            }
            if (colorPanel != null) {
                Destroy(colorPanel.gameObject);
            }
        }

        private void RefreshButtons(UIButton focusedButton, UIButton[] buttons) {
            for (int i = 0; i < buttons.Length; i++) {
                SetButtonUnfocused(buttons[i]);
            }
            SetButtonFocused(focusedButton);
        }

        private void SetButtonFocused(UIButton button) {
            if (button != null) {
                button.normalBgSprite = button.focusedBgSprite = button.hoveredBgSprite = string.Concat(button.normalBgSprite.Replace("Focused", ""), "Focused");
            }
        }

        private void SetButtonUnfocused(UIButton button) {
            if (button != null) {
                button.normalBgSprite = button.focusedBgSprite = button.normalBgSprite.Replace("Focused", "");
                button.hoveredBgSprite = button.hoveredBgSprite.Replace("Focused", "Hovered");
            }
        }
    }
}
