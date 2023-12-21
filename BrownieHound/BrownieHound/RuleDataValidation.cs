using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace BrownieHound
{
    public class RuleDataValidation
    {
        public class Rule_Validation : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            [RegularExpression("all|myAddress|^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])$", ErrorMessage = "IPアドレスの形式が正しくありません")]
            public ReactiveProperty<string> sourceIP { get; set; }

            [RegularExpression("broadcast|all|myAddress|^((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])$", ErrorMessage = "IPアドレスの形式が正しくありません")]
            public ReactiveProperty<string> destinationIP { get; set; }

            [RegularExpression("all|^([1-9]|[1-9][0-9]{1,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$", ErrorMessage = "ポート番号は1～65535の間で指定してください")]
            public ReactiveProperty<string> portNum { get; set; }

            [RegularExpression("none|[1-9][0-9]{0,3}", ErrorMessage = "サイズは1～9999の間で指定してください")]
            public ReactiveProperty<string> packetSize { get; set; }

            [RegularExpression("[1-9][0-9]{0,3}", ErrorMessage = "回数は1～9999の間で指定してください")]

            public ReactiveProperty<string> detectionCnt { get; set; }

            [RegularExpression("[1-9][0-9]{0,6}", ErrorMessage = "秒数は1～9999999の間で指定してください")]
            public ReactiveProperty<string> detectionMins { get; set; }

            public Rule_Validation()
            {
                this.sourceIP = new ReactiveProperty<string>("").SetValidateAttribute(() => sourceIP);
                this.destinationIP = new ReactiveProperty<string>("").SetValidateAttribute(() => destinationIP);
                this.portNum = new ReactiveProperty<string>("").SetValidateAttribute(() => portNum);
                this.packetSize = new ReactiveProperty<string>("").SetValidateAttribute(() => packetSize);
                this.detectionCnt = new ReactiveProperty<string>("").SetValidateAttribute(() => detectionCnt);
                this.detectionMins = new ReactiveProperty<string>("").SetValidateAttribute(() => detectionMins);

            }
        }
    }
}
