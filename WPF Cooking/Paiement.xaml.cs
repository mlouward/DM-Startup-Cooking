using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Paiement.xaml
    /// </summary>
    public partial class Paiement : Window
    {
        public Paiement()
        {
            InitializeComponent();
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select Prix_recette From recette where NomRecette_Recette= \"nom_recette\"";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int prix = int.Parse(reader.GetValue(0).ToString());
            command.CommandText = "Select solde From client";
            reader = command.ExecuteReader();
            int solde = int.Parse(reader.GetValue(0).ToString());
            connection.Close();

            TextBlockPrix.Text += " " + prix;
            TextBlockSolde.Text += " " + solde;
            TextBlockPrixBis.Text += " " + int.Parse(TextBlockNbPlats.Text) * prix;
            TextBlockSoldeRestant.Text += " " + (solde - int.Parse(TextBlockNbPlats.Text) * prix);

        }

        private void ButtonPayer_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = 1502kanDAL";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "Select Prix_recette From recette where NomRecette_Recette= \"nom_recette\"";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            int prix = int.Parse(reader.GetValue(0).ToString());
            command.CommandText = "Select solde From client";
            reader = command.ExecuteReader();
            int solde = int.Parse(reader.GetValue(0).ToString());
            connection.Close();

            if (solde < prix * int.Parse(TextBlockNbPlats.Text))
            {
                MessageBox.Show("Vous ne possédez pas assez de Cook pour réaliser cette commande. Veuillez recharger votre solde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int newSolde = solde - prix * int.Parse(TextBlockNbPlats.Text);
                command.CommandText = "Update client Set solde=" + newSolde + " Where Nom_Client=...";
                MessageBox.Show("La commande a bien été effectuée. Merci de votre confiance. N'hésitez pas à noter l'application!");


            }

        }

        private void ButtonRecharger_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
