using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.Skins
{
    public class NetworkSkinLoadErrors
    {
        private Exception _exception = null;
        private readonly List<string> _prefabErrors = new List<string>();

        public void MajorException(Exception exception)
        {
            _exception = exception;

            Debug.LogError($"NS: A major exception occured while deserializing skin data!");
            Debug.LogException(exception);
        }

        public void PrefabNotFound(string prefabName)
        {
            _prefabErrors.Add(prefabName);

            Debug.LogError($"NS: Prefab not found: {prefabName}");
        }

        public void MaybeShowErrors()
        {
            if (_exception != null)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Network Skins",
                    "The skin data could not be loaded from the savegame because a major exception occured!" +
                    "Please report this error on the Network Skins workshop page.\n\n" +
                    $"{_exception}",
                    true
                );
            }
            else if (_prefabErrors.Count > 0)
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Network Skins",
                    "Some skins could not be restored because the used assets are missing!\n" +
                    "Be aware that if you save the city, the missing assets will no longer " +
                    "be present in the skin data so there is no way to restore those skins\n\n" +
                    "List of missing assets:\n" +
                    $"{string.Join("\n", _prefabErrors.ToArray())}",
                    false
                );
            }
        }
    }
}
