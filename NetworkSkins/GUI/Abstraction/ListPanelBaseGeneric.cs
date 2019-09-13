using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListPanelBase<TListBase, TPrefabInfo> : ListPanelBase
        where TListBase : ListBase<TPrefabInfo> 
        where TPrefabInfo : PrefabInfo
    {
        protected TListBase list;

        public override void OnDestroy() {
            list.EventItemClick -= OnItemClick;
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            list.EventItemClick += OnItemClick;
        }

        protected override void CreateList() {
            list = AddUIComponent<TListBase>();
            list.Build(PanelType, new Layout(new Vector2(390.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
        }

        protected abstract void OnItemClick(string itemID);
    }
}
