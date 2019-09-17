using System;
using System.Collections.Generic;


namespace NetworkSkins.Skins.Bundles
{
    [Serializable]
    public class NetTextureSet
    {
        public List<string> RequiredTags;
        public List<string> ForbiddenTags;
        public List<NetTexture> Segments;
        public List<NetTexture> Nodes;
    }
}
