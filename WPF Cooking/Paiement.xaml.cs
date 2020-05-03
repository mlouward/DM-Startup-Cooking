using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Paiement.xaml
    /// </summary>
    public partial class Paiement : Window
    {
        decimal prixTot = 0;
        decimal soldeRestant = 0;
        public Paiement()
        {
            InitializeComponent();

            //string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";

            //MySqlConnection connection = new MySqlConnection(connectionString);
            //connection.Open();
            //MySqlCommand command = connection.CreateCommand();
            //command.CommandText = "Select Prix_recette From recette where NomRecette_Recette= \"nom_recette\"";

            //MySqlDataReader reader;
            //reader = command.ExecuteReader();
            //int prix = int.Parse(reader.GetValue(0).ToString());
            //command.CommandText = "Select solde From client";
            //reader = command.ExecuteReader();
            //int solde = int.Parse(reader.GetValue(0).ToString());
            //connection.Close();

            Dictionary<Recette, int> recap = ListeRecettes.compteRecettes; //On récupère la liste des recettes dans le panier et leur nombre.
            listBoxRecap.ItemsSource = recap;
            foreach (KeyValuePair<Recette, int> recette in recap)
            {
                prixTot += recette.Key.PrixVente * recette.Value;
            }
            TextBlockPrix.Text = $"{prixTot} cook(s)";
            TextBlockSolde.Text = $"{MainWindow.currentUser.Solde} cook(s)";
            TextBlockPrixBis.Text = $"- {prixTot} cook(s)";
            soldeRestant = MainWindow.currentUser.Solde - prixTot;
            TextBlockSoldeRestant.Text = $"Solde restant : {soldeRestant} cook(s)";
        }

        private void ButtonPayer_Click(object sender, RoutedEventArgs e)
        {
            decimal solde = MainWindow.currentUser.Solde;

            if (soldeRestant < 0)
                MessageBox.Show("Vous ne possédez pas assez de Cook pour réaliser cette commande. Veuillez recharger votre solde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"Update client Set Solde_Client={soldeRestant} Where Mail_Client=\"{MainWindow.currentUser.Mail}\"";
                command.ExecuteNonQuery();
                MessageBox.Show($"La commande a bien été effectuée (solde restant : {soldeRestant} cook(s)). \nMerci de votre confiance. N'hésitez pas à noter l'application!");
                connection.Close();
            }
        }
        private void ButtonRecharger_Click(object sender, RoutedEventArgs e)
        {
            //Lien hypothétique vers une plateforme de paiement pour ajouter des cooks à son compte.
        }
    }
}