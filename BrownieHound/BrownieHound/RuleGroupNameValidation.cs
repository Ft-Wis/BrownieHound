using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BrownieHound
{
    internal class RuleGroupNameValidation
    {
        public class RuleName_Validation : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [RegularExpression("[!#-\\)\\+-\\.0-;=@-\\[\\]-\\{\\}~]{1,34}", ErrorMessage = "34文字以下、英数字と一部の記号が使えます")]
            public ReactiveProperty<string> ruleGroupName { get; set; }

            public RuleName_Validation()
            {
                this.ruleGroupName = new ReactiveProperty<string>("").SetValidateAttribute(() => this.ruleGroupName);

            }
        }

    }
}
