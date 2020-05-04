using System;
using System.Net.Mail;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour MdpOublie.xaml
    /// </summary>
    public partial class MdpOublie : Window
    {
        public MdpOublie()
        {
            InitializeComponent();
        }

        private void Envoyer_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            //try
            //{
            //    Random random = new Random();
            //    MailMessage mail = new MailMessage();
            //    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            //    mail.From = new MailAddress("4timesperma@gmail.com");
            //    mail.To.Add(mailRecup.Text);
            //    mail.Subject = "Récupération Mot de Passe Cooking";
            //    mail.Body = "Bonjour,\n\nVotre mot de passe temporaire est : " + random.Next(1000, 9999);

            //    SmtpServer.Port = 587;
            //    SmtpServer.Credentials = new System.Net.NetworkCredential("4timesperma@gmail.com", "vmQbF4AmdXWpJnZ");
            //    SmtpServer.EnableSsl = true;

            //    SmtpServer.Send(mail);
            //    MessageBox.Show("mail sent");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }
    }
}