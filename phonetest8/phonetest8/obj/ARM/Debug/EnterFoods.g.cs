﻿#pragma checksum "C:\Users\lamch_000\Documents\GitHub\flamingcupcake\phonetest8\phonetest8\EnterFoods.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5A646B97323380B7E11A264011E6D212"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace phonetest8 {
    
    
    public partial class Page1 : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Canvas viewfinderCanvas;
        
        internal System.Windows.Media.VideoBrush viewfinderBrush;
        
        internal System.Windows.Media.CompositeTransform viewfinderTransform;
        
        internal System.Windows.Controls.TextBlock focusBrackets;
        
        internal System.Windows.Controls.Grid BottomPanel;
        
        internal System.Windows.Controls.TextBlock Info;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/phonetest8;component/EnterFoods.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.viewfinderCanvas = ((System.Windows.Controls.Canvas)(this.FindName("viewfinderCanvas")));
            this.viewfinderBrush = ((System.Windows.Media.VideoBrush)(this.FindName("viewfinderBrush")));
            this.viewfinderTransform = ((System.Windows.Media.CompositeTransform)(this.FindName("viewfinderTransform")));
            this.focusBrackets = ((System.Windows.Controls.TextBlock)(this.FindName("focusBrackets")));
            this.BottomPanel = ((System.Windows.Controls.Grid)(this.FindName("BottomPanel")));
            this.Info = ((System.Windows.Controls.TextBlock)(this.FindName("Info")));
        }
    }
}

