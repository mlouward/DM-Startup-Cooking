using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MySql.Data.MySqlClient;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ForgottenPw_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MdpOublie recupMdp = new MdpOublie();
            recupMdp.Show();
        }

        private void SignUp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SignUpPage p = new SignUpPage();
            p.Show();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string mail = TextBoxMail.Text;
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            string requete = $"Select mdp_Client from client where Mail_Client = \"{ mail}\"";
            string resultatRequete = "";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Read();
                resultatRequete = rdr.GetString(0);

                rdr.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show("La connection avec la BDD a échoué."); //Pourquoi s'affiche tout le temps?
            }
            connection.Close();
            // Si nom d'utilisateur pas dans la base de données
            if (resultatRequete == "") MessageBox.Show($"L'adresse mail '{mail}' n'appartient pas à la base de données. Veuillez créer un compte.");

            // Si mdp incorrect
            else if (resultatRequete != PasswordBoxMdp.Password) MessageBox.Show($"Le mot de passe entré ne correspond pas à l'adresse '{mail}'");

            // Si tout est correct
            else
            {
                ListeRecettes listeRecettes = new ListeRecettes();
                listeRecettes.Show();
            }
        }

    }
}
