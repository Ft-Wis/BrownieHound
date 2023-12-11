using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static BrownieHound.capture;

namespace BrownieHound
{
    /// <summary>
    /// packet_detail_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class packet_detail_Window : Window
    {
        public packet_detail_Window(string data)
        {
            
            InitializeComponent();
            if (data != null)
            {
                JObject packetObject = JObject.Parse(data);
                AddJObjectToTreeView(detail_tree.Items, packetObject);
            }
        }

        private void AddJObjectToTreeView(ItemCollection items, JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                TreeViewItem node = new TreeViewItem();
                if(property.Name.IndexOf("_") > 0)
                {
                    node.Header = property.Name.Substring(property.Name.IndexOf("_") + 1);
                }
                else
                {
                    node.Header = property.Name;
                }

                if (property.Value.Type == JTokenType.Object)
                {
                    // プロパティがオブジェクトの場合は再帰的に追加
                    AddJObjectToTreeView(node.Items, (JObject)property.Value);
                }
                else if (property.Value.Type == JTokenType.Array)
                {
                    // プロパティが配列の場合は配列要素を追加
                    int index = 0;
                    foreach (var item in (JArray)property.Value)
                    {
                        TreeViewItem arrayNode = new TreeViewItem();
                        arrayNode.Header = $"[{index}]";
                        if (item.Type == JTokenType.Object)
                        {
                            // 配列要素がオブジェクトの場合は再帰的に追加
                            AddJObjectToTreeView(arrayNode.Items, (JObject)item);
                        }
                        else
                        {
                            // 配列要素が値の場合はノードに追加
                            arrayNode.Header += $" :  [{property.Value[index].ToString()}]";
                        }
                        node.Items.Add(arrayNode);
                        index++;
                    }
                }
                else
                {
                    // プロパティが値の場合はノードに追加
                    node.Header += $" :  [{property.Value.ToString()}]";
                    //node.Items.Add(property.Value.ToString());
                }

                items.Add(node);
            }
        }
    }

}
