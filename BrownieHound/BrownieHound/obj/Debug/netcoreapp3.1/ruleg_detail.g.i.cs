﻿#pragma checksum "..\..\..\ruleg_detail.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7E3B7B987A879FE4F3D8FBC3E16E199E6983E39E"
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
    /// ruleg_detail
    /// </summary>
    public partial class ruleg_detail : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 18 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button add;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button edit;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button redo;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button inactivate;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button correct;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ruleGroupName;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid rule_DataGrid;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkAll;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\ruleg_detail.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox linkCheck;
        
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
            System.Uri resourceLocater = new System.Uri("/BrownieHound;component/ruleg_detail.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ruleg_detail.xaml"
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
            this.add = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\ruleg_detail.xaml"
            this.add.Click += new System.Windows.RoutedEventHandler(this.addButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.edit = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\ruleg_detail.xaml"
            this.edit.Click += new System.Windows.RoutedEventHandler(this.editButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.redo = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\ruleg_detail.xaml"
            this.redo.Click += new System.Windows.RoutedEventHandler(this.redoButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.inactivate = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\ruleg_detail.xaml"
            this.inactivate.Click += new System.Windows.RoutedEventHandler(this.inactivate_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.correct = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\ruleg_detail.xaml"
            this.correct.Click += new System.Windows.RoutedEventHandler(this.correct_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ruleGroupName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.rule_DataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 24 "..\..\..\ruleg_detail.xaml"
            this.rule_DataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DataGrid_Selected);
            
            #line default
            #line hidden
            
            #line 24 "..\..\..\ruleg_detail.xaml"
            this.rule_DataGrid.BeginningEdit += new System.EventHandler<System.Windows.Controls.DataGridBeginningEditEventArgs>(this.rule_DataGrid_BeginningEdit);
            
            #line default
            #line hidden
            return;
            case 9:
            this.checkAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 45 "..\..\..\ruleg_detail.xaml"
            this.checkAll.Checked += new System.Windows.RoutedEventHandler(this.checkAll_Checked);
            
            #line default
            #line hidden
            
            #line 45 "..\..\..\ruleg_detail.xaml"
            this.checkAll.Unchecked += new System.Windows.RoutedEventHandler(this.checkAll_Unchecked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.linkCheck = ((System.Windows.Controls.CheckBox)(target));
            
            #line 46 "..\..\..\ruleg_detail.xaml"
            this.linkCheck.Checked += new System.Windows.RoutedEventHandler(this.linkCheck_Checked);
            
            #line default
            #line hidden
            
            #line 46 "..\..\..\ruleg_detail.xaml"
            this.linkCheck.Unchecked += new System.Windows.RoutedEventHandler(this.linkCheck_Unchecked);
            
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
            switch (connectionId)
            {
            case 8:
            
            #line 29 "..\..\..\ruleg_detail.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.IsChecked_Checked);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\ruleg_detail.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.IsChecked_Unchecked);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

