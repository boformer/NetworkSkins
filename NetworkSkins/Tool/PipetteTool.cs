using ColossalFramework;
using ColossalFramework.UI;
using NetworkSkins.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetworkSkins.GUI;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Tool
{
    public class PipetteTool : ToolBase
    {
        public delegate void NetInfoPipettedEventHandler(NetInfo info);
        public event NetInfoPipettedEventHandler EventNetInfoPipetted;
        private CursorInfo CursorInfo { get; set; }
        private Dictionary<string, UIComponent> _componentCache = new Dictionary<string, UIComponent>();

        private UIButton TSCloseButton {
            get {
                if (!_componentCache.TryGetValue("TSCloseButton", out UIComponent button))
                    button = UIView.Find("TSCloseButton");
                _componentCache["TSCloseButton"] = button;
                return button as UIButton;
            }
        }

        protected override void Awake() {
            base.Awake();
            enabled = false;
            CursorInfo = ScriptableObject.CreateInstance<CursorInfo>();
            CursorInfo.m_texture = TextureUtil.GetSpriteTexture(Resources.Atlas, Resources.PipetteCursor);
            CursorInfo.m_hotspot = new Vector2(5f, 0f);
            FieldInfo fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            ToolBase[] tools = (ToolBase[])fieldInfo.GetValue(ToolsModifierControl.toolController);
            int initialLength = tools.Length;
            Array.Resize(ref tools, initialLength + 1);
            Dictionary<Type, ToolBase> dictionary = (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            dictionary.Add(typeof(PipetteTool), this);
            tools[initialLength] = this;
            fieldInfo.SetValue(ToolsModifierControl.toolController, tools);
            ToolsModifierControl.SetTool<DefaultTool>();
        }

        public static void Destroy() {
            Destroy(ToolsModifierControl.toolController.gameObject.GetComponent<PipetteTool>());
        }

        protected override void OnDestroy() {
            try
            {
                FieldInfo fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
                List<ToolBase> tools = ((ToolBase[])fieldInfo.GetValue(ToolsModifierControl.toolController)).ToList();
                tools.Remove(this);
                fieldInfo.SetValue(ToolsModifierControl.toolController, tools.ToArray());
                Dictionary<Type, ToolBase> dictionary = (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                dictionary.Remove(typeof(PipetteTool));
            } catch (Exception exception) {
                Debug.LogWarning(exception);
            }
            base.OnDestroy();
        }

        protected override void OnEnable() {
            base.OnEnable();
            this.m_toolController.ClearColliding();
            ToolCursor = CursorInfo;
        }

        protected override void OnDisable() {
            base.OnDisable();
        }

        private void ApplyTool() {
            if (HoveredSegmentId != 0)
            {
                NetInfo info = NetManager.instance.m_segments.m_buffer[HoveredSegmentId].Info;
                var modifiers = NetworkSkinManager.instance.GetModifiersForSegment(HoveredSegmentId);
                info = NetUtils.FindDefaultElevation(info);
                ShowInPanel(info, modifiers);
            }
        }

        private void ShowInPanel(NetInfo info, List<NetworkSkinModifier> modifiers) {
            UIButton networkButton = FindComponentCached<UIButton>(info.name);
            if (networkButton != null) {
                TSCloseButton.SimulateClick();

                // apply the skin data in the tool window
                NetworkSkinPanelController.Instance.OnPrefabWithModifiersSelected(info, modifiers);

                UITabstrip subMenuTabstrip = null;
                UIScrollablePanel scrollablePanel = null;
                UIComponent current = networkButton, parent = networkButton.parent;
                int subMenuTabstripIndex = -1, menuTabstripIndex = -1;
                while (parent != null) {
                    if (current.name == "ScrollablePanel") {
                        subMenuTabstripIndex = parent.zOrder;
                        scrollablePanel = current as UIScrollablePanel;
                    }
                    if (current.name == "GTSContainer")
                    {
                        menuTabstripIndex = parent.zOrder;
                        subMenuTabstrip = parent.Find<UITabstrip>("GroupToolstrip");
                    }
                    current = parent;
                    parent = parent.parent;
                }
                UITabstrip menuTabstrip = current.Find<UITabstrip>("MainToolstrip");
                if (scrollablePanel == null
                || subMenuTabstrip == null
                || menuTabstrip == null
                || menuTabstripIndex == -1
                || subMenuTabstripIndex == -1) return;
                menuTabstrip.selectedIndex = menuTabstripIndex;
                menuTabstrip.ShowTab(menuTabstrip.tabs[menuTabstripIndex].name);
                subMenuTabstrip.selectedIndex = subMenuTabstripIndex;
                subMenuTabstrip.ShowTab(subMenuTabstrip.tabs[subMenuTabstripIndex].name);

                // Clear filters
                UIPanel filterPanel = scrollablePanel.parent.Find<UIPanel>("FilterPanel");
                if (filterPanel != null)
                {
                    foreach (UIMultiStateButton c in filterPanel.GetComponentsInChildren<UIMultiStateButton>())
                    {
                        if (c.isVisible && c.activeStateIndex == 1)
                        {
                            c.activeStateIndex = 0;
                        }
                    }
                }

                StartCoroutine(DoClick(scrollablePanel, networkButton));
            }
            SimulationManager.instance.AddAction(() => EventNetInfoPipetted?.Invoke(info));
        }

        private IEnumerator<object> DoClick(UIScrollablePanel scrollablePanel, UIButton networkButton)
        {
            yield return new WaitForSeconds(0.02f);

            networkButton.SimulateClick();
            scrollablePanel.ScrollIntoView(networkButton);
        }

        private T FindComponentCached<T>(string name) where T : UIComponent {
            if (!_componentCache.TryGetValue(name, out UIComponent component) || component == null) {
                component = UIView.Find<UIButton>(name);
                _componentCache[name] = component;
            }
            return component as T;
        }

        protected override void OnToolUpdate()
        {
            DetermineHoveredElements();
            ToolCursor = HoveredSegmentId != 0 ? CursorInfo : null;
            if (HoveredSegmentId != 0 && Input.GetMouseButtonDown(0)) {
                ApplyTool();
            }

        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (HoveredSegmentId != 0)
            {
                ushort segment = HoveredSegmentId;
                NetManager netManager = NetManager.instance;
                Color color = GetToolColor(false, false);
                float alpha = 1.0f;
                NetTool.CheckOverlayAlpha(ref netManager.m_segments.m_buffer[HoveredSegmentId], ref alpha);
                color.a *= alpha;

                NetTool.RenderOverlay(
                    cameraInfo,
                    ref netManager.m_segments.m_buffer[HoveredSegmentId],
                    color,
                    color);
            }
            base.RenderOverlay(cameraInfo);
        }

        #region raycast
        private Ray MouseRay => Camera.main.ScreenPointToRay(Input.mousePosition);
        private float MouseRayLength => Camera.main.farClipPlane;
        private Vector3 MouseRayRight => Camera.main.transform.TransformDirection(Vector3.right);
        private bool MouseRayValid => isActiveAndEnabled && Cursor.visible && !m_toolController.IsInsideUI;

        internal const float MAX_HIT_ERROR = 2.5f;
        internal static ushort HoveredNodeId;
        internal static ushort HoveredSegmentId;

        private ushort RayCastNode(out RaycastOutput nodeOutput)
        {
            RaycastInput nodeInput = new RaycastInput(MouseRay, MouseRayLength)
            {
                m_rayRight = MouseRayRight,
                m_netService = {
                        // find road nodes
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreNodeFlags = NetNode.Flags.Untouchable
            };

            if (!RayCast(nodeInput, out nodeOutput))
            {
                // find train nodes
                nodeInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                nodeInput.m_netService.m_subService = ItemClass.SubService.PublicTransportTrain;

                if (!RayCast(nodeInput, out nodeOutput))
                {
                    // find metro nodes
                    nodeInput.m_netService.m_subService =
                        ItemClass.SubService.PublicTransportMetro;

                    if (!RayCast(nodeInput, out nodeOutput))
                    {
                        return 0;
                    }
                }
            }
            return nodeOutput.m_netNode;
        }

        private ushort RayCastSegment(out RaycastOutput segmentOutput)
        {
            RaycastInput segmentInput = new RaycastInput(MouseRay, MouseRayLength)
            {
                m_netService = {
                        // find road segments
                        m_itemLayers = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels,
                        m_service = ItemClass.Service.Road
                    },
                m_ignoreTerrain = true,
                m_ignoreSegmentFlags = NetSegment.Flags.Untouchable
            };

            if (!RayCast(segmentInput, out segmentOutput))
            {
                segmentInput.m_netService.m_service = ItemClass.Service.PublicTransport;
                segmentInput.m_netService.m_subService =
                    ItemClass.SubService.PublicTransportTrain;

                if (!RayCast(segmentInput, out segmentOutput))
                {
                    // find metro segments
                    segmentInput.m_netService.m_subService =
                        ItemClass.SubService.PublicTransportMetro;

                    if (!RayCast(segmentInput, out segmentOutput))
                    {
                        return 0;
                    }
                }
            }
            return segmentOutput.m_netSegment;
        }

        private bool DetermineHoveredElements()
        {
            HoveredSegmentId = 0;
            HoveredNodeId = 0;

            RaycastOutput raycastOutput;
            if (MouseRayValid)
            {
                // TODO: why level crossings do not give a node hit?
                HoveredNodeId = RayCastNode(out raycastOutput);
                if (HoveredNodeId != 0)
                {
                    HoveredSegmentId = GetHoveredSegmentFromNode(raycastOutput.m_hitPos);
                }
                else if ((HoveredSegmentId = RayCastSegment(out raycastOutput)) != 0)
                {
                    HoveredNodeId = GetHoveredNodeFromSegment(raycastOutput.m_hitPos);
                }
                if (HoveredNodeId != 0)
                {
                    // to increase accuracy around nodes.
                    HoveredSegmentId = GetHoveredSegmentFromNode(raycastOutput.m_hitPos);
                }
            }

            return HoveredNodeId != 0 || HoveredSegmentId != 0;
        }

        /// <summary>
        /// returns the segment connected to HoveredNodeId that is closest to the input position.
        /// </summary>
        internal ushort GetHoveredSegmentFromNode(Vector3 hitPos)
        {
            ushort minSegId = 0;
            NetManager netManager = NetManager.instance;
            NetNode node = netManager.m_nodes.m_buffer[HoveredNodeId];
            float minDistance = float.MaxValue;

            for (int i = 0; i < 8; ++i)
            {
                ushort segmentId = node.GetSegment(i);
                Vector3 pos = netManager.m_segments.m_buffer[segmentId].GetClosestPosition(hitPos);
                float distance = (hitPos - pos).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minSegId = segmentId;
                }
            };
            return minSegId;
        }

        /// <summary>
        /// checks distance to start and end nodes of HoveredSegmentId to get node.
        /// </summary>
        internal ushort GetHoveredNodeFromSegment(Vector3 hitPos)
        {
            // alternative way to get a node hit: check distance to start and end nodes
            // of the segment
            NetManager netManager = NetManager.instance;
            ushort startNodeId = netManager.m_segments.m_buffer[HoveredSegmentId].m_startNode;
            ushort endNodeId = netManager.m_segments.m_buffer[HoveredSegmentId].m_endNode;

            NetNode[] nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            float startDist = (hitPos - nodesBuffer[startNodeId]
                                                        .m_position).magnitude;
            float endDist = (hitPos - nodesBuffer[endNodeId]
                                                      .m_position).magnitude;
            if (startDist < endDist && startDist < 75f)
            {
                return startNodeId;
            }
            else if (endDist < startDist && endDist < 75f)
            {
                return endNodeId;
            }
            return 0;
        }

        #endregion
    }
}
