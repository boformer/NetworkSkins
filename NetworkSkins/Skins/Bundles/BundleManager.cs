using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkSkins.Skins.Bundles
{
    public class BundleManager : MonoBehaviour
    {
        private static BundleManager _instance;
        public static BundleManager instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<BundleManager>();
                    if (_instance == null) {
                        var gameObject = new GameObject(nameof(BundleManager));
                        _instance = gameObject.AddComponent<BundleManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public static BundleManager Ensure() => instance;

        public static void Uninstall() {
            if (_instance != null) {
                Destroy(_instance.gameObject);
            }
        }

        public SkinBundle MakeDefaultBundle() {

            var crossingsOption = new BooleanOption() {
                Label = "Crossings",
                Tag = "crossings",
                Default = true
            };

            var linesOption = new BooleanOption() {
                Label = "Lines",
                Tag = "lines",
                Default = true
            };

            var optionsList = new List<BooleanOption>() {
                linesOption,
                crossingsOption
            };

            var segment = new NetTexture() {
                Index = 0,
                Diffuse = "basic-road.png",
                LodDiffuse = "basic-road-lod.png",
                Xys = "basic-road-xys.png",
                LodXys = "basic-road-xys-lod"
            };

            var elevatedSegment = new NetTexture() {
                Index = 0,
                Diffuse = "basic-road-elevated.png",
                LodDiffuse = "basic-road-elevated-lod.png"
            };

            var slopeSegment = new NetTexture() {
                Index = 0,
                Diffuse = "basic-road-slope.png",
                LodDiffuse = "basic-road-slope-lod.png"
            };

            var slopeNode = new NetTexture() {
                Index = 0,
                Diffuse = "basic-road-slope-node.png",
                LodDiffuse = "basic-road-slope-node-lod.png"
            };

            var crossingsSet = new NetTextureSet() {
                RequiredTags = new List<string> { "crossings" },
                Nodes = new List<NetTexture>() { segment }
            };

            var linesSet = new NetTextureSet() {
                RequiredTags = new List<string> { "lines" },
                Segments = new List<NetTexture>() { segment }
            };

            var crossingsSetElevated = new NetTextureSet() {
                RequiredTags = new List<string> { "crossings" },
                Nodes = new List<NetTexture>() { elevatedSegment }
            };

            var linesSetElevated = new NetTextureSet() {
                RequiredTags = new List<string> { "lines" },
                Segments = new List<NetTexture>() { elevatedSegment }
            };

            var crossingsSetSlope = new NetTextureSet() {
                RequiredTags = new List<string> { "crossings" },
                Nodes = new List<NetTexture>() { slopeNode }
            };

            var linesSetSlope = new NetTextureSet() {
                RequiredTags = new List<string> { "lines" },
                Segments = new List<NetTexture>() { slopeSegment }
            };

            var basicRoad = new Network() {
                Name = "Basic Road",
                TextureSets = new List<NetTextureSet>() { crossingsSet, linesSet }
            };

            var basicRoadElevated = new Network() {
                Name = "Basic Road Elevated",
                TextureSets = new List<NetTextureSet>() { crossingsSetElevated, linesSetElevated }
            };

            var basicRoadBridge = new Network() {
                Name = "Basic Road Bridge",
                TextureSets = new List<NetTextureSet>() { crossingsSetElevated, linesSetElevated }
            };

            var basicRoadSlope = new Network() {
                Name = "Basic Road Slope",
                TextureSets = new List<NetTextureSet>() { crossingsSetSlope, linesSetSlope }
            };

            var networks = new List<Network>() {
                basicRoad,
                basicRoadElevated,
                basicRoadBridge,
                basicRoadSlope
            };

            return new SkinBundle() {
                Name = "NS2 Bundle",
                BooleanOptions = optionsList,
                Networks = networks
            };
        }
    }
}
