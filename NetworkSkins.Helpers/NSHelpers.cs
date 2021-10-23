﻿namespace NetworkSkins.Helpers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NetworkSkins.API;
    using ColossalFramework.Plugins;

    public static class NSHelpers {
        private static List<Action> pendingActions_;

        public static PluginManager.PluginInfo GetNS() {
            return PluginManager.instance.GetPluginsInfo()
                .FirstOrDefault(p => p.userModInstance.GetType().Name == "NetworkSkinsMod");
        }

        public static bool IsNSEnabled() => GetNS()?.isEnabled ?? false;

         public static void DoOnNSEnabled(Action action) {
            if(action is null)
                throw new ArgumentNullException("action");
            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => {
                if(IsNSEnabled()) {
                    action();
                } else {
                    pendingActions_ ??= new List<Action>();
                    pendingActions_.Add(action);
                    PluginManager.instance.eventPluginsStateChanged -= OneventPluginsStateChanged;
                    PluginManager.instance.eventPluginsStateChanged += OneventPluginsStateChanged;
                }
            });
        }

        private static void OneventPluginsStateChanged() {
            if(IsNSEnabled()) {
                foreach(var action in pendingActions_) {
                    action();
                }
                pendingActions_ = null;
            }
        }

        public static object GetSegmentSkinData(this INSImplementation impl, ushort segmentID) {
            return NSAPI.Instance.GetSegmentSkinData(impl.Index, segmentID);
        }

        public static void Register(this INSImplementation impl) =>
            NSAPI.Instance.AddImplementation(impl);

        public static bool Remove(this INSImplementation impl) =>
            NSAPI.Instance.RemoveImplementation(impl);
    }
}
