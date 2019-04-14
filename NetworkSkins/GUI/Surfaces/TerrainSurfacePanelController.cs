using System.Collections.Generic;
using System.Linq;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;

namespace NetworkSkins.GUI.Surfaces
{
    public class TerrainSurfacePanelController : ListPanelController<Surface>
    {
        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            if (!SurfaceUtil.SupportsSurfaces(Prefab))
            {
                return new List<Item>();
            }

            var items = new List<Item>
            {
                new SimpleItem(Surface.Pavement.ToString("G"), Surface.Pavement),
                new SimpleItem(Surface.Gravel.ToString("G"), Surface.Gravel),
                new SimpleItem(Surface.Ruined.ToString("G"), Surface.Ruined),
            };

            if (SurfaceUtil.SupportsNoneSurface(Prefab))
            {
                items.Add(new SimpleItem(Surface.None.ToString("G"), Surface.None));
            }

            var defaultSurface = SurfaceUtil.GetDefaultSurface(Prefab);
            defaultItem = items.FirstOrDefault(item => ((SimpleItem) item).Value == defaultSurface);

            return items;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (SelectedItem != null && SelectedItem is SimpleItem item && item != DefaultItem)
            {
                modifiers[Prefab] = new List<NetworkSkinModifier>
                {
                    new TerrainSurfaceModifier(item.Value)
                };
            }

            return modifiers;
        }

        protected override string SelectedItemKey => "TerrainSurface";
    }
}
