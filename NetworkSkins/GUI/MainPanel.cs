using System;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class MainPanel : PanelBase
    {
        public static NetInfo Prefab { get; set; }
        public static Color32 GUIColor { get; set; } = new Color32(128, 128, 128, 255);

        private ToolBar toolBar;
        private TreePanel treesPanel;
        private LightPanel lightsPanel;
        private SurfacePanel surfacePanel;
        private PillarPanel pillarPanel;
        private CatenaryPanel catenaryPanel;

        public override void OnDestroy() {
            UnregisterEvents();
            base.OnDestroy();
        }

        public override void Start() {
            Build(new Layout(new Vector2(0.0f, 234.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            color = GUIColor;
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
            surfacePanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreateCatenaryPanel() {
            catenaryPanel = AddUIComponent<CatenaryPanel>();
            catenaryPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
        }

        private void CreatePillarsPanel() {
            pillarPanel = AddUIComponent<PillarPanel>();
            pillarPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
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

        private void OnExtrasVisibilityChanged(bool visible) {
        }

        private void OnCatenaryVisibilityChanged(bool visible) {
        }

        private void OnColorVisibilityChanged(bool visible) {
        }

        private void OnPillarsVisibilityChanged(bool visible) {
        }

        private void OnSurfacesVisibilityChanged(bool visible) {
        }

        private void OnLightsVisibilityChanged(bool visible) {
            if (!visible && lightsPanel != null) {
                Destroy(lightsPanel.gameObject);
            }
        }

        private void OnTreesVisibilityChanged(bool visible) {
            if (!visible && treesPanel != null) {
                Destroy(treesPanel.gameObject);
            }
        }

        private void OnExtrasClicked() {
        }

        private void OnCatenaryClicked() {
            if (catenaryPanel != null) {
                Destroy(catenaryPanel.gameObject);
            } else {
                CloseAll();
                CreateCatenaryPanel();
            }
        }

        private void OnColorClicked() {
        }

        private void OnSurfacesClicked() {
            if (surfacePanel != null) {
                Destroy(surfacePanel.gameObject);
            } else {
                CloseAll();
                CreateSurfacePanel();
            }
        }

        private void OnPillarsClicked() {
            if (pillarPanel != null) {
                Destroy(pillarPanel.gameObject);
            } else {
                CloseAll();
                CreatePillarsPanel();
            }
        }

        private void OnLightsClicked() {
            if (lightsPanel != null) {
                Destroy(lightsPanel.gameObject);
            } else {
                CloseAll();
                CreateLightsPanel();
            }
        }

        private void OnTreesClicked() {
            if (treesPanel != null) {
                Destroy(treesPanel.gameObject);
            } else {
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
        }
    }
}
