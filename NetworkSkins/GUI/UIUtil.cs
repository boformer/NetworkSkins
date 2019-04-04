namespace NetworkSkins.GUI
{
    public class UIUtil
    {
        public static ItemType PanelToItemType(PanelType panelType) {
            switch (panelType) {
                case PanelType.Trees: return ItemType.Trees;
                case PanelType.Lights: return ItemType.Lights;
                case PanelType.Surfaces: return ItemType.Surfaces;
                case PanelType.Pillars: return ItemType.Pillars;
                case PanelType.Color: return ItemType.Colors;
                case PanelType.Catenary: return ItemType.Catenary;
                default: return ItemType.None;
            }
        }
    }
}
