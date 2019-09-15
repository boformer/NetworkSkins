using System.Collections.Generic;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.GUI.Pillars
{
    public class PillarPanelController : ListPanelController<BuildingInfo>
    {
        public readonly PillarType Type;

        private readonly List<Item> _items;

        public PillarPanelController(PillarType type, List<BuildingInfo> availablePillars)
        {
            Type = type;

            _items = new List<Item>
            {
                new SimpleItem("#NONE#", null)
            };

            foreach (var pillar in availablePillars)
            {
                _items.Add(new SimpleItem(pillar.name, pillar));
            }
        }

        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            if (!PillarUtils.SupportsPillars(Prefab, Type))
            {
                return new List<Item>();
            }

            var defaultPillar = PillarUtils.GetDefaultPillar(Prefab, Type);

            foreach (var item in _items)
            {
                if (item is SimpleItem simpleItem && simpleItem.Value == defaultPillar)
                {
                    defaultItem = item;
                    break;
                }
            }

            //Debug.Log($"Built {_items.Count} pillar items with default {defaultPillar} for {Prefab}");

            return _items;
        }

        protected override Item GetSelectedItemFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is PillarModifier pillarModifier && pillarModifier.Type == Type)
                {
                    return FindItemByName(pillarModifier.Pillar?.name ?? "#NONE#");
                }
            }

            return null;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (SelectedItem != DefaultItem && SelectedItem is SimpleItem item)
            {
                modifiers[Prefab] = new List<NetworkSkinModifier>
                {
                    new PillarModifier(Type, item.Value)
                };
            }

            return modifiers;
        }

        protected override string SelectedItemKey => $"Pillar_{Type}";
    }
}
