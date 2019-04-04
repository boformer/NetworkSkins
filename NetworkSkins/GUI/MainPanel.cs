using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class MainPanel : PanelBase
    {
        private NetToolMonitor Monitor => NetToolMonitor.Instance;

        public static NetInfo Prefab { get; set; }

        private ToolBar toolBar;

        private PanelBase currentPanel;
        private PanelType currentPanelType = PanelType.None;

        public override void Start() {
            Build(new Layout(new Vector2(0.0f, 234.0f), true, LayoutDirection.Horizontal, LayoutStart.TopLeft, 0));
            color = PanelBase.GUIColor;
            relativePosition = new Vector3(100.0f, 100.0f);
            autoFitChildrenVertically = true;

            CreateToolBar();
            toolBar.ButtonBar.EventPanelButtonClicked += OnPanelButtonClicked;

            RefreshAfterBuild();
        }

        protected override void RefreshUI(NetInfo netInfo)
        {
            PanelType visiblePanels = PanelType.Extras;
            if (Monitor.NetInfoHasTrees) visiblePanels |= PanelType.Trees;
            if (Monitor.NetInfoHasStreetLights) visiblePanels |= PanelType.Lights;
            if (Monitor.NetInfoHasSurfaces) visiblePanels |= PanelType.Surfaces;
            if (Monitor.NetInfoHasPillars) visiblePanels |= PanelType.Pillars;
            if (Monitor.NetInfoIsColorable) visiblePanels |= PanelType.Color;
            if (Monitor.NetInfoHasCatenaries) visiblePanels |= PanelType.Catenary;

            if (currentPanel != null && !visiblePanels.IsFlagSet(currentPanelType))
            {
                CloseCurrentPanel();
            }

            toolBar.ButtonBar.SetVisiblePanelButtons(visiblePanels);
        }

        private void OnPanelButtonClicked(PanelType type)
        {
            currentPanelType = currentPanelType != type ? type : PanelType.None;

            CloseCurrentPanel();

            if (currentPanelType != PanelType.None)
            {
                ShowPanel(currentPanelType);
            }

            toolBar.ButtonBar.SetSelectedPanelButton(currentPanelType);
        }

        private void ShowPanel(PanelType type)
        {
            CloseCurrentPanel();

            currentPanel = CreatePanel(type);
        }

        private void CloseCurrentPanel()
        {
            if (currentPanel != null)
            {
                Destroy(currentPanel.gameObject);
                currentPanel = null;
            }
        }

        #region Child Component Builders
        private void CreateToolBar()
        {
            toolBar = AddUIComponent<ToolBar>();
            toolBar.Build(new Layout(new Vector2(40.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0, "GenericPanel"));
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.size = new Vector2(5.0f, toolBar.height);
        }

        private PanelBase CreatePanel(PanelType type)
        {
            switch (type)
            {
                case PanelType.Trees:
                    return CreateTreesPanel();
                case PanelType.Lights:
                    return CreateLightsPanel();
                case PanelType.Surfaces:
                    return CreateSurfacePanel();
                case PanelType.Pillars:
                    return CreatePillarsPanel();
                case PanelType.Color:
                    return CreateColorsPanel();
                case PanelType.Catenary:
                    return CreateCatenaryPanel();
                case PanelType.Extras:
                    // TODO extras panel
                default:
                    return null;
            }
        }
        
        private PanelBase CreateTreesPanel() {
            var treesPanel = AddUIComponent<TreePanel>();
            treesPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return treesPanel;
        }

        private PanelBase CreateLightsPanel() {
            var lightsPanel = AddUIComponent<LightPanel>();
            lightsPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return lightsPanel;
        }

        private PanelBase CreateSurfacePanel() {
            var surfacePanel = AddUIComponent<SurfacePanel>();
            surfacePanel.Build(new Layout(new Vector2(388.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return surfacePanel;
        }

        private PanelBase CreatePillarsPanel() {
            var pillarPanel = AddUIComponent<PillarPanel>();
            pillarPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return pillarPanel;
        }

        private PanelBase CreateColorsPanel()
        {
            var colorPanel = AddUIComponent<ColorPanel>();
            colorPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return colorPanel;
        }

        private PanelBase CreateCatenaryPanel() {
            var catenaryPanel = AddUIComponent<CatenaryPanel>();
            catenaryPanel.Build(new Layout(new Vector2(400.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 5, "GenericPanel"));
            return catenaryPanel;
        }
        #endregion
    }
}
