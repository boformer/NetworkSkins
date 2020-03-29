using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Harmony;
using ICities;
using NetworkSkins.GUI;
using NetworkSkins.Locale;
using NetworkSkins.Persistence;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.Object;

namespace NetworkSkins
{
    public class NetworkSkinsMod : ILoadingExtension, IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";

        public string Name => "Network Skins 2";
        public string Description => Translation.Instance.GetTranslation(TranslationID.MOD_DESCRIPTION);
        
        private HarmonyInstance harmony;

        private NetworkSkinPanel panel;
        private GameObject skinControllerGameObject;
        private GameObject persistenceServiceGameObject;


        #region Lifecycle
        public static bool InGame => (ToolManager.instance.m_properties.m_mode == ItemClass.Availability.Game);

        public static UITextureAtlas defaultAtlas;

        public void OnEnabled()
        {
            NetworkSkinManager.Ensure();

            InstallHarmony();

            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
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
            Uninstall();

            UninstallHarmony();

            NetworkSkinManager.Uninstall();
        }
        #endregion

        #region Harmony
        private void InstallHarmony()
        {
            if (harmony == null)
            {
                Debug.Log("NetworkSkins Patching...");

#if DEBUG
                HarmonyInstance.DEBUG = true;
#endif
                HarmonyInstance.SELF_PATCHING = false;
                harmony = HarmonyInstance.Create(HarmonyId);
                harmony.PatchAll(GetType().Assembly);
            }
        }

        private void UninstallHarmony()
        {
            if (harmony != null)
            {
                harmony.UnpatchAll(HarmonyId);
                harmony = null;

                Debug.Log("NetworkSkins Reverted...");
            }
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
