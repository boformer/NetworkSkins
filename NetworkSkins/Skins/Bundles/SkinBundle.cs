
using System;
using System.Collections.Generic;

namespace NetworkSkins.Skins.Bundles
{
    [Serializable]
    public class SkinBundle
    {
        public string Name;
        public List<Network> Networks;
        public List<BooleanOption> BooleanOptions;
    }
}
