﻿#pragma checksum "C:\Users\Vivian\Documents\Github\flamingcupcake\phonetest8\phonetest8\ViewRecipe.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A0C5630AD1544EF7184E865431251916"
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
    
    
    public partial class ViewRecipe : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Style UnfilledNomStyle;
        
        internal System.Windows.Style FilledNomStyle;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.Pivot PivotPage;
        
        internal System.Windows.Controls.TextBlock percentMatchText;
        
        internal System.Windows.Controls.TextBlock prepTimeText;
        
        internal Microsoft.Phone.Controls.LongListSelector ingLongListSelector;
        
        internal Microsoft.Phone.Controls.LongListSelector insLongListSelector;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Foodstorm;component/ViewRecipe.xaml", System.UriKind.Relative));
            this.UnfilledNomStyle = ((System.Windows.Style)(this.FindName("UnfilledNomStyle")));
            this.FilledNomStyle = ((System.Windows.Style)(this.FindName("FilledNomStyle")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PivotPage = ((Microsoft.Phone.Controls.Pivot)(this.FindName("PivotPage")));
            this.percentMatchText = ((System.Windows.Controls.TextBlock)(this.FindName("percentMatchText")));
            this.prepTimeText = ((System.Windows.Controls.TextBlock)(this.FindName("prepTimeText")));
            this.ingLongListSelector = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("ingLongListSelector")));
            this.insLongListSelector = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("insLongListSelector")));
        }
    }
}

