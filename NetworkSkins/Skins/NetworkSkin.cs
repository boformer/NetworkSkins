
// ReSharper disable InconsistentNaming

using System;
using UnityEngine;

namespace NetworkSkins.Skins
{
    public class NetworkSkin
    {
        public readonly NetInfo Prefab;

        public NetworkSkin(NetInfo prefab)
        {
            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            m_lanes = prefab.m_lanes;
            m_createPavement = prefab.m_createPavement;
            m_createGravel = prefab.m_createGravel;
            m_createRuining = prefab.m_createRuining;
            m_clipTerrain = prefab.m_clipTerrain;
            m_color = prefab.m_color;
        }

        public NetInfo.Lane[] m_lanes;

        public bool m_createPavement;
        public bool m_createGravel;
        public bool m_createRuining;
        public bool m_clipTerrain;

        public Color m_color;

        public override string ToString()
        {
            return $"Skin for {Prefab.name}";
        }
    }
}
