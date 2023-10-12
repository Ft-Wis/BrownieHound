using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BrownieHound
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public class mailData
        {
            public bool enabled { get; set; }
            public int sendSpan { get; set; }
            public string mailAddress { get; set; }
            public bool authorized { get; set; } = false;
            public mailData(bool enabled, int sendSpan, string mailAddress, bool authorized)
            {
                this.enabled = enabled;
                this.sendSpan = sendSpan;
                this.mailAddress = mailAddress;
                this.authorized = authorized;
            }
        }

        public class ruleData
        {
            public int ruleGroupNo { get; set; }
            public int ruleNo { get; set; }
            public int detectionInterval { get; set; }
            public int detectionCount { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string Protocol { get; set; }
            public string sourcePort { get; set; }
            public string destinationPort { get; set; }
            public int frameLength { get; set; }
            public int ruleCategory { get; set; } = 0;

            private void ruleSplit(string ruleSeet)
            {
                string[] data = ruleSeet.Split(',');
                int i = 0;
                detectionInterval = Int32.Parse(data[i++]);
                detectionCount = Int32.Parse(data[i++]);
                Source = data[i++];
                Destination = data[i++];
                Protocol = data[i++];
                sourcePort = data[i++];
                destinationPort = data[i++];
                frameLength = Int32.Parse(data[i]);
            }
            public ruleData(string ruleSeet, int ruleGroupNo, int ruleNo)
            {
                this.ruleGroupNo = ruleGroupNo;
                this.ruleNo = ruleNo;
                ruleSplit(ruleSeet);
            }

            public ruleData(string ruleSheet)
            {
                ruleSplit(ruleSheet);
            }
        }

        public class ruleGroupData
        {
            public bool isCheck { get; set; } = false;
            public int No { get; set; }
            public String Name { get; set; }
            public int ruleItems { get; set; } = 0;
            public List<ruleData> ruleDatas { get; set; } = new List<ruleData>();
            public List<ruleData> blackListRules { get; set; } = new List<ruleData>();
            public List<ruleData> whiteListRules { get; set; } = new List<ruleData>();

            public ruleGroupData(int no, String name)
            {
                this.No = no;
                this.Name = name;
            }

            public void ruleSet(string ruleLine)
            {
                ruleDatas.Add(new ruleData(ruleLine, this.No, this.ruleItems++));
            }
        }

        public static List<ruleGroupData> Read(string path)
        {
            List<ruleGroupData> data = new List<ruleGroupData>();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            IEnumerable<System.IO.FileInfo> ruleFiles = di.EnumerateFiles("*.txt", System.IO.SearchOption.TopDirectoryOnly).OrderBy(f => f.CreationTime).ToList();
            foreach (var ruleFile in ruleFiles.Select((Value, Index) => new { Value, Index }))
            {
                string ruleGroupName = ruleFile.Value.Name.Remove(ruleFile.Value.Name.Length - 4);
                ruleGroupData ruleGroup = new ruleGroupData(ruleFile.Index, ruleGroupName);
                string filePath = $"{path}\\{ruleFile.Value.Name}";
                try
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
                    {
                        while (sr.Peek() != -1)
                        {
                            ruleGroup.ruleSet(sr.ReadLine());
                        }
                    }
                }
                catch
                {

                }
                data.Add(ruleGroup);
            }
            return data;
        }

    }
}
