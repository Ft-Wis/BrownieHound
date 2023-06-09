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
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Net;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;


namespace BrownieHound
{
    /// <summary>
    /// mail_settings.xaml の相互作用ロジック
    /// </summary>
    public partial class mail_settings : Page
    {
        public mail_settings()
        {
            InitializeComponent();
            SendEmailNew();
            //SendEmail();   
        }

        private void s_rTotop_redo_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void s_rTotop_submit_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SendEmail()
        {
            //SmtpClient client = new SmtpClient("smtp.gmail.com");
            //client.Port = 587;
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("browniehound2024@gmail.com", "Sotsuken2024");

            //MailAddress from = new MailAddress("browniehound2024@gmail.com");
            //MailAddress to = new MailAddress("ctb20.nakane.takumi@gmail.com");

            //MailMessage message = new MailMessage(from, to);
            //message.Subject = "テスト送信";
            //message.Body = "browniehoundですメールアドレスの認証を行ってください";
            //client.Send(message);
        }

        private void SendEmailNew()
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("browniehound", "browniehound2024@gmail.com"));
            email.To.Add(new MailboxAddress("you", "ctb20.nakane.takumi@gmail.com"));

            email.Subject = "テスト送信";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "MailKit を使ってメールを送ってみるテストです。"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com",587,false);
                smtp.Authenticate("browniehound2024", "eszyyyyhrwarlsns");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
