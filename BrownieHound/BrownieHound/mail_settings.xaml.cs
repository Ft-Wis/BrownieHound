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
        HashFunction hashFunction = new HashFunction();
        string path = @"conf\mail.conf";
        string authorizedPath = @"conf\authorize.conf"; 
        string authorize;
        public mail_settings()
        {
            InitializeComponent();
            mailValidation = new Mail_Validation();
            DataContext = mailValidation;
            App.Current.MainWindow.MinHeight = 680;
        }

        private void s_rTotop_redo_Click(object sender, RoutedEventArgs e)
        {
            Window_Resize();
            NavigationService.GoBack();
            
        }

        private void Window_Resize()
        {
            Application.Current.MainWindow.MinHeight = 450;
            Application.Current.MainWindow.Height = 450;
        }

        private void s_rTotop_submit_Click(object sender, RoutedEventArgs e)
        {

            if (!Directory.Exists(@"conf"))
            {
                Directory.CreateDirectory(@"conf");
            }
            if (mailValidation.isEnabled.Value)
            {
                //メール認証処理
                if ((mailValidation.span.Value != "" && !mailValidation.span.HasErrors) && (mailValidation.mailAddress.Value != "" && !mailValidation.mailAddress.HasErrors) && (mailValidation.userName.Value != "" && !mailValidation.userName.HasErrors))
                {
                    if (!File.Exists(authorizedPath))
                    {
                        certification_Window certificationWindow = new certification_Window(mailValidation.mailAddress.Value,mailValidation.userName.Value);
                        if (certificationWindow.ShowDialog() == true)
                        {
                            var nextPage = new top();
                            Window_Resize();
                            NavigationService.Navigate(nextPage);
                            
                        }
                    }
                    else
                    {
                        //メール検証
                        if (hashFunction.verifyMail(mailValidation.mailAddress.Value, authorizedPath))
                        {
                            MessageBox.Show("内容を保存しました。");
                            var nextPage = new top();
                            Window_Resize();
                            NavigationService.Navigate(nextPage);
                            
                        }
                        else
                        {
                            MessageBox.Show("ご入力いただいたメールアドレスは認証されておりませんので、認証手続きに進みます。\n画面が切り替わるまで少々お待ちください。");
                            certification_Window certificationWindow = new certification_Window(mailValidation.mailAddress.Value, mailValidation.userName.Value);
                            if (certificationWindow.ShowDialog() == true)
                            {
                                var nextPage = new top();
                                Window_Resize();
                                NavigationService.Navigate(nextPage);
                                
                            }
                        }
                    }

                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
                    {
                        sw.WriteLine($"userName:{mailValidation.userName.Value}");
                        sw.WriteLine($"sendEnabled:{mailValidation.isEnabled.Value}");
                        sw.WriteLine($"sendSpan:{mailValidation.span.Value}");
                        sw.WriteLine($"mailLimit:{mailValidation.mailLimit.Value}");
                        sw.WriteLine($"sendMailAddress:{mailValidation.mailAddress.Value}");
                    }

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
                    sw.WriteLine($"userName:{mailValidation.userName.Value}");
                    sw.WriteLine($"sendEnabled:{mailValidation.isEnabled.Value}");
                    sw.WriteLine($"sendSpan:{mailValidation.span.Value}");
                    sw.WriteLine($"mailLimit:{mailValidation.mailLimit.Value}");
                    sw.WriteLine($"sendMailAddress:{mailValidation.mailAddress.Value}");
                }

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
                    sw.WriteLine($"userName:");
                    sw.WriteLine($"sendEnabled:");
                    sw.WriteLine($"sendSpan:");
                    sw.WriteLine($"mailLimit:");
                    sw.WriteLine($"sendMailAddress:");
                }
            }
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("UTF-8")))
            {
                mailValidation.userName.Value = sr.ReadLine().Split(":")[1];
                if (bool.TryParse(sr.ReadLine().Split(":")[1], out var isEnabled))
                {
                    mailValidation.isEnabled.Value = isEnabled;
                }
                if (int.TryParse(sr.ReadLine().Split(":")[1], out var span))
                {
                    mailValidation.span.Value = span.ToString();
                }
                if (int.TryParse(sr.ReadLine().Split(":")[1], out var mailLimit))
                {
                    mailValidation.mailLimit.Value = mailLimit.ToString();
                }
                mailValidation.mailAddress.Value = sr.ReadLine().Split(":")[1];
            }
            //Debug.WriteLine(authorize);
        }

        private class InputDialog
        {
        }
    }
    public class Mail_Validation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [RegularExpression("[1-9][0-9]{0,4}", ErrorMessage = "送信スパンは1～99999の間で設定してください。")]
        public ReactiveProperty<string> span { get; }

        [RegularExpression("[1-9][0-9]{0,4}", ErrorMessage = "しきい値は1～99999の間で設定してください。")]
        public ReactiveProperty<string> mailLimit { get; }

        [RegularExpression(@"[\w-.]+@[\w-._]+.[A-Za-z]+", ErrorMessage = "メールアドレスを正しく入力してください。")]
        public ReactiveProperty<string> mailAddress { get; }

        [RegularExpression("[!-~]{4,20}", ErrorMessage = "ユーザー名は4～20文字の半角英数字で入力してください。")]
        public ReactiveProperty<string> userName { get; }

        public ReactiveProperty<bool> isEnabled { get; }

        public Mail_Validation()
        {
            this.span = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.span);
            this.mailLimit = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.mailLimit);
            this.mailAddress = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.mailAddress);
            this.isEnabled = new ReactiveProperty<bool>(false);
            this.userName = new ReactiveProperty<string>("")
                .SetValidateAttribute(() => this.userName);
        }
    }
}
