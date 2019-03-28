namespace NetworkSkins.Skins
{
    public class TerrainSurfaceModifier : NetworkSkinModifier
    {
        public readonly NetworkGroundType GroundType;

        public TerrainSurfaceModifier(NetworkGroundType groundType)
        {
            GroundType = groundType;
        }

        public override void Apply(NetworkSkin skin)
        {
            if (GroundType == NetworkGroundType.Pavement)
            {
                skin.m_createPavement = true;
                skin.m_createGravel = false;
                skin.m_createRuining = false;
            }
            else if (GroundType == NetworkGroundType.Gravel)
            {
                skin.m_createPavement = false;
                skin.m_createGravel = true;
                skin.m_createRuining = false;
            }
            else if (GroundType == NetworkGroundType.Ruined)
            {
                skin.m_createPavement = false;
                skin.m_createGravel = false;
                skin.m_createRuining = true;
            }
            else if (GroundType == NetworkGroundType.None)
            {
                skin.m_createPavement = false;
                skin.m_createGravel = false;
                skin.m_createRuining = false;
            }
        }
    }

    public enum NetworkGroundType
    {
        Unchanged = 0,
        Pavement = 1,
        Gravel = 2,
        Ruined = 3,
        /// <summary>
        /// None only works when the network does not clip terrain. The option should be hidden for network which do clip terrain
        /// </summary>
        None = 4
    }
}
