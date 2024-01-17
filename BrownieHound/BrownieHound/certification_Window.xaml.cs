using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MimeKit;
using MailKit.Net.Smtp;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Net.Mail;
using System.Windows.Threading;
using System.IO;

namespace BrownieHound
{
    /// <summary>
    /// certification_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class certification_Window : Window
    {
        private string certiCode;
        private int wrongcetiCode = 0;
        private string senderMailAddress;
        public certification_Window(string mailAddress)
        {
            InitializeComponent();
            //MessageBox.Show(this, mailAddress);

            this.senderMailAddress = mailAddress;
            mailblock.Text = mailAddress;
            certiCode = Randomcerti();
            sendEmail(mailAddress, certiCode);
        }

        private string Randomcerti (){
            string authCode = "";
            Random r = new Random();
            int intAuthCode = r.Next(999999);
            //５桁以下なら0埋め
            if (intAuthCode < 100000)
            {
                authCode = intAuthCode.ToString("D6");
            }
            else
            {
                authCode = intAuthCode.ToString();
            }

            //MessageBox.Show(authCode);
            return authCode;
        }

        private void sendEmail(string mailAddress, string authCode)
        {
            var host = "smtp.gmail.com";
            var port = 587;
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(host, port);
                smtp.Authenticate("browniehound2024", "eszyyyyhrwarlsns");

                var email = new MimeMessage();
                var builder = new MimeKit.BodyBuilder();
                email.From.Add(new MailboxAddress("browniehound", "browniehound2024@gmail.com"));
                email.To.Add(new MailboxAddress("", mailAddress));
                email.Subject = "メール認証";
                MimeKit.TextPart textPart = new MimeKit.TextPart("Plain");
                var body = new BodyBuilder();
                body.HtmlBody = $"<html><body>下記の数字を画面に入力してください<h2>{authCode}</h2><br>" +
                    $"※このメールはBrownieHoundにてメール認証の手続きが行われたメールアドレスに自動送信しております。<br>認証手続きにお心当たりのない場合は、お手数ですがメールを破棄していただきますようお願いいたします。";
                body.HtmlBody += "</body></html>";
                email.Body = body.ToMessageBody();
                smtp.Send(email);
                // タイマーの初期化
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMinutes(15);
                timer.Tick += Timer_Tick;

                // タイマーを開始
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 15分ごとの処理をここに追加
                MessageBox.Show("認証コードの有効期限が経過しました。\n再度認証処理を行ってください。");
                      
            // ウィンドウを閉じる
            Close();
        }

        private void authorizeButton_Click(object sender, RoutedEventArgs e)
        {
           string userInput = certitext.Text;
           
           int maxcertiCode = 5;

            if (!string.IsNullOrEmpty(userInput))
            {                               
                if (userInput == certiCode)
                {
                    //検証に成功したとき
                    MessageBox.Show("認証完了");
                    writeToAuthConf();
                    Close();
                }
                else
                {
                    //6桁の数字を間違えたとき
                    MessageBox.Show("パスワードが一致しません。");
                    wrongcetiCode++;
                    MessageBox.Show(wrongcetiCode.ToString());
                    if (wrongcetiCode >= maxcertiCode)
                    {
                        //検証に失敗したとき
                        MessageBox.Show("パスワードを5回間違えました。再度認証処理を行ってください。");
                        Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("パスワードの入力がありません。");
                // テキストボックスに入力がない場合の処理を追加
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            certitext.MaxLength = 6;
        }

        private void writeToAuthConf()
        {
            HashFunction hashFunction = new HashFunction();
            string authorizedPath = @"conf\authorize.conf";
            if (!File.Exists(authorizedPath))
            {
                using (FileStream fs = File.Create(authorizedPath)) ;
            }
            using (StreamWriter sw = new StreamWriter(authorizedPath, false, Encoding.GetEncoding("UTF-8")))
            {
                sw.WriteLine(hashFunction.ComputeHash(senderMailAddress));
            }

        }
    }
}
