﻿using CitiesHarmony.API;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.GUI;
using NetworkSkins.Locale;
using NetworkSkins.Patches;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Object;
using NetworkSkins.API;
using UnityEngine.SceneManagement;

namespace NetworkSkins
{
    public class NetworkSkinsMod : ILoadingExtension, IUserMod
    {
        public string Name => "Network Skins";
        public string Description => Translation.Instance.GetTranslation(TranslationID.MOD_DESCRIPTION);
        
        private NetworkSkinPanel panel;
        private GameObject skinControllerGameObject;
        private GameObject persistenceServiceGameObject;


        #region Lifecycle
        public static bool InGame => (ToolManager.instance.m_properties.m_mode == ItemClass.Availability.Game);
        
        internal static string[] StartupScenes = new[] { "IntroScreen", "IntroScreen2", "Startup", "MainMenu" };
        internal static bool InStartupMenu => StartupScenes.Contains(SceneManager.GetActiveScene().name);

        public static UITextureAtlas defaultAtlas;

        public void OnEnabled()
        {
            NetworkSkinManager.Ensure();

            HarmonyHelper.DoOnHarmonyReady(NetworkSkinsPatcher.Install);

            NSAPI.Enable();

            if(LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
                NSAPI.Instance.OnLevelPreloaded();
                Install();
            }
        }

        public void OnCreated(ILoading loading) {}

        public void OnLevelLoaded(LoadMode mode)
        {
            NetworkSkinManager.instance.OnLevelLoaded();

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
            NSAPI.Instance?.Disable();

            Uninstall();

            if (HarmonyHelper.IsHarmonyInstalled) NetworkSkinsPatcher.Uninstall();

            NetworkSkinManager.Uninstall();
        }
        #endregion

        #region NetToolMonitor/GUI
        private void Install()
        {
            // try to get InGame atlas
            defaultAtlas = InGame ? UIView.GetAView().defaultAtlas : UIView.library?.Get<OptionsMainPanel>("OptionsPanel")?.GetComponent<UIPanel>()?.atlas;
            persistenceServiceGameObject = new GameObject(nameof(PersistenceService));
            skinControllerGameObject = new GameObject(nameof(NetworkSkinPanelController));
            persistenceServiceGameObject.transform.parent = NetworkSkinManager.instance.gameObject.transform;
            skinControllerGameObject.transform.parent = NetworkSkinManager.instance.gameObject.transform;
            PersistenceService.Instance = persistenceServiceGameObject.AddComponent<PersistenceService>();
            NetworkSkinPanelController.Instance = skinControllerGameObject.AddComponent<NetworkSkinPanelController>();
            NetworkSkinPanelController.Instance.EventToolStateChanged += OnNetToolStateChanged;

            foreach(var impl in NSAPI.Instance.ActiveImplementationWrappers) {
                impl.OnAfterNSLoaded();
            }
        }

        private void Uninstall()
        {
            if (NetworkSkinPanelController.Instance != null)
            {
                NetworkSkinPanelController.Instance.EventToolStateChanged -= OnNetToolStateChanged;
                if (skinControllerGameObject != null)
                {
                    Destroy(skinControllerGameObject);
                    skinControllerGameObject = null;
                }
            }

            if (PersistenceService.Instance != null) {
                if (persistenceServiceGameObject != null) {
                    Destroy(persistenceServiceGameObject);
                    persistenceServiceGameObject = null;
                }
            }

            if (panel != null && panel.gameObject != null)
            {
                Destroy(panel.gameObject);
                panel = null;
            }

            defaultAtlas = null;
        }

        private void OnNetToolStateChanged(bool isToolEnabled)
        {
            if (isToolEnabled)
            {
                panel = UIView.GetAView().AddUIComponent(typeof(NetworkSkinPanel)) as NetworkSkinPanel;
            }
            else
            {
                if (panel.gameObject != null)
                {
                    Destroy(panel.gameObject);
                    panel = null;
                }
            }
        }
        #endregion

        public static Type ResolveSerializedType(string type)
        {
            var assemblyName = typeof(NetworkSkinsMod).Assembly.GetName();
            var fixedType = Regex.Replace(type, $@"{assemblyName.Name}, Version=\d+.\d+.\d+.\d+", $"{assemblyName.Name}, Version={assemblyName.Version}");
            return Type.GetType(fixedType);
        }
    }
}
