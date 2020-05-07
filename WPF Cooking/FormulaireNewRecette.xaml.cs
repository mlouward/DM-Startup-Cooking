using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour FormulaireNewRecette.xaml
    /// </summary>
    public partial class FormulaireNewRecette : Window
    {
        public static List<Recette> recettesEnAttente = new List<Recette>();

        public FormulaireNewRecette()
        {
            InitializeComponent();
        }

        private void BoutonValiderRecette_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxNomRecette.Text == "" || TextBoxPrixRecette.Text == "" || !decimal.TryParse(TextBoxPrixRecette.Text, out decimal prix))
            {
                MessageBox.Show("Veuillez rentrer un nom et un prix (valide) pour la recette!");
            }
            else
            {
                List<Produit> ingredients = new List<Produit>();

                if (TextBoxIngredient1.Text != "") ingredients.Add(new Produit(TextBoxIngredient1.Text, int.Parse(TextBoxQtt1.Text), unite1.Text));
                if (TextBoxIngredient2.Text != "") ingredients.Add(new Produit(TextBoxIngredient2.Text, int.Parse(TextBoxQtt2.Text), unite2.Text));
                if (TextBoxIngredient3.Text != "") ingredients.Add(new Produit(TextBoxIngredient3.Text, int.Parse(TextBoxQtt3.Text), unite3.Text));
                if (TextBoxIngredient4.Text != "") ingredients.Add(new Produit(TextBoxIngredient4.Text, int.Parse(TextBoxQtt4.Text), unite4.Text));

                RecetteCDR recette = new RecetteCDR(TextBoxNomRecette.Text, TextBoxTypeRecette.Text, TextBoxDescriptif.Text,
                    prix, 0, MainWindow.currentUser.Mail, ingredients, false);

                MessageBox.Show("Merci de votre proposition! Votre recette sera évaluée par Cooking." +
                    "\nSi elle est validée, vous pourrez la voir dans la liste des recettes disponibles très bientôt!");

                #region TEMPORAIRE (test ajout recette). Cette région ira dans le portail admin.

                string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                MySqlConnection connection = new MySqlConnection(connectionString);
                //Actualise les tables Recette et Compose
                try
                {
                    connection.Open();
                    var cmd = new MySqlCommand();
                    cmd.Connection = connection;

                    //les Ingrédients sont ajoutés à la table des ingrédients s'ils n'y existaient pas.
                    List<string> produitsDispos = new List<string>(); //Liste des produits dans la bdd
                    cmd.CommandText = "select NomProduit_Produit from produit";
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        produitsDispos.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    foreach (Produit item in ingredients)
                    {
                        if (!produitsDispos.Contains(item.Nom))
                        {
                            cmd.CommandText = $"insert into produit (NomProduit_Produit) values (\"{item.Nom}\")";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    //Recette
                    string requete = $"insert into recette values (\"{recette.Nom}\", \"{recette.Type}\", \"{recette.Descriptif}\", \"{recette.PrixVente.ToString(new CultureInfo("en-US"))}\", {recette.Popularite}, \"{0}\")";
                    cmd.CommandText = requete;
                    cmd.ExecuteNonQuery();

                    //Compose
                    foreach (Produit item in ingredients)
                    {
                        cmd.CommandText = $"insert into compose values (\"{recette.Nom}\", \"{item.Nom}\", {item.Quantite}, \"{item.Unite}\")";
                        cmd.ExecuteNonQuery();
                    }

                    //Crée
                    cmd.CommandText = $"insert into crée values(\"{MainWindow.currentUser.Mail}\", \"{recette.Nom}\")";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                connection.Close();

                #endregion TEMPORAIRE (test ajout recette). Cette région ira dans le portail admin.

                Close();
            }
        }
    }
}