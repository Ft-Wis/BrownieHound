using System;
using System.Collections.Generic;

namespace BrownieHound
{
    public class RuleData
    {
        public class ruleData
        {
            // ruleData クラスの定義
            public int RuleGroupNo { get; set; }
            public int RuleNo { get; set; }
            public int DetectionInterval { get; set; }
            public int DetectionCount { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string Protocol { get; set; }
            public string SourcePort { get; set; }
            public string DestinationPort { get; set; }
            public string FrameLength { get; set; }
            public int RuleCategory { get; set; } = 0;

            private void ruleSplit(string ruleSeet)
            {
                string[] data = ruleSeet.Split(',');
                int i = 0;
                RuleCategory = Int32.Parse(data[i++]);
                DetectionInterval = Int32.Parse(data[i++]);
                DetectionCount = Int32.Parse(data[i++]);
                Source = data[i++];
                Destination = data[i++];
                Protocol = data[i++];
                SourcePort = data[i++];
                DestinationPort = data[i++];
                FrameLength = data[i];
            }
            public ruleData(string ruleSeet, int ruleGroupNo, int ruleNo)
            {
                this.RuleGroupNo = ruleGroupNo;
                this.RuleNo = ruleNo;
                ruleSplit(ruleSeet);
            }
            public ruleData()
            {
                
            }

            public ruleData(string ruleSheet)
            {
                ruleSplit(ruleSheet);
            }
        }
    }
}
