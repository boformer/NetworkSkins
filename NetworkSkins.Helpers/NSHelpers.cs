namespace NetworkSkins.Helpers {
    using System;
    using System.Linq;
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
        public static bool IsNSEnabled() {
            return
                GetSupportedNS() is PluginManager.PluginInfo p &&
                p.isEnabled &&
                HasAPI(); // detect when NS is being disabled
        }


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

        public static bool HasAPI() => GetNSAPI() != null;

        public static object GetNSAPI() {
            var asm = GetSupportedNS()?.userModInstance?.GetType()?.Assembly;
            return asm?.GetType("NetworkSkins.API.NSAPI", throwOnError: false);
        }

        public static object GetPersistency() {
            object nsapi = GetNSAPI();
            return nsapi.GetType()
            .GetProperty("Persistency")
            .GetValue(nsapi, null);
        }
    }
}
