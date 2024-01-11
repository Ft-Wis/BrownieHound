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

namespace BrownieHound
{
    /// <summary>
    /// certification_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class certification_Window : Window
    {
        public certification_Window(string mailAddress)
        {
            InitializeComponent();
            MessageBox.Show(this, mailAddress);

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
                body.HtmlBody = $"<html><body><h1>メール認証</h1><br><h2>２行目</h2>";
                body.HtmlBody += "</body></html>";
                email.Body = body.ToMessageBody();
                smtp.Send(email);
            }
        }

        private string Randomcerti (){
            Random random = new System.Random();
            
            Console.WriteLine(random.ToString);//random.Next(0, 10)
            return random;
        }


    private void authorizeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
