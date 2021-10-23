using ColossalFramework.UI;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Locale;
using NetworkSkins.API;
using NetworkSkins.TranslationFramework;
using UnityEngine;
using System;

namespace NetworkSkins.GUI.Custom
{
    public class CustomPanel : PanelBase
    {
        public NSImplementationWrapper Implementation;
        public override void Build(PanelType panelType, Layout layout)
        {
            throw new NotImplementedException("TODO: call build from implemetation"); // add UI Items.

            base.Build(panelType, layout);
            autoFitChildrenHorizontally = true;
        }

        protected override void RefreshUI(NetInfo netInfo)
        {
            throw new NotImplementedException("TODO call refresh UI from implementation"); // ui reads data from controller
        }
    }
}