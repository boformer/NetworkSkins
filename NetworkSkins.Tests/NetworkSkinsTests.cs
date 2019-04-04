using ICities;
using System;
using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.Tests
{
    public class NetworkSkinsTests : IUserMod
    {
        public string Name => "NetworkSkinsTests";
        public string Description => "Integration Tests for Network Skins. Enable to run!";

        public void OnEnabled()
        {
            try
            {
                new SkinSerializationTest().TestSerialization();
                Debug.Log("Tests Successful!");
            }
            catch (Exception e)
            {
                Debug.LogError("Tests Failed!");
                throw e;
            }
        }
    }
}