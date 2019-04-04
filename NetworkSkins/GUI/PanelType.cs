using System;

namespace NetworkSkins.GUI
{
    [Flags]
    public enum PanelType
    {
        None = 0,
        Trees = 1,
        Lights = 2,
        Surfaces = 4,
        Pillars = 8,
        Color = 16,
        Catenary = 32,
        Extras = 64
    }

    public enum ItemType
    {
        None = -1,
        Trees,
        Lights,
        Surfaces,
        Pillars,
        Colors,
        Catenary,
        Count
    }
}
