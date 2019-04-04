using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListPanelBase<TListBase, VPrefabInfo> : ListPanelBase
        where TListBase : ListBase<VPrefabInfo> 
        where VPrefabInfo : PrefabInfo
    {
        protected TListBase list;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            list.EventFavouriteChanged += OnFavouriteChanged;
            list.EventSelectedChanged += OnSelectedChanged;
        }

        protected override void CreateList() {
            list = AddUIComponent<TListBase>();
            list.Build(PanelType, new Layout(new Vector2(390.0f, 0.0f), true, LayoutDirection.Vertical, LayoutStart.TopLeft, 0));
        }
    }
}
