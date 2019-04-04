using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.Skins.Serialization
{
    public class NetworkSkinLoadErrors
    {
        public Exception Exception { get; private set; }
        public List<string> PrefabErrors { get; private set; } = new List<string>();

        public void MajorException(Exception exception)
        {
            Exception = exception;

            Debug.LogError($"NS: A major exception occured while deserializing skin data!");
            Debug.LogException(exception);
        }

        public void PrefabNotFound(string prefabName)
        {
            PrefabErrors.Add(prefabName);

            Debug.LogError($"NS: Prefab not found: {prefabName}");
        }

        public void MaybeShowErrors()
        {
            if (Exception != null)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Network Skins",
                    "The skin data could not be loaded from the savegame because a major exception occured! " +
                    "Please report this error on the Network Skins workshop page.\n\n" +
                    $"{Exception}",
                    true
                );
            }
            else if (PrefabErrors.Count > 0)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Network Skins",
                    "Some skins could not be restored because the used assets are missing!\n" +
                    "Be aware that if you save the city, the missing assets will no longer " +
                    "be present in the skin data so there is no way to restore those skins\n\n" +
                    "List of missing assets:\n" +
                    $"{string.Join("\n", PrefabErrors.ToArray())}",
                    false
                );
            }
        }
    }
}
