using System;
using System.Collections.Generic;

namespace BrownieHound
{
    public class RuleData
    {
        public class ruleData
        {
            // ruleData クラスの定義
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
                ruleCategory = Int32.Parse(data[i++]);
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
    }
}
