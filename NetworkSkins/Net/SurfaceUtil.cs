using NetworkSkins.Skins.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public class SurfaceUtil
    {
        public static Surface StringToSurface(string name) {
            switch (name) {
                case "None": return Surface.None;
                case "Pavement": return Surface.Pavement;
                case "Gravel": return Surface.Gravel;
                case "Ruined": return Surface.Ruined;
                default: return Surface.Unchanged;
            }
        }
    }
}
