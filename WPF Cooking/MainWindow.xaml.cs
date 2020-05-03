using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Personne currentUser = new Personne();

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

            string requeteMdp = $"Select mdp_Client from client where Mail_Client = \"{mail}\"";
            string requeteSolde = $"Select Solde_client from client where Mail_Client = \"{mail}\"";
            string requeteStatut = $"Select Statut_client from client where Mail_Client = \"{mail}\"";

            string resultatRequete = "";
            string resultatStatut = "";
            int solde = 0;

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlConnection connection2 = new MySqlConnection(connectionString);
            MySqlConnection connection3 = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                connection2.Open();
                connection3.Open();

                MySqlCommand cmd = new MySqlCommand(requeteMdp, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Read();
                resultatRequete = rdr.GetString(0);

                MySqlCommand cmd2 = new MySqlCommand(requeteSolde, connection2);
                MySqlDataReader rdr2 = cmd2.ExecuteReader();

                rdr2.Read();
                solde = rdr2.GetInt32(0);

                MySqlCommand cmd3 = new MySqlCommand(requeteStatut, connection3);
                MySqlDataReader rdr3 = cmd3.ExecuteReader();

                rdr3.Read();
                resultatStatut = rdr3.GetString(0).ToLower();

                rdr.Close();
                rdr2.Close();
                rdr3.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("La connection avec la BDD a échoué."); //Pourquoi s'affiche tout le temps?
            }
            connection.Close();
            connection2.Close();
            connection3.Close();
            // Si nom d'utilisateur pas dans la base de données
            if (resultatRequete == "") MessageBox.Show($"L'adresse mail '{mail}' n'appartient pas à la base de données. Veuillez créer un compte.");

            // Si mdp incorrect
            else if (resultatRequete != PasswordBoxMdp.Password) MessageBox.Show($"Le mot de passe entré ne correspond pas à l'adresse '{mail}'");

            // Si tout est correct
            else
            {
                if (resultatStatut == "client")
                {
                    currentUser.Mail = TextBoxMail.Text;
                    currentUser.Solde = solde;
                    ListeRecettes listeRecettes = new ListeRecettes();
                    listeRecettes.Show();
                }
                else if (resultatStatut == "cdr")
                {

                }
                else if (resultatStatut == "admin")
                {

                }
                else
                {
                    MessageBox.Show($"Le statut \"{resultatStatut}\" n'est pas reconnu.");
                }
            }
        }
    }
}