using System.Collections.Generic;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins.GUI.Trees
{
    public class TreePanelController : ListPanelController<TreeInfo>
    {
        public readonly LanePosition Position;

        public float DefaultRepeatDistance { get; private set; }

        public float SelectedRepeatDistance { get; private set; }
        
        public TreePanelController(LanePosition position)
        {
            Position = position;
        }

        public void SetRepeatDistance(float repeatDistance)
        {
            SelectedRepeatDistance = repeatDistance;

            SaveSelectedRepeatDistance();

            OnChanged();
        }

        protected override void Build()
        {
            base.Build();

            if (Prefab != null)
            {
                DefaultRepeatDistance = TreeUtils.GetDefaultRepeatDistance(Prefab, Position);
                SelectedRepeatDistance = LoadSelectedRepeatDistance() ?? DefaultRepeatDistance;
            }
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers)
        {
            base.BuildWithModifiers(modifiers);

            if (Prefab != null)
            {
                DefaultRepeatDistance = TreeUtils.GetDefaultRepeatDistance(Prefab, Position);
                SelectedRepeatDistance = GetSelectedRepeatDistanceFromModifiers(modifiers) ?? DefaultRepeatDistance;
            }
            SaveSelectedRepeatDistance();
        }

        protected override List<Item> BuildItems(ref Item defaultItem)
        {
            if (Prefab == null)
            {
                return new List<Item>();
            }

            var defaultTree = TreeUtils.GetDefaultTree(Prefab, Position);
            if (defaultTree == null)
            {
                return new List<Item>();
            }

            var trees = TreeUtils.GetAvailableTrees();

            var items = new List<Item> { new SimpleItem("#NONE#", null) };
            foreach (var tree in trees)
            {
                var item = new SimpleItem(tree.name, tree);
                items.Add(item);

                if (tree == defaultTree)
                {
                    defaultItem = item;
                }
            }

            //Debug.Log($"Built {items.Count} tree items with default {defaultTree} in lane {Position}");

            return items;
        }

        protected override Item GetSelectedItemFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is TreeModifier treeModifier && treeModifier.Position == Position)
                {
                    return FindItemByName(treeModifier.Tree?.name ?? "#NONE#");
                }
            }

            return null;
        }

        private float? GetSelectedRepeatDistanceFromModifiers(List<NetworkSkinModifier> modifiers)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is TreeModifier treeModifier && treeModifier.Position == Position)
                {
                    return treeModifier.RepeatDistance;
                }
            }

            return null;
        }

        protected override Dictionary<NetInfo, List<NetworkSkinModifier>> BuildModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            if (SelectedItem != null && SelectedItem is SimpleItem item)
            {
                if (item != DefaultItem || SelectedRepeatDistance != DefaultRepeatDistance)
                {
                    modifiers[Prefab] = new List<NetworkSkinModifier>()
                    {
                        new TreeModifier(Position, item.Value, SelectedRepeatDistance)
                    };
                }
            }

            return modifiers;
        }

        #region Active Selection Data
        protected override string SelectedItemKey => $"Tree_{Position}";

        private string SelectedRepeatDistanceKey => $"Tree_{Position}_RepeatDistance";

        private float? LoadSelectedRepeatDistance()
        {
            return ActiveSelectionData.Instance.GetFloatValue(Prefab, SelectedRepeatDistanceKey);
        }

        private void SaveSelectedRepeatDistance()
        {
            if (SelectedRepeatDistance != DefaultRepeatDistance)
            {
                ActiveSelectionData.Instance.SetFloatValue(Prefab, SelectedRepeatDistanceKey, SelectedRepeatDistance);
            }
            else
            {
                ActiveSelectionData.Instance.ClearValue(Prefab, SelectedRepeatDistanceKey);
            }
        }
        #endregion
    }
}
