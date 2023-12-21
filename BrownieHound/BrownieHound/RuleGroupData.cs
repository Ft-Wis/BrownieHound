using System;
using System.Collections.Generic; 

namespace BrownieHound
{
    public class ruleGroupData
    {
        public bool isCheck { get; set; } = false;
        public int No { get; set; }
        public String Name { get; set; }
        public int ruleItems { get; set; } = 0;
        public List<RuleData.ruleData> ruleDatas { get; set; } = new List<RuleData.ruleData>();
        public List<RuleData.ruleData> blackListRules { get; set; } = new List<RuleData.ruleData>();
        public List<RuleData.ruleData> whiteListRules { get; set; } = new List<RuleData.ruleData>();

        public ruleGroupData(int no, String name)
        {
            this.No = no;
            this.Name = name;
        }

        public void ruleSet(string ruleLine)
        {
            ruleDatas.Add(new RuleData.ruleData(ruleLine, this.No, this.ruleItems++));
        }
    }
}