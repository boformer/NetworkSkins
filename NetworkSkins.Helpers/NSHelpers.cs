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

        public static PluginManager.PluginInfo GetNS() {
            return PluginManager.instance.GetPluginsInfo()
                .FirstOrDefault(p => p.userModInstance.GetType().Name == "NetworkSkinsMod");
        }

        public static bool IsNSEnabled() => GetNS()?.isEnabled ?? false;

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

        public static object GetSegmentSkinData(this INSImplementation impl, ushort segmentID) {
            return NSAPI.Instance.GetSegmentSkinData(implIndex: impl.Index, segmentID);
        }

        /// <summary>
        /// If user changed skin data using UI call this to rebuild skin.
        /// </summary>
        public static void OnControllerChanged(this INSImplementation impl) =>
            NSAPI.Instance.OnControllerChanged(impl.ID);

        public static void Register(this INSImplementation impl) {
            if(NSAPI.Instance == null)
                throw new Exception("NS is not ready yet. Please use DoOnNSEnabled()");
            NSAPI.Instance.AddImplementation(impl);
        }

        public static bool Remove(this INSImplementation impl) =>
            NSAPI.Instance?.RemoveImplementation(impl) ?? false;
    }
}
