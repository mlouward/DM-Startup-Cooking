using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Window
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = $"select mail_client from client where mail_client=\"{Mail.Text.Trim()}\"";
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = command.ExecuteReader();
                if (rdr.Read())
                {
                    MessageBox.Show($"Le compte lié à l'adresse \"{Mail.Text}\" existe déjà.");
                    rdr.Close();
                }
                else
                {
                    rdr.Close();
                    if (Mdp.Password != MdpVerif.Password)
                    {
                        MessageBox.Show("Les mots de passes rentrés ne correspondent pas");
                        Mdp.Password = "";
                        MdpVerif.Password = "";
                    }
                    else
                    {
                        Client client = new Client(Mail.Text.Trim(), Nom.Text.Trim(), NumTel.Text.Trim(),
                            Mdp.Password.Trim(), 0, (bool)IsCdr.IsChecked ? "cdr" : "client");
                        command.CommandText = $"insert into client values ({client})";
                        command.ExecuteNonQuery();
                        MessageBox.Show($"Bonjour {Nom.Text}! Bienvenue chez Cooking!");
                        MainWindow main = new MainWindow();
                        main.Show();
                        main.TextBoxMail.Text = Mail.Text.Trim();
                        main.PasswordBoxMdp.Password = Mdp.Password;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
    }
}