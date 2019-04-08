using System;
//using NetworkSkins.Data;
using UnityEngine;

namespace NetworkSkins.Net
{
    /// <summary>
    /// Position of roadside trees
    /// </summary>
    public enum LanePosition
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public static class LanePositionExtensions
    {
        public const int LanePositionCount = 3;

        public static bool IsCorrectSide(this LanePosition position, float lanePosition) {
            switch (position) {
                case LanePosition.Left: return lanePosition < 0f;
                case LanePosition.Middle: return Mathf.Approximately(lanePosition, 0f);
                case LanePosition.Right: return lanePosition > 0f;
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }
    }
}