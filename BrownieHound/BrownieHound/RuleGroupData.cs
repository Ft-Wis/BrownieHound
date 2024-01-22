using System;
using System.Collections.Generic; 

namespace BrownieHound
{
    public class ruleGroupData
    {
        public bool IsCheck { get; set; } = false;
        public int No { get; set; }
        public String Name { get; set; }
        public int RuleItems { get; set; } = 0;
        public bool ExtendFlg { get; set; } = false;
        public string Linked { get; set; }
        public List<RuleData.ruleData> RuleDatas { get; set; } = new List<RuleData.ruleData>();
        public List<RuleData.ruleData> BlackListRules { get; set; } = new List<RuleData.ruleData>();
        public List<RuleData.ruleData> WhiteListRules { get; set; } = new List<RuleData.ruleData>();

        public ruleGroupData(int no, String name)
        {
            this.No = no;
            this.Name = name;
        }

        public void ruleSet(string ruleLine)
        {
            if (ruleLine.Equals("ExtendRule"))
            {
                ExtendFlg = true;
                Linked = "linked";
            }
            else if (ruleLine.Equals("StandardRule"))
            {
                ExtendFlg = false;
                Linked = "--";
            }
            else
            {
                RuleDatas.Add(new RuleData.ruleData(ruleLine, this.No, this.RuleItems++));
            }
        }
    }
}