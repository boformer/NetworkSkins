using System;
using System.Collections.Generic;
using System.Globalization;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class TreeFeatureController : ItemListFeatureController<TreeInfo>
    {
        public readonly LanePosition Position;

        public float DefaultRepeatDistance { get; private set; }

        public float SelectedRepeatDistance { get; private set; }
        
        public TreeFeatureController(LanePosition position)
        {
            Position = position;
        }

        public void OnRepeatDistanceChanged(float repeatDistance)
        {
            SelectedRepeatDistance = repeatDistance;

            SaveSelectedRepeatDistance();

            OnChanged();
        }

        protected override void Build()
        {
            base.Build();

            DefaultRepeatDistance = TreeUtils.GetDefaultRepeatDistance(Prefab, Position);
            SelectedRepeatDistance = LoadSelectedRepeatDistance() ?? DefaultRepeatDistance;
        }

        protected override List<Item> BuildItems(out Item defaultItem)
        {
            defaultItem = null;

            var defaultTree = TreeUtils.GetDefaultTree(Prefab, Position);
            if (defaultTree == null)
            {
                return new List<Item>();
            }

            var trees = new List<TreeInfo>();

            var prefabCount = PrefabCollection<TreeInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                trees.Add(PrefabCollection<TreeInfo>.GetLoaded(prefabIndex));
            }

            trees.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

            var items = new List<Item> { new NoneItem() };
            foreach (var tree in trees)
            {
                var item = new SimpleItem(tree.name, tree);
                items.Add(item);
                if (tree == defaultTree) defaultItem = item;
            }

            Debug.Log($"Built {items.Count} tree items with default {defaultTree} in lane {Position}");

            return items;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            if (SelectedItem == null || SelectedItem == DefaultItem)
            {
                return new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            }

            return new Dictionary<NetInfo, List<NetworkSkinModifier>>
            {
                {
                    Prefab, new List<NetworkSkinModifier>
                    {
                        new TreeModifier(Position, SelectedItem.GetValue(Prefab), SelectedRepeatDistance)
                    }
                }
            };
        }

        #region Active Selection Data
        protected override string SelectedItemKey => $"Tree_{Position}";

        private string SelectedRepeatDistanceKey => $"Tree_{Position}_RepeatDistance";

        private float? LoadSelectedRepeatDistance()
        {
            var value = ActiveSelectionData.Instance.GetValue(Prefab, SelectedRepeatDistanceKey);
            if (value == null) return null;

            try
            {
                return float.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private void SaveSelectedRepeatDistance()
        {
            if (SelectedRepeatDistance != DefaultRepeatDistance)
            {
                var value = SelectedRepeatDistance.ToString("R", CultureInfo.InvariantCulture);
                ActiveSelectionData.Instance.SetValue(Prefab, SelectedRepeatDistanceKey, value);
            }
            else
            {
                ActiveSelectionData.Instance.ClearValue(Prefab, SelectedRepeatDistanceKey);
            }
        }
        #endregion
    }
}
