using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Fenêtre montrant les ingrédients de la recette en attente sélectionnée.
    /// </summary>
    public partial class IngredientsRecette : Window
    {
        /// <summary>
        /// Liste des ingrédients de la recette sélectionnée.
        /// </summary>
        private List<Produit> ingredients = new List<Produit>();

        public IngredientsRecette()
        {
            InitializeComponent();
            // On récupère la recette séléctionnée dans la fenêtre précédente.
            Recette currentRecette = ValiderRecettes.selectionne;
            Title = $"Ingrédients de {currentRecette.Nom}";
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = $"select p.*, c.QttProduit_Compose from produit p natural join compose c where c.nomRecette_recette = \"{currentRecette.Nom}\"";
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(requete, connection);

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ingredients.Add(new Produit(rdr.GetString(0), rdr.GetInt32(9), rdr.GetString(2), rdr.GetInt32(3), rdr.GetInt32(4), rdr.GetInt32(7), rdr.GetString(5), rdr.GetDateTime(8)));
                }
                DGIngredients.ItemsSource = ingredients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
    }
}