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
        public static Client currentUser = new Client();

        public MainWindow()
        {
            InitializeComponent();
            // TODO : remove
            TextBoxMail.Text = "admin";
            PasswordBoxMdp.Password = "Maxime";
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

        public void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string mail = TextBoxMail.Text;
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";

            string requeteMdp = $"Select mdp_Client, Solde_Client, Statut_client, Nom_Client from client where Mail_Client = \"{mail}\"";

            string resultatMdp = "";

            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requeteMdp, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Read();
                resultatMdp = rdr.GetString(0);
                int solde = rdr.GetInt32(1);
                string resultatStatut = rdr.GetString(2).ToLower();
                string resultatNom = rdr.GetString(3);
                rdr.Close();
                // Si nom d'utilisateur pas dans la base de données
                if (resultatMdp == "") MessageBox.Show($"L'adresse mail '{mail}' n'appartient pas à la base de données. Veuillez créer un compte.");

                // Si mdp incorrect
                else if (resultatMdp != PasswordBoxMdp.Password) MessageBox.Show($"Le mot de passe entré ne correspond pas à l'adresse '{mail}'");

                // Si tout est correct
                else
                {
                    if (resultatStatut == "client")
                    {
                        currentUser.Mail = TextBoxMail.Text;
                        currentUser.Solde = solde;
                        currentUser.Nom = resultatNom;

                        ListeRecettes listeRecettes = new ListeRecettes();
                        listeRecettes.Show();
                    }
                    else if (resultatStatut.ToLower() == "cdr")
                    {
                        currentUser.Mail = TextBoxMail.Text;
                        currentUser.Solde = solde;
                        currentUser.Nom = resultatNom;
                        //Un cdr est un client, donc il peut accéder au portail client ainsi qu'au portail CDR.
                        var res = MessageBox.Show($"Bonjour {currentUser.Nom}! Voulez-vous vous accéder au portail Client? Sinon, vous ouvrirez le portail CDR.",
                            "Choix de portail", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (res == MessageBoxResult.Yes)
                        {
                            ListeRecettes listeRecettes = new ListeRecettes();
                            listeRecettes.Show();
                        }
                        else
                        {
                            PageCDR pageCdr = new PageCDR();
                            pageCdr.Show();
                        }
                    }
                    else if (resultatStatut.ToLower() == "admin")
                    {
                        currentUser.Mail = TextBoxMail.Text;
                        currentUser.Solde = solde;
                        currentUser.Nom = resultatNom;
                        //Un admin est un client, donc il peut accéder au portail client ainsi qu'au portail administration.
                        var res = MessageBox.Show($"Bonjour {currentUser.Nom}! Voulez-vous vous accéder au portail Client? Sinon, vous ouvrirez le portail ADMIN.",
                            "Choix de portail", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (res == MessageBoxResult.Yes)
                        {
                            ListeRecettes listeRecettes = new ListeRecettes();
                            listeRecettes.Show();
                        }
                        else
                        {
                            Administration admin = new Administration();
                            admin.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Le statut \"{resultatStatut}\" n'est pas reconnu. Veuillez contacter l\'administration.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("La connection avec la BDD a échoué :\n", ex.Message);
            }
            connection.Close();
        }
    }
}