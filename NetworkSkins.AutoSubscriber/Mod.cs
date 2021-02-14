using System;
using ColossalFramework.PlatformServices;
using ColossalFramework.Plugins;
using ICities;

namespace NetworkSkins.AutoSubscriber {
    public class Mod : IUserMod {
        internal const ulong NetworkSkinsWorkshopId = 1758376843uL;
        internal const ulong LegacyWorkshopId = 543722850uL;

        public string Name => "Legacy Network Skins Mod (PLEASE UNSUBSCRIBE)";
        public string Description => "New version at steamcommunity.com/sharedfiles/filedetails/?id=1758376843";

        static Mod() {
            LoadingManager.instance.m_introLoaded += RefreshNetworkSkinsSubscription;
        }

        private static void RefreshNetworkSkinsSubscription() {
            try {
                LoadingManager.instance.m_introLoaded -= RefreshNetworkSkinsSubscription;
                if (PlatformService.platformType != PlatformType.Steam || PluginManager.noWorkshop ||
                    !PlatformService.workshop.IsAvailable()) {
                    UnityEngine.Debug.Log(
                        "[NetworkSkins.AutoSubscriber] Workshop not available, unable to subscribe to NetworkSkins!");
                    return;
                }

                bool wasEnabled = false;
                foreach (var pluginInfo in PluginManager.instance.GetPluginsInfo()) {
                    if (pluginInfo.publishedFileID.AsUInt64 == LegacyWorkshopId) {
                        wasEnabled = pluginInfo.isEnabled;
                        pluginInfo.isEnabled = false;
                        break;
                    }
                }

                UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Unsubscribing Legacy NetworkSkins");
                if (PlatformService.workshop.Unsubscribe(new PublishedFileId(LegacyWorkshopId))) {
                    UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Success!");

                    UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Subscribing NetworkSkins");
                    if (PlatformService.workshop.Subscribe(new PublishedFileId(NetworkSkinsWorkshopId))) {
                        UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Success!");
                        if (wasEnabled) {
                            foreach (var pluginInfo in PluginManager.instance.GetPluginsInfo()) {
                                if (pluginInfo.publishedFileID.AsUInt64 == NetworkSkinsWorkshopId) {
                                    UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Enabling NetworkSkins!");
                                    pluginInfo.isEnabled = true;
                                    break;
                                }
                            }
                        }
                    } else {
                        UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Unable to subscribe NetworkSkins!");
                    }
                } else {
                    UnityEngine.Debug.Log("[NetworkSkins.AutoSubscriber] Unable to unsubscribe Legacy NetworkSkins!");
                }
            } catch (Exception e) {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}
