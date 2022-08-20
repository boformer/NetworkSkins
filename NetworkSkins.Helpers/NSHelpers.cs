namespace NetworkSkins.Helpers {
    extern alias NS;
    using System;
    using System.Linq;
    using NS.NetworkSkins.API;
    using ColossalFramework.Plugins;
    using ColossalFramework.Threading;
    using UnityEngine;

    public static class NSHelpers {
        public static event Action EventNSInstalled;

        /// <summary>
        /// get NS (a version that supports <see cref="INSIntegration>"/> plug-in.
        /// </summary>
        public static PluginManager.PluginInfo GetSupportedNS() {
            return PluginManager.instance.GetPluginsInfo().FirstOrDefault(IsSupportedNS);
        }

        static bool IsSupportedNS(PluginManager.PluginInfo p) {
            try {
                if (p == null) {
                    Debug.LogWarning("[NetworkSkins.Helpers] | IsSupportedNS :  null PluginInfo");
                    return false;
                }
                var mod = p.userModInstance;
                if(mod == null) {
                    Debug.LogWarning($"[NetworkSkins.Helpers] | IsSupportedNS :  PluginInfo with no  userModInstance: " + p);
                    return false;
                }

                Type type = mod.GetType();
                string name = type.Name;
                Version version = type.Assembly.GetName().Version;
                return name == "NetworkSkinsMod" && version >= new Version(1, 1, 0);
            } catch (Exception ex) {
                Debug.LogException(ex);
                return false;
            }
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
                EventNSInstalled += action;
                PluginManager.instance.eventPluginsStateChanged -= OneventPluginsStateChanged;
                PluginManager.instance.eventPluginsStateChanged += OneventPluginsStateChanged;
                LoadingManager.instance.m_introLoaded -= OneventPluginsStateChanged;
                LoadingManager.instance.m_introLoaded += OneventPluginsStateChanged;
            }
        }

        private static void OneventPluginsStateChanged() {
            if(EventNSInstalled != null && IsNSEnabled()) {
                UnityEngine.Debug.Log("NS is enabled. Execute pending actions ...");
                EventNSInstalled?.Invoke();
                EventNSInstalled = null;
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
