using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.Net;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListPanelBase<TListBase, VPrefabInfo> : PanelBase 
        where TListBase : ListBase<VPrefabInfo> 
        where VPrefabInfo : PrefabInfo
    {
        protected NetToolMonitor Monitor => NetToolMonitor.Instance;
        protected TListBase list;
        protected UITabstrip tabStrip;
        protected UIButton[] tabs;
        protected SearchBox searchBox;

        public override void Awake() {
            base.Awake();
            Monitor.EventPrefabChanged += OnPrefabChanged;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            Monitor.EventPrefabChanged -= OnPrefabChanged;
        }

        public override void Build(Layout layout) {
            base.Build(layout);
            color = MainPanel.GUIColor;

            CreateTabstrip();
            SetupTabs();
            CreateList();
            CreateSearchBox();
            CreateSpace(width - (Spacing * 2), 0.1f);
            RefreshUI(Monitor.Prefab);
            OnPanelCreated();
        }

        private void CreateTabstrip() {
            tabStrip = AddUIComponent<UITabstrip>();
            tabStrip.builtinKeyNavigation = true;
            tabStrip.size = new Vector2(width - (Spacing * 2.0f), 30.0f);
        }

        private void SetupTabs() {
            tabs = new UIButton[(int)LanePosition.Count];
            for (int i = 0; i < tabs.Length; i++) {
                string lane = string.Empty;
                switch ((LanePosition)i) {
                    case LanePosition.Left: lane = Translation.Instance.GetTranslation(TranslationID.LEFT_LANE); break;
                    case LanePosition.Middle: lane = Translation.Instance.GetTranslation(TranslationID.MIDDLE_LANE); break;
                    case LanePosition.Right: lane = Translation.Instance.GetTranslation(TranslationID.RIGHT_LANE); break;
                    default: break;
                }
                tabs[i] = CreateButton(Vector2.zero, lane, backgroundSprite: "GenericTab", parentComponent: tabStrip, isFocusable: true);
                tabs[i].color = tabs[i].focusedColor = new Color32(210, 210, 210, 255);
                tabs[i].size = new Vector2(tabStrip.width / 3.0f, tabStrip.height);
            }
        }

        private void CreateList() {
            list = AddUIComponent<TListBase>();
            list.Build(new Layout(new Vector2(390.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
        }

        private void CreateSearchBox() {
            //searchBox = AddUIComponent<SearchBox>();
            //searchBox.Build(new Layout(new Vector2(390.0f, 50.0f), true, LayoutDirection.Horizontal, LayoutStart.TopRight, Spacing));
            //searchBox.EventSearchTextChanged += OnSearchTextChanged;
            //searchBox.EventLostFocus += OnSearchLostFocus;
        }

        protected override void RefreshUI(NetInfo netInfo) {
            base.RefreshUI(netInfo);
        }

        /// <summary>
        /// This event is invoked when the NetTool switches to a different NetInfo.
        /// </summary>
        /// <param name="netInfo"></param>
        protected abstract void OnPrefabChanged(NetInfo netInfo);

        protected abstract void OnPanelCreated();

        protected abstract void OnSearchTextChanged(string text);

        protected abstract void OnSearchLostFocus();
    }
}
