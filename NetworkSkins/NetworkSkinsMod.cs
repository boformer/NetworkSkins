using ColossalFramework.UI;
using Harmony;
using ICities;
using NetworkSkins.GUI;
using NetworkSkins.Locale;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using UnityEngine;
using static UnityEngine.Object;

namespace NetworkSkins
{
    // TODO ListBase.cs -> IsSelected, IsFavourite, OnSelectedChanged, OnFavouriteChanged, and SearchBox events implementation
    // TODO NetToolMonitor.cs -> NetInfoDefaultEquals
    public class NetworkSkinsMod : ILoadingExtension, IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";

        public string Name => "Network Skins";
        public string Description => Translation.Instance.GetTranslation(TranslationID.MOD_DESCRIPTION);
        
        private HarmonyInstance _harmony;

        private MainPanel _panel;
        private GameObject _monitorGameObject;

        #region Lifecycle
        private static bool InGame => LoadingManager.exists && LoadingManager.instance.m_loadingComplete;

        public void OnEnabled()
        {
            NetworkSkinManager.Ensure();

            //InstallHarmony();

            if (InGame)
            {
                Install();
            }
        }

        public void OnCreated(ILoading loading) {}

        public void OnLevelLoaded(LoadMode mode)
        {
            NetworkSkinManager.instance.OnLevelLoaded();

            // TODO seems fishy
            while (!InGame) { }
            Install();
        }

        public void OnLevelUnloading()
        {
            Uninstall();

            NetworkSkinManager.instance.OnLevelUnloading();
        }

        public void OnReleased() {}

        public void OnDisabled()
        {
            Uninstall();

            UninstallHarmony();

            Destroy(NetworkSkinManager.instance.gameObject);
        }
        #endregion

        #region Harmony
        private void InstallHarmony()
        {
            if (_harmony == null)
            {
                Debug.Log("NetworkSkins Patching...");

                //HarmonyInstance.SELF_PATCHING = false;
                //HarmonyInstance.DEBUG = true;

                _harmony = HarmonyInstance.Create(HarmonyId);
                _harmony.PatchAll(GetType().Assembly);
            }
        }

        private void UninstallHarmony()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;

                Debug.Log("NetworkSkins Reverted...");
            }
        }
        #endregion

        #region NetToolMonitor/GUI
        private void Install()
        {
            _monitorGameObject = new GameObject(nameof(NetToolMonitor));
            NetToolMonitor.Instance = _monitorGameObject.AddComponent<NetToolMonitor>();
            NetToolMonitor.Instance.EventToolStateChanged += OnNetToolStateChanged;
        }

        private void Uninstall()
        {
            if (NetToolMonitor.Instance != null)
            {
                NetToolMonitor.Instance.EventToolStateChanged -= OnNetToolStateChanged;
                if (_monitorGameObject != null)
                {
                    Destroy(_monitorGameObject);
                    _monitorGameObject = null;
                }
            }

            if (_panel != null && _panel.gameObject != null)
            {
                Destroy(_panel.gameObject);
                _panel = null;
            }
        }

        private void OnNetToolStateChanged(bool isToolEnabled)
        {
            if (isToolEnabled)
            {
                _panel = UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;
            }
            else
            {
                if (_panel.gameObject != null)
                {
                    Destroy(_panel.gameObject);
                    _panel = null;
                }
            }
        }
        #endregion
    }
}
