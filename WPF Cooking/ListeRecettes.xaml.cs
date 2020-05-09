using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Fenêtre de commande (portail client) des recettes validées.
    /// </summary>

    public partial class ListeRecettes : Window
    {
        /// <summary>
        /// Stocke les recettes du panier et leur nombre.
        /// </summary>
        public static Dictionary<Recette, int> compteRecettes = new Dictionary<Recette, int>();

        /// <summary>
        /// Liste des recettes du panier
        /// </summary>
        private static List<Recette> listeRecettes = new List<Recette>();

        /// <summary>
        /// Stocke les noms des recettes du panier et leur nombre (utilisé dans le récap)
        /// </summary>
        private static Dictionary<string, int> recettesNoms = new Dictionary<string, int>();

        public ListeRecettes()
        {
            InitializeComponent();
            recettesNoms.Clear(); // Remet le panier à 0.
            compteRecettes.Clear(); // Remet le compte des recettes à 0.
            List<Recette> recettes = new List<Recette>();

            #region Récupérer les recettes de la BDD

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = "Select * From recette where Validation_Recette = 1"; //N'affiche que les recettes validées.
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    recettes.Add(new Recette(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();

            #endregion Récupérer les recettes de la BDD

            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom}";
            lvRecettes.ItemsSource = recettes;
        }

        /// <summary>
        /// Permet d'ajouter une ou plusieurs (ctrl+click) recette(s) au panier, et actualise le prix total.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoutonAjout_Click(object sender, RoutedEventArgs e)
        {
            foreach (Recette recette in lvRecettes.SelectedItems)
            {
                textePrix.Text = Convert.ToString(decimal.Parse(textePrix.Text) + recette.PrixVente);
                if (recettesNoms.ContainsKey(recette.Nom))
                {
                    recettesNoms[recette.Nom]++;
                    compteRecettes[recette]++;
                }
                else
                {
                    listeRecettes.Add(recette);
                    recettesNoms.Add(recette.Nom, 1);
                    compteRecettes.Add(recette, 1);
                }
            }
            lvRecap.ItemsSource = null;
            lvRecap.ItemsSource = recettesNoms;
        }

        private void BoutonRetirer_Click(object sender, RoutedEventArgs e)
        {
            if (lvRecap.SelectedItem is null)
                MessageBox.Show("Veuillez sélectionner un produit à retirer du panier.");
            else
            {
                // Nom de la recette à supprimer/mettre à jour du panier.
                string index = lvRecap.SelectedItem.ToString().Split(',')[0].Substring(1);
                Recette r = listeRecettes.Find(x => x.Nom == index);
                textePrix.Text = Convert.ToString(decimal.Parse(textePrix.Text) - r.PrixVente);
                if (recettesNoms[index] == 1)
                {
                    recettesNoms.Remove(index);
                    listeRecettes.Remove(listeRecettes.Find(x => x.Nom == index));
                    compteRecettes.Remove(r);
                }
                else
                {
                    recettesNoms[index]--;
                    compteRecettes[r]--;
                }
                lvRecap.ItemsSource = null;
                lvRecap.ItemsSource = recettesNoms;
            }
        }

        private void Commander_Click(object sender, RoutedEventArgs e)
        {
            if (listeRecettes.Count == 0) MessageBox.Show("Votre panier est vide.");
            else
            {
                Paiement paiement = new Paiement();
                paiement.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Déconnecte l'utilisateur
            MainWindow.currentUser = new Client();
        }
    }
}