using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour IngredientsRecette.xaml
    /// </summary>
    public partial class IngredientsRecette : Window
    {
        public static List<Produit> ingredients { get; set; }
        public IngredientsRecette()
        {
            InitializeComponent();
            ingredients = new List<Produit>();
            Recette currentRecette = ValiderRecettes.selectionne;
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;

                //Recette
                string requete = $"select p.*, c.QttProduit_Compose from produit p natural join compose c where c.nomRecette_recette = \"{currentRecette.Nom}\"";
                cmd.CommandText = requete;
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
                throw;
            }
            connection.Close();
        }
    }
}