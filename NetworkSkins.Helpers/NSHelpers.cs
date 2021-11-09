namespace NetworkSkins.Helpers {
    extern alias NS;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NS.NetworkSkins.API;
    using ColossalFramework.Plugins;
    using ColossalFramework.Threading;

    public static class NSHelpers {
        private static List<Action> pendingActions_;

        /// <summary>
        /// get NS (a version that supports <see cref="INSIntegration>"/> plug-in.
        /// </summary>
        public static PluginManager.PluginInfo GetSupportedNS() {
            return PluginManager.instance.GetPluginsInfo().FirstOrDefault(IsSupportedNS);
        }

        static bool IsSupportedNS(PluginManager.PluginInfo p) {
            if(p == null)
                return false;
            Type type = p.userModInstance.GetType();
            string name = type.Name;
            Version version = type.Assembly.GetName().Version;
            return name == "NetworkSkinsMod" && version >= new Version(1,1,0);
        }

        /// <returns><c>true</c> if NS (a version that supports <see cref="INSIntegration>"/> is enabled</returns>
        public static bool IsNSEnabled() => GetSupportedNS()?.isEnabled ?? false;


        /// <summary>
        /// performs the given action once NS (a version that supports <see cref="INSIntegration>"/> is enabled
        /// use this in OnEnabled in your mod to register your Implementation of INSIntegration
        /// </summary>
        public static void DoOnNSEnabled(Action action) {
            if(action is null)
                throw new ArgumentNullException("action");
            if(Dispatcher.currentSafe == ThreadHelper.dispatcher) {
                InvokeOrPostpone(action);
            } else {
                ThreadHelper.dispatcher.Dispatch(() => InvokeOrPostpone(action));
            }
        }

        private static void InvokeOrPostpone(Action action) {
            if(IsNSEnabled()) {
                action();
            } else {
                pendingActions_ ??= new List<Action>();
                pendingActions_.Add(action);
                PluginManager.instance.eventPluginsStateChanged -= OneventPluginsStateChanged;
                PluginManager.instance.eventPluginsStateChanged += OneventPluginsStateChanged;
                LoadingManager.instance.m_introLoaded -= OneventPluginsStateChanged;
                LoadingManager.instance.m_introLoaded += OneventPluginsStateChanged;
            }
        }

        private static void OneventPluginsStateChanged() {
            if(pendingActions_ != null && IsNSEnabled()) {
                UnityEngine.Debug.Log("NS is enabled. Execute pending actions ...");
                foreach(var action in pendingActions_) {
                    action();
                }
                pendingActions_ = null;
            }
        }

        public static object GetSegmentSkinData(this INSIntegration impl, ushort segmentID) {
            return NSAPI.Instance.GetSegmentSkinData(implIndex: impl.Index, segmentID);
        }

        public static object GetNodeSkinData(this INSIntegration impl, ushort nodeID) {
            return NSAPI.Instance.GetNodeSkinData(implIndex: impl.Index, nodeID);
        }

        /// <summary>
        /// If user changed skin data using UI call this to rebuild skin.
        /// </summary>
        public static void OnControllerChanged(this INSIntegration impl) =>
            NSAPI.Instance.OnControllerChanged(impl.ID);

        public static void Register(this INSIntegration impl) {
            if(NSAPI.Instance == null)
                throw new Exception("NS is not ready yet. Please use DoOnNSEnabled()");
            NSAPI.Instance.AddImplementation(impl);
        }

        public static bool Remove(this INSIntegration impl) =>
            NSAPI.Instance?.RemoveImplementation(impl) ?? false;
    }
}
