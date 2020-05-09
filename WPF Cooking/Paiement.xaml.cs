using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        // On récupère la liste des recettes dans le panier et leur nombre.
        private Dictionary<Recette, int> recap = ListeRecettes.compteRecettes;

        public Paiement()
        {
            InitializeComponent();
            listBoxRecap.ItemsSource = recap;
            foreach (KeyValuePair<Recette, int> recette in recap)
            {
                prixTot += recette.Key.PrixVente * recette.Value;
            }
            // CultureInfo pour afficher un '.' au lieu d'une ',' dans le résultat. Autrement, peut
            // créer des conflits avec la représentation des decimal dans MySQL.
            TextBlockPrix.Text = $"Prix total : {prixTot.ToString(new CultureInfo("en-US"))} cook(s)";
            TextBlockSolde.Text = $"{MainWindow.currentUser.Solde.ToString(new CultureInfo("en-US"))} cook(s)";
            TextBlockPrixBis.Text = $"- {prixTot.ToString(new CultureInfo("en-US"))} cook(s)";
            soldeRestant = MainWindow.currentUser.Solde - prixTot;
            TextBlockSoldeRestant.Text = $"Solde restant : {soldeRestant.ToString(new CultureInfo("en-US"))} cook(s)";
        }

        private void ButtonPayer_Click(object sender, RoutedEventArgs e)
        {
            decimal solde = MainWindow.currentUser.Solde;

            if (solde - prixTot < 0)
                MessageBox.Show("Vous ne possédez pas assez de cooks pour réaliser cette commande. Veuillez recharger votre solde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                //Mise à jour du solde
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"Update client Set Solde_Client=\"{soldeRestant.ToString(new CultureInfo("en-US"))}\" Where Mail_Client=\"{MainWindow.currentUser.Mail}\"";
                command.ExecuteNonQuery();
                MainWindow.currentUser.Solde = soldeRestant;

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

                //Met à jour le nb de commande d'une recette (Popularité).
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command.CommandText = $"Update recette Set Popularité_Recette=Popularité_Recette + {item.Value} Where NomRecette_Recette=\"{item.Key.Nom}\"";
                    command.ExecuteNonQuery();
                }

                //Mise à jour des prix de vente.
                MySqlDataReader rdr;
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command.CommandText = $"select Popularité_Recette from recette where NomRecette_Recette=\"{item.Key.Nom}\"";
                    rdr = command.ExecuteReader();
                    rdr.Read();
                    int pop = rdr.GetInt32(0);
                    rdr.Close();
                    if (pop > 10 && item.Key.Popularite <= 10) // Si le nb de commandes était < 10 et mtnt est > 10
                    {
                        command.CommandText = $"Update recette Set PrixVente_Recette=PrixVente_Recette + 2 Where NomRecette_Recette=\"{item.Key.Nom}\"";
                        command.ExecuteNonQuery();
                    }
                    if (pop > 50 && item.Key.Popularite <= 50) // Idem pour 50
                    {
                        command.CommandText = $"Update recette Set PrixVente_Recette=PrixVente_Recette + 3 Where NomRecette_Recette=\"{item.Key.Nom}\"";
                        command.ExecuteNonQuery();
                    }
                }

                //rémunération des CDR
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    command.CommandText = $"update client set Solde_Client = Solde_Client + {item.Key.Remuneration} where " +
                        $"Mail_Client in (select Mail_Client from crée where Nomrecette_recette = \"{ item.Key.Nom}\")";
                    command.ExecuteNonQuery();
                }

                //Mise à jour des stocks
                List<string> ingredients = new List<string>(); //Liste des ingrédients
                string req;
                foreach (KeyValuePair<Recette, int> item in ListeRecettes.compteRecettes)
                {
                    ingredients.Clear();
                    req = $"select NomProduit_Produit from compose where NomRecette_Recette = \"{item.Key.Nom}\";";
                    command.CommandText = req;
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        ingredients.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    foreach (string i in ingredients)
                    {
                        command.CommandText = $"select QttProduit_Compose from compose where NomProduit_Produit=\"{i}\"";
                        rdr = command.ExecuteReader();
                        rdr.Read();
                        int qtt = rdr.GetInt32(0);
                        rdr.Close();
                        // date de dernière commande actualisée à aujourd'hui.
                        string date = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
                        command.CommandText = $"update produit set StockActuel_Produit = StockActuel_Produit - {qtt}, DateDerniereCommande_Produit = \"{date}\" where NomProduit_Produit=\"{i}\"";
                        command.ExecuteNonQuery();
                    }
                }
                Close(); // ferme la fenêtre de paiement.
            }
        }

        /// <summary>
        /// Dans ce POC, rajoute 100 cooks au client actuel. Lors de la réalisation du projet,
        /// doit ouvrir une fenêtre de paiement pour recharger le compte (avec conversion
        /// euros <=> cooks etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRecharger_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"Update client Set Solde_Client = Solde_Client + 100 Where Mail_Client=\"{MainWindow.currentUser.Mail}\"";
                MainWindow.currentUser.Solde += 100;
                command.ExecuteNonQuery();

                TextBlockSolde.Text = $"{MainWindow.currentUser.Solde.ToString(new CultureInfo("en-US"))} cook(s)";
                TextBlockPrixBis.Text = $"- {prixTot.ToString(new CultureInfo("en-US"))} cook(s)";
                soldeRestant = MainWindow.currentUser.Solde - prixTot;
                TextBlockSoldeRestant.Text = $"Solde restant : {soldeRestant.ToString(new CultureInfo("en-US"))} cook(s)";

                MessageBox.Show($"+100 COOKS");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
    }
}