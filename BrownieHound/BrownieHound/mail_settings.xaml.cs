using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Reactive.Bindings;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Reactive.Disposables;

namespace BrownieHound
{
    /// <summary>
    /// mail_settings.xaml の相互作用ロジック
    /// </summary>
    public partial class mail_settings : Page
    {
        Mail_Validation mailValidation;
        string path = @"conf\mail.conf";
        string authorize;
        public mail_settings()
        {
            InitializeComponent();
            mailValidation = new Mail_Validation();
            DataContext = mailValidation;
            
        }

        private void s_rTotop_redo_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void s_rTotop_submit_Click(object sender, RoutedEventArgs e)
        {

            if (!Directory.Exists(@"conf"))
            {
                Directory.CreateDirectory(@"conf");
            }
            if (mailValidation.isEnabled.Value)
            {
                if ((mailValidation.span.Value != "" && !mailValidation.span.HasErrors) && (mailValidation.mailAddress.Value != "" && !mailValidation.mailAddress.HasErrors))
                {

                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
                    {
                        sw.WriteLine($"sendEnabled:{mailValidation.isEnabled.Value}");
                        sw.WriteLine($"sendSpan:{mailValidation.span.Value}");
                        sw.WriteLine($"sendMailAddress:{mailValidation.mailAddress.Value}");
                        sw.WriteLine($"Authorized:Unauthorized");

                    }
                    MessageBox.Show("メール設定を保存しました。", "メール設定成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    var nextPage = new top();
                    NavigationService.Navigate(nextPage);

                }
                else
                {
                    MessageBox.Show("メールの送信を希望する場合は\nフィールドを正しく埋めてください。", "!警告!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
                {
                    sw.WriteLine($"sendEnabled:{mailValidation.isEnabled.Value}");
                    sw.WriteLine($"sendSpan:{mailValidation.span.Value}");
                    sw.WriteLine($"sendMailAddress:{mailValidation.mailAddress.Value}");
                    sw.WriteLine($"Authorized:Unauthorized");
                }
                MessageBox.Show("メール設定を保存しました。", "メール設定成功", MessageBoxButton.OK, MessageBoxImage.Information);
                var nextPage = new top();
                NavigationService.Navigate(nextPage);
            }


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@"conf"))
            {
                Directory.CreateDirectory(@"conf");
            }
            if (!File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
                {
                    sw.WriteLine($"sendEnabled:False");
                    sw.WriteLine($"sendSpan:");
                    sw.WriteLine($"sendMailAddress:");
                    sw.WriteLine($"Authorized:Unauthorized");
                }
            }
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
            {

                if (bool.TryParse(sr.ReadLine().Split(":")[1], out var isEnabled))
                {
                    mailValidation.isEnabled.Value = isEnabled;
                }
                if (int.TryParse(sr.ReadLine().Split(":")[1], out var span))
                {
                    mailValidation.span.Value = span.ToString();
                }
                mailValidation.mailAddress.Value = sr.ReadLine().Split(":")[1];
                authorize = sr.ReadLine().Split(":")[1];

            }
            //Debug.WriteLine(authorize);
        }
    }
    public class Mail_Validation:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [RegularExpression("[1-9][0-9]{0,4}")]
        public ReactiveProperty<string> span { get; }

        [RegularExpression(@"[\w\-\._]+@[\w\-\._]+\.[A-Za-z]+")]
        public ReactiveProperty<string> mailAddress { get; }
        
        public ReactiveProperty<bool> isEnabled { get; }

        public Mail_Validation()
        {
            this.span = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.span);
            this.mailAddress = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.mailAddress);
            this.isEnabled = new ReactiveProperty<bool>(false);
        }
        

    }
}
