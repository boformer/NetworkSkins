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
using ColossalFramework.Threading;

namespace NetworkSkins.Tool
{
    public class PipetteTool : ToolBase
    {
        public delegate void NetInfoPipettedEventHandler(NetInfo info);
        public event NetInfoPipettedEventHandler EventNetInfoPipetted;
        private Ray MouseRay { get; set; }
        private float MouseRayLength { get; set; }
        private Vector3 MouseRayRight { get; set; }
        private bool MouseLeftDown { get; set; }
        private InstanceID HoverInstance { get; set; }
        private InstanceID HoverInstance2 { get; set; }
        private bool MouseRightDown { get; set; }
        private int SubHoverIndex { get; set; }
        private RaycastOutput RayOutput;
        private bool MouseRayValid { get; set; }
        private CursorInfo CursorInfo { get; set; }
        private bool Active { get; set; }

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
            CursorInfo.m_texture = TextureUtil.GetSpriteTexture(Sprites.Atlas, Sprites.PipetteCursor);
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
            try {
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
            Active = MouseLeftDown = MouseRightDown = MouseRayValid = false;
        }

        public override void SimulationStep() {
            ToolBase.RaycastInput input = new ToolBase.RaycastInput {
                m_ray = MouseRay,
                m_length = MouseRayLength,
                m_rayRight = MouseRayRight
            };

            if (MouseRayValid && ToolBase.RayCast(input, out RayOutput)) {
                InstanceID instanceID = InstanceID.Empty;
                InstanceID instanceID2 = InstanceID.Empty;
                if (RayOutput.m_netSegment != 0 && !(NetManager.instance.NetAdjust is null)) {
                    int index = NetManager.instance.NetAdjust.CheckHoverSegment(ref RayOutput.m_netSegment, RayOutput.m_hitPos);
                    if (index != 0) RayOutput.m_overlayButtonIndex = index;
                }
                if (RayOutput.m_netSegment != 0) {
                    RayOutput.m_hitPos = NetManager.instance.m_segments.m_buffer[RayOutput.m_netSegment].GetClosestPosition(RayOutput.m_hitPos);
                    NetInfo info = NetManager.instance.m_segments.m_buffer[RayOutput.m_netSegment].Info;
                    if (NetUtils.IsValidNet(info)) {
                        instanceID.NetSegment = RayOutput.m_netSegment;
                        ushort segment2 = FindSecondarySegment(RayOutput.m_netSegment);
                        if (segment2 != 0) {
                            instanceID2.NetSegment = segment2;
                        }
                    }
                }

                SetHoverInstances(instanceID, instanceID2);

                SubHoverIndex = RayOutput.m_overlayButtonIndex;
                if (MouseLeftDown != MouseRightDown) {
                    ApplyTool();
                }
            }
        }

        public static ushort FindSecondarySegment(ushort segment) {
            if (segment == 0) return 0;
            NetManager netManager = NetManager.instance;
            ushort node = netManager.m_segments.m_buffer[segment].m_startNode;
            if ((netManager.m_nodes.m_buffer[node].m_flags & NetNode.Flags.Double) == 0) {
                node = netManager.m_segments.m_buffer[segment].m_endNode;
                if ((netManager.m_nodes.m_buffer[node].m_flags & NetNode.Flags.Double) == 0) {
                    return 0;
                }
            }
            for (int i = 0; i < 8; ++i) {
                ushort segment2 = netManager.m_nodes.m_buffer[node].GetSegment(i);
                if (segment2 != 0 && segment2 != segment) {
                    return segment2;
                }
            }
            return 0;
        }

        private void ApplyTool() {
            if (MouseLeftDown) {
                if (!Active && HoverInstance.NetSegment != 0) {
                    Active = true;
                    NetInfo info = NetManager.instance.m_segments.m_buffer[HoverInstance.NetSegment].Info;

                    var modifiers = NetworkSkinManager.instance.GetModifiersForSegment(HoverInstance.NetSegment);
                    info = NetUtils.FindDefaultElevation(info);
                    ThreadHelper.dispatcher.Dispatch(() => ShowInPanel(info, modifiers));
                }
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
                    if (current.name == "GTSContainer") {
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
                networkButton.SimulateClick();
                scrollablePanel.ScrollIntoView(networkButton);
            }
            SimulationManager.instance.AddAction(() => EventNetInfoPipetted?.Invoke(info));
        }

        private T FindComponentCached<T>(string name) where T : UIComponent {
            if (!_componentCache.TryGetValue(name, out UIComponent component) || component == null) {
                component = UIView.Find<UIButton>(name);
                _componentCache[name] = component;
            }
            return component as T;
        }

        protected override void OnToolGUI(Event e) {
            if (!this.m_toolController.IsInsideUI && e.type == EventType.MouseDown) {
                if (e.button == 0) {
                    MouseLeftDown = true;
                } else if (e.button == 1) {
                    MouseRightDown = true;
                }
            } else if (e.type == EventType.MouseUp) {
                if (e.button == 0) {
                    Active = MouseLeftDown = false;
                } else if (e.button == 1) {
                    MouseRightDown = false;
                }
            }
        }

        protected override void OnToolLateUpdate() {
            MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            MouseRayLength = Camera.main.farClipPlane;
            MouseRayRight = Camera.main.transform.TransformDirection(Vector3.right);
            MouseRayValid = (!m_toolController.IsInsideUI && Cursor.visible);
        }

        private void SetHoverInstances(InstanceID id1, InstanceID id2) {
            if (id1 != HoverInstance) {
                HoverInstance = id1;
            }
            if (id2 != HoverInstance2) {
                HoverInstance2 = id2;
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
            if (!MouseRayValid) {
                base.RenderOverlay(cameraInfo);
                return;
            }

            switch (HoverInstance.Type) {
                case InstanceType.NetSegment: {
                    ushort segment = HoverInstance.NetSegment;
                    ushort segment2 = HoverInstance2.NetSegment;
                    NetManager netManager = NetManager.instance;
                    Color color = GetToolColor(false, false);
                    float alpha = 1.0f;
                    NetTool.CheckOverlayAlpha(ref netManager.m_segments.m_buffer[segment], ref alpha);
                    if (segment2 != 0) {
                        NetTool.CheckOverlayAlpha(ref netManager.m_segments.m_buffer[segment2], ref alpha);
                    }
                    color.a *= alpha;

                    if (netManager.NetAdjust != null) {
                        if (netManager.NetAdjust.RenderOverlay(cameraInfo, segment, color, SubHoverIndex)) {
                            break;
                        }
                    }

                    NetTool.RenderOverlay(cameraInfo, ref netManager.m_segments.m_buffer[segment], color, color);
                    if (segment2 != 0) {
                        NetTool.RenderOverlay(cameraInfo, ref netManager.m_segments.m_buffer[segment2], color, color);
                    }
                    
                    break;
                }
            }
            base.RenderOverlay(cameraInfo);
        }
    }
}
