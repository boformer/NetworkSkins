using System;
using System.Collections.Generic;
using System.Linq;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class CatenaryFeatureController : ItemListFeatureController<PropInfo>
    {
        protected override List<Item> BuildItems(out Item defaultItem)
        {
            defaultItem = null;

            if (!(Prefab.m_netAI is TrainTrackBaseAI))
            {
                return new List<Item>();
            }

            var defaultCatenaries = GetDefaultCatenaries();
            if (defaultCatenaries.Count == 0)
            {
                return new List<Item>();
            }

            var items = new List<Item>()
            {
                new SimpleItem("#NONE#", null)
            };

            PropInfo singleDefaultCatenary = null;
            var uniqueDefaultCatenaries = new HashSet<PropInfo>(defaultCatenaries.Values);
            if (uniqueDefaultCatenaries.Count == 1)
            {
                singleDefaultCatenary = uniqueDefaultCatenaries.First();
            }
            else
            {
                defaultItem = new DefaultVariantItem();
                items.Add(defaultItem);
            }

            var singleTrack = IsSingleTrack(uniqueDefaultCatenaries);

            var catenaries = GetAvailableCatenaries(singleTrack);

            foreach (var catenary in catenaries)
            {
                var item = new SimpleItem(catenary.name, catenary);
                items.Add(item);

                if (catenary == singleDefaultCatenary)
                {
                    defaultItem = item;
                }
            }

            Debug.Log($"Built {items.Count} catenary items with default {singleDefaultCatenary}");

            return items;
        }

        private Dictionary<NetInfo, PropInfo> GetDefaultCatenaries()
        {
            var defaultCatenaries = new Dictionary<NetInfo, PropInfo>();

            var groundCatenary = CatenaryUtils.GetDefaultCatenary(Prefab);
            if (groundCatenary != null) defaultCatenaries[Prefab] = groundCatenary;

            var slopePrefab = NetUtil.GetSlopePrefab(Prefab);
            if (slopePrefab != null)
            {
                var slopeCatenary = CatenaryUtils.GetDefaultCatenary(slopePrefab);
                if (slopeCatenary != null) defaultCatenaries[slopePrefab] = slopeCatenary;
            }

            var elevatedPrefab = NetUtil.GetElevatedPrefab(Prefab);
            if (elevatedPrefab != null)
            {
                var elevatedCatenary = CatenaryUtils.GetDefaultCatenary(elevatedPrefab);
                if (elevatedCatenary != null) defaultCatenaries[elevatedPrefab] = elevatedCatenary;
            }

            var bridgePrefab = NetUtil.GetBridgePrefab(Prefab);
            if (bridgePrefab != null)
            {
                var bridgeCatenary = CatenaryUtils.GetDefaultCatenary(bridgePrefab);
                if (bridgeCatenary != null) defaultCatenaries[bridgePrefab] = bridgeCatenary;
            }

            return defaultCatenaries;
        }

        private static bool IsSingleTrack(ICollection<PropInfo> defaultCatenaries)
        {
            return defaultCatenaries.Any(CatenaryUtils.IsSingleRailCatenaryProp);
        }

        private List<PropInfo> GetAvailableCatenaries(bool singleTrack)
        {
            var catenaries = new List<PropInfo>();

            var prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (singleTrack)
                {
                    if (CatenaryUtils.IsSingleRailCatenaryProp(prefab))
                    {
                        catenaries.Add(prefab);
                    }
                }
                else
                {
                    if (CatenaryUtils.IsDoubleRailCatenaryProp(prefab))
                    {
                        catenaries.Add(prefab);
                    }
                }
            }

            catenaries.Sort((a, b) => string.Compare(a.GetUncheckedLocalizedTitle(), b.GetUncheckedLocalizedTitle(), StringComparison.Ordinal));

            return catenaries;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (Prefab != null && SelectedItem != null && SelectedItem is SimpleItem item)
            {
                if (item != DefaultItem)
                {
                    var prefabModifiers = new List<NetworkSkinModifier>
                    {
                        new CatenaryModifier(item.Value)
                    };

                    modifiers.Add(Prefab, prefabModifiers);

                    var slopePrefab = NetUtil.GetSlopePrefab(Prefab);
                    if (slopePrefab != null) modifiers.Add(slopePrefab, prefabModifiers);

                    var elevatedPrefab = NetUtil.GetElevatedPrefab(Prefab);
                    if (elevatedPrefab != null) modifiers.Add(elevatedPrefab, prefabModifiers);

                    var bridgePrefab = NetUtil.GetBridgePrefab(Prefab);
                    if (bridgePrefab != null) modifiers.Add(bridgePrefab, prefabModifiers);
                }
            }

            return modifiers;
        }

        protected override string SelectedItemKey => $"Catenary";
    }
}
