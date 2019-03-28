using System.Reflection;

namespace NetworkSkins.Skins
{
    public class StreetLightModifier : NetworkSkinModifier
    {
        public readonly PropInfo StreetLight;

        public StreetLightModifier(PropInfo streetLight)
        {
            StreetLight = streetLight;
        }

        public override void Apply(NetworkSkin skin)
        {
            var originalLanes = skin.m_lanes;
            if (originalLanes == null) return;

            skin.m_lanes = new NetInfo.Lane[originalLanes.Length];
            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                skin.m_lanes[l] = ReplaceStreetLightsAndTreesOnLane(originalLanes[l]);
            }
        }

        private NetInfo.Lane ReplaceStreetLightsAndTreesOnLane(NetInfo.Lane originalLane)
        {
            var originalLaneProps = originalLane?.m_laneProps;
            if (originalLaneProps == null || originalLaneProps.m_props == null || !HasStreetLightProps(originalLaneProps.m_props))
            {
                return originalLane;
            }

            var replacementLane = new NetInfo.Lane();
            CopyProperties(replacementLane, originalLane);
            replacementLane.m_laneProps = UnityEngine.Object.Instantiate(originalLaneProps);

            for (var p = 0; p < replacementLane.m_laneProps.m_props.Length; p++)
            {
                var originalLaneProp = replacementLane.m_laneProps.m_props[p];

                if (IsStreetLightProp(originalLaneProp?.m_prop))
                {
                    var replacementLaneProp = new NetLaneProps.Prop();
                    CopyProperties(replacementLaneProp, originalLaneProp);

                    replacementLaneProp.m_prop = StreetLight;
                    replacementLaneProp.m_finalProp = StreetLight;

                    replacementLane.m_laneProps.m_props[p] = replacementLaneProp;
                }
            }

            return replacementLane;
        }

        private bool HasStreetLightProps(NetLaneProps.Prop[] laneProps)
        {
            foreach (var laneProp in laneProps)
            {
                if (IsStreetLightProp(laneProp?.m_finalProp))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsStreetLightProp(PropInfo prefab)
        {
            if (prefab == null) return false;

            if (prefab.m_class.m_service == ItemClass.Service.Road ||
                prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane ||
                prefab.name.ToLower().Contains("streetlamp") || prefab.name.ToLower().Contains("streetlight") || prefab.name.ToLower().Contains("lantern"))
            {
                if (prefab.m_effects != null && prefab.m_effects.Length > 0)
                {
                    if (prefab.name.ToLower().Contains("taxiway")) return false;
                    if (prefab.name.ToLower().Contains("runway")) return false;

                    foreach (var effect in prefab.m_effects)
                    {
                        if (effect.m_effect is LightEffect)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static void CopyProperties(object target, object origin)
        {
            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                fieldInfo.SetValue(target, fieldInfo.GetValue(origin));
            }
        }
    }
}
