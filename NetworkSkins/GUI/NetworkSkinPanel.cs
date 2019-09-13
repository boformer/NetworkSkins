using System;
using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.GUI.Catenaries;
using NetworkSkins.GUI.Colors;
using NetworkSkins.GUI.Lights;
using NetworkSkins.GUI.Pillars;
using NetworkSkins.GUI.Surfaces;
using NetworkSkins.GUI.Trees;
using NetworkSkins.Tool;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class NetworkSkinPanel : PanelBase
    {
        private ToolBar toolBar;
        private TreePanel treesPanel;
        private StreetLightPanel lightsPanel;
        private TerrainSurfacePanel terrainSurfacePanel;
        private PillarPanel pillarPanel;
        private CatenaryPanel catenaryPanel;
        private ColorPanel colorPanel;
        private SettingsPanel settingsPanel;
        private UIPanel space;
        private PanelBase currentPanel;
        public override void OnDestroy() {
            toolBar.EventDragEnd -= OnToolBarDragEnd;
            UnregisterEvents();
            base.OnDestroy();
        }

        public override void Start() {
            base.Start();
            Build(PanelType.None, new Layout(new Vector2(0.0f, 234.0f), true, LayoutDirection.Horizontal, LayoutStart.BottomRight, 0));
            color = GUIColor;
            CreateToolBar();

            relativePosition = Persistence.GetToolbarPosition() ?? CalculateDefaultToolbarPosition();
            EnsureToolbarOnScreen();

            RefreshZOrder();
            RegisterEvents();
        }

        private static Vector2 CalculateDefaultToolbarPosition()
        {
            Vector2 screenRes = UIView.GetAView().GetScreenResolution();
            return screenRes - new Vector2(10.0f, 130.0f);
        }

        private void EnsureToolbarOnScreen()
        {
            Vector2 screenRes = UIView.GetAView().GetScreenResolution();
            if(relativePosition.x < 0f || relativePosition.x > screenRes.x || relativePosition.y < 0f || relativePosition.y > screenRes.y)
            {
                relativePosition = CalculateDefaultToolbarPosition();
            }
        }

        public override void Update() {
            base.Update();
            Vector2 screenRes = UIView.GetAView().GetScreenResolution();

            if ((autoLayoutStart == LayoutStart.TopLeft  || autoLayoutStart == LayoutStart.BottomLeft) && relativePosition.x > screenRes.x / 2.0f) {
                autoLayoutStart = autoLayoutStart == LayoutStart.TopLeft ? LayoutStart.TopRight : LayoutStart.BottomRight;
                RefreshZOrder();
            } else if ((autoLayoutStart == LayoutStart.TopRight || autoLayoutStart == LayoutStart.BottomRight) && relativePosition.x + width < screenRes.x / 2.0f) {
                autoLayoutStart = autoLayoutStart == LayoutStart.TopRight ? LayoutStart.TopLeft : LayoutStart.BottomLeft;
                RefreshZOrder();
            }
            if ((autoLayoutStart == LayoutStart.TopLeft || autoLayoutStart == LayoutStart.TopRight) && relativePosition.y > screenRes.y / 2.0f) {
                autoLayoutStart = autoLayoutStart == LayoutStart.TopLeft ? LayoutStart.BottomLeft : LayoutStart.BottomRight;
                RefreshZOrder();
            } else if ((autoLayoutStart == LayoutStart.BottomLeft || autoLayoutStart == LayoutStart.BottomRight) && relativePosition.y + height < screenRes.y / 2.0f) {
                autoLayoutStart = autoLayoutStart == LayoutStart.BottomLeft ? LayoutStart.TopLeft : LayoutStart.TopRight;
                RefreshZOrder();
            }
        }

        private void RefreshZOrder() {
            if (autoLayoutStart == LayoutStart.TopLeft || autoLayoutStart == LayoutStart.BottomLeft) {
                toolBar.zOrder = 0;
                space.zOrder = 1;
                if (currentPanel != null) currentPanel.zOrder = 2;
            } else if (autoLayoutStart == LayoutStart.TopRight || autoLayoutStart == LayoutStart.BottomRight) {
                if (currentPanel != null) {
                    currentPanel.zOrder = 0;
                    space.zOrder = 1;
                    toolBar.zOrder = 2;
                } else {
                    space.zOrder = 0;
                    toolBar.zOrder = 1;
                }
            }
        }

        private void CreateToolBar() {
            toolBar = AddUIComponent<ToolBar>();
            toolBar.Build(PanelType.None, new Layout(new Vector2(40.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0, "GenericPanel"));
            space = AddUIComponent<UIPanel>();
            space.size = new Vector2(5.0f, toolBar.height);
        }

        private void CreateTreesPanel() {
            treesPanel = AddUIComponent<TreePanel>();
            treesPanel.Build(PanelType.Trees, new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            currentPanel = treesPanel;
        }

        private void CreateLightsPanel() {
            lightsPanel = AddUIComponent<StreetLightPanel>();
            lightsPanel.Build(PanelType.Lights, new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            currentPanel = lightsPanel;
        }

        private void CreateSurfacePanel() {
            terrainSurfacePanel = AddUIComponent<TerrainSurfacePanel>();
            terrainSurfacePanel.Build(PanelType.Surfaces, new Layout(new Vector2(388.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            currentPanel = terrainSurfacePanel;
        }

        private void CreateCatenaryPanel() {
            catenaryPanel = AddUIComponent<CatenaryPanel>();
            catenaryPanel.Build(PanelType.Catenary, new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            currentPanel = catenaryPanel;
        }

        private void CreatePillarsPanel() {
            pillarPanel = AddUIComponent<PillarPanel>();
            pillarPanel.Build(PanelType.Pillars, new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            currentPanel = pillarPanel;
        }

        private void CreateColorsPanel() {
            colorPanel = AddUIComponent<ColorPanel>();
            colorPanel.Build(PanelType.Color, new Layout(new Vector2(255f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0, "GenericPanel"));
            currentPanel = colorPanel;
        }

        private void CreateSettingsPanel() {
            settingsPanel = AddUIComponent<SettingsPanel>();
            settingsPanel.Build(PanelType.Settings, new Layout(new Vector2(228.6f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0, "GenericPanel"));
            currentPanel = settingsPanel;
        }

        private void RegisterEvents() {
            toolBar.EventDragEnd += OnToolBarDragEnd;
            RegisterClickEvents();
            RegisterVisibilityEvents();
        }

        private void OnToolBarDragEnd() {
            Persistence.SetToolbarPosition(relativePosition);
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
            toolBar.ButtonBar.EventPipetteClicked += OnPipetteClicked;
        }

        private void RegisterVisibilityEvents() {
            toolBar.ButtonBar.EventTreesVisibilityChanged += OnTreesVisibilityChanged;
            toolBar.ButtonBar.EventLightsVisibilityChanged += OnLightsVisibilityChanged;
            toolBar.ButtonBar.EventSurfacesVisibilityChanged += OnSurfacesVisibilityChanged;
            toolBar.ButtonBar.EventPillarsVisibilityChanged += OnPillarsVisibilityChanged;
            toolBar.ButtonBar.EventColorVisibilityChanged += OnColorVisibilityChanged;
            toolBar.ButtonBar.EventCatenaryVisibilityChanged += OnCatenaryVisibilityChanged;
            toolBar.ButtonBar.EventSettingsVisibilityChanged += OnSettingsVisibilityChanged;
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
            toolBar.ButtonBar.EventSettingsVisibilityChanged -= OnSettingsVisibilityChanged;
        }

        private void OnSettingsVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && settingsPanel != null) {
                SetButtonUnfocused(button);
                Destroy(settingsPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnCatenaryVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && catenaryPanel != null) {
                SetButtonUnfocused(button);
                Destroy(catenaryPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnColorVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && colorPanel != null) {
                SetButtonUnfocused(button);
                Destroy(colorPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnPillarsVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && pillarPanel != null) {
                SetButtonUnfocused(button);
                Destroy(pillarPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnSurfacesVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && terrainSurfacePanel != null) {
                SetButtonUnfocused(button);
                Destroy(terrainSurfacePanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnLightsVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && lightsPanel != null) {
                SetButtonUnfocused(button);
                Destroy(lightsPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnTreesVisibilityChanged(UIButton button, UIButton[] buttons, bool visible) {
            if (!visible && treesPanel != null) {
                SetButtonUnfocused(button);
                Destroy(treesPanel.gameObject);
            }
            RefreshZOrder();
        }

        private void OnExtrasClicked(UIButton button, UIButton[] buttons) {
            if (settingsPanel != null) {
                SetButtonUnfocused(button);
                Destroy(settingsPanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateSettingsPanel();
            }
            RefreshZOrder();
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
            RefreshZOrder();
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
            RefreshZOrder();
        }

        private void OnSurfacesClicked(UIButton button, UIButton[] buttons) {
            if (terrainSurfacePanel != null) {
                SetButtonUnfocused(button);
                Destroy(terrainSurfacePanel.gameObject);
            } else {
                RefreshButtons(button, buttons);
                CloseAll();
                CreateSurfacePanel();
            }
            RefreshZOrder();
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
            RefreshZOrder();
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
            RefreshZOrder();
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
            RefreshZOrder();
        }

        private void OnPipetteClicked(UIButton focusedButton, UIButton[] buttons) {
            ToolsModifierControl.SetTool<PipetteTool>();
        }

        private void CloseAll() {
            if (treesPanel != null) {
                Destroy(treesPanel.gameObject);
            }
            if (lightsPanel != null) {
                Destroy(lightsPanel.gameObject);
            }
            if (terrainSurfacePanel != null) {
                Destroy(terrainSurfacePanel.gameObject);
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
            if (settingsPanel != null) {
                Destroy(settingsPanel.gameObject);
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