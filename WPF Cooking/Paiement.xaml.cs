using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Paiement.xaml
    /// </summary>
    public partial class Paiement : Window
    {
        private decimal prixTot = 0;
        private decimal soldeRestant = 0;
        private Dictionary<Recette, int> recap = ListeRecettes.compteRecettes; //On récupère la liste des recettes dans le panier et leur nombre.

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
                //Popularité, solde restant s'incrémentent et ajoute une entrée dans la table "commande"
                string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                //Mise à jour du solde
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"Update client Set Solde_Client={soldeRestant} Where Mail_Client=\"{MainWindow.currentUser.Mail}\"";
                command.ExecuteNonQuery();

                //Ajout dans la table "commande" pour chaque recette.
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    //Date au format SQL.
                    string date = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00")
                        + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
                    command.CommandText = $"insert into commande values(\"{MainWindow.currentUser.Mail}\", \"{item.Key.Nom}\", {item.Value}" +
                            $", \'{date}\')";
                    command.ExecuteNonQuery();
                }
                MessageBox.Show($"La commande a bien été effectuée (solde restant : {soldeRestant} cook(s)). \nMerci de votre confiance. N'hésitez pas à noter l'application!");
                connection.Close();

                //Met à jour le nb de commande d'une recette (Popularité).
                MySqlConnection connection2 = new MySqlConnection(connectionString);
                connection2.Open();
                MySqlCommand command2 = connection2.CreateCommand();
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command2.CommandText = $"Update recette Set Popularité_Recette=Popularité_Recette + {item.Value} Where NomRecette_Recette=\"{item.Key.Nom}\"";
                    command2.ExecuteNonQuery();
                }
                connection2.Close();

                //Mise à jour des prix de vente.
                MySqlConnection connection3 = new MySqlConnection(connectionString);
                connection3.Open();
                MySqlCommand command3 = connection3.CreateCommand();
                MySqlDataReader rdr;
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command3.CommandText = $"select Popularité_Recette from recette where NomRecette_Recette=\"{item.Key.Nom}\"";
                    rdr = command3.ExecuteReader();
                    rdr.Read();
                    int pop = rdr.GetInt32(0);
                    rdr.Close();
                    if (pop > 10 && item.Key.Popularite <= 10)
                    {
                        command3.CommandText = $"Update recette Set PrixVente_Recette=PrixVente_Recette + 2 Where NomRecette_Recette=\"{item.Key.Nom}\"";
                        command3.ExecuteNonQuery();
                    }
                    else if (pop > 50 && item.Key.Popularite <= 50)
                    {
                        command3.CommandText = $"Update recette Set PrixVente_Recette=PrixVente_Recette + 5 Where NomRecette_Recette=\"{item.Key.Nom}\"";
                        command3.ExecuteNonQuery();
                    }
                }
                connection3.Close();

                //rémunération des CDR
                MySqlConnection connection4 = new MySqlConnection(connectionString);
                connection4.Open();
                MySqlCommand command4 = connection4.CreateCommand();
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command4.CommandText = $"update client set Solde_Client = Solde_Client + {item.Key.Remuneration} where " +
                        $"Mail_Client in (select Mail_Client from crée where Nomrecette_recette = \"{ item.Key.Nom}\")";
                    command4.ExecuteNonQuery();
                }
                connection4.Close();

                //Mise à jour des stocks
                MySqlConnection connection5 = new MySqlConnection(connectionString);
                connection5.Open();
                MySqlCommand command5 = connection5.CreateCommand();
                List<string> ingredients = new List<string>(); //Liste des ingrédients
                string req;
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    ingredients.Clear();
                    req = $"select NomProduit_Produit from compose where NomRecette_Recette = \"{item.Key.Nom}\";";
                    command5.CommandText = req;
                    rdr = command5.ExecuteReader();
                    while (rdr.Read())
                    {
                        ingredients.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    foreach (string i in ingredients)
                    {
                        command5.CommandText = $"select QttProduit_Compose from compose where NomProduit_Produit=\"{i}\"";
                        rdr = command5.ExecuteReader();
                        rdr.Read();
                        int qtt = rdr.GetInt32(0);
                        rdr.Close();
                        command5.CommandText = $"update produit set StockActuel_Produit = StockActuel_Produit - {qtt} where NomProduit_Produit=\"{i}\"";
                        command5.ExecuteNonQuery();
                    }
                }
                connection5.Close();
                Close(); //ferme la fenêtre de paiement.
            }
        }

        private void ButtonRecharger_Click(object sender, RoutedEventArgs e)
        {
            //Lien hypothétique vers une plateforme de paiement pour ajouter des cooks à son compte.
            //Pour l'instant, recharge juste de 100 cooks le compte en cours.
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"Update client Set Solde_Client={MainWindow.currentUser.Solde + 100} Where Mail_Client=\"{MainWindow.currentUser.Mail}\"";
            MainWindow.currentUser.Solde += 100;
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show($"+100 COOKS");
        }
    }
}