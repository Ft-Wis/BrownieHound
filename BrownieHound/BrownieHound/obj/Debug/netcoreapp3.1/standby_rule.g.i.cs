﻿#pragma checksum "..\..\..\standby_rule.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C2525E00B1826B02DCAD3E81BC1DBCEBFE135AA9"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using BrownieHound;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace BrownieHound {
    
    
    /// <summary>
    /// standby_rule
    /// </summary>
    public partial class standby_rule : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 12 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button s_rTotop;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label interfaceLabel;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ruleGroupList;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button activate;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ruleGroupDetail;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\standby_rule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkAll;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BrownieHound;component/standby_rule.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\standby_rule.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 11 "..\..\..\standby_rule.xaml"
            ((System.Windows.Controls.Grid)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.s_rTotop = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\..\standby_rule.xaml"
            this.s_rTotop.Click += new System.Windows.RoutedEventHandler(this.s_rTotop_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.interfaceLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.ruleGroupList = ((System.Windows.Controls.ListView)(target));
            return;
            case 6:
            this.activate = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\..\standby_rule.xaml"
            this.activate.Click += new System.Windows.RoutedEventHandler(this.activate_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ruleGroupDetail = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\standby_rule.xaml"
            this.ruleGroupDetail.Click += new System.Windows.RoutedEventHandler(this.ruleGroupDetail_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.checkAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 45 "..\..\..\standby_rule.xaml"
            this.checkAll.Unchecked += new System.Windows.RoutedEventHandler(this.checkAll_Unchecked);
            
            #line default
            #line hidden
            
            #line 45 "..\..\..\standby_rule.xaml"
            this.checkAll.Checked += new System.Windows.RoutedEventHandler(this.checkAll_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 5:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.Control.MouseDoubleClickEvent;
            
            #line 18 "..\..\..\standby_rule.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.ListViewItem_DoubleClikck);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.ListBoxItem.SelectedEvent;
            
            #line 19 "..\..\..\standby_rule.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.ListViewItem_Selected);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

