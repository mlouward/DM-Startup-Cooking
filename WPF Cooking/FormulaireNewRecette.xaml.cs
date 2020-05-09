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
        /// <summary>
        /// Liste des recettes en attente de validation.
        /// </summary>
        public static List<Recette> recettesEnAttente = new List<Recette>();

        public FormulaireNewRecette()
        {
            InitializeComponent();
            unite1.SelectedItem = " ";
            unite2.SelectedItem = " ";
            unite3.SelectedItem = " ";
            unite4.SelectedItem = " ";
        }

        private void BoutonValiderRecette_Click(object sender, RoutedEventArgs e)
        {
            // Le cdr doit rentrer au moins un nom et un prix (decimal) pour que la recette soit soumise.
            if (TextBoxNomRecette.Text == "" || TextBoxPrixRecette.Text == "" || !decimal.TryParse(TextBoxPrixRecette.Text, out decimal prix))
            {
                MessageBox.Show("Veuillez saisir un nom et un prix (valide) pour la recette!");
            }
            else
            {
                List<Produit> ingredients = new List<Produit>();
                bool formulaireCorrect = true;
                while (formulaireCorrect)
                {
                    if (TextBoxIngredient1.Text != "" && int.TryParse(TextBoxQtt1.Text, out int q1))
                        ingredients.Add(new Produit(TextBoxIngredient1.Text, q1, unite1.Text));
                    else if (TextBoxIngredient1.Text != "")
                    {
                        MessageBox.Show($"La quantité saisie pour {TextBoxIngredient1.Text} n'est pas un entier valide.");
                        formulaireCorrect = false;
                        ingredients.Clear();
                        break;
                    }
                    else break;

                    if (TextBoxIngredient2.Text != "" && int.TryParse(TextBoxQtt2.Text, out int q2))
                        ingredients.Add(new Produit(TextBoxIngredient2.Text, q2, unite2.Text));
                    else if (TextBoxIngredient2.Text != "")
                    {
                        MessageBox.Show($"La quantité saisie pour {TextBoxIngredient2.Text} n'est pas un entier valide.");
                        formulaireCorrect = false;
                        ingredients.Clear();
                        break;
                    }
                    else break;

                    if (TextBoxIngredient3.Text != "" && int.TryParse(TextBoxQtt3.Text, out int q3))
                        ingredients.Add(new Produit(TextBoxIngredient3.Text, q3, unite3.Text));
                    else if (TextBoxIngredient3.Text != "")
                    {
                        MessageBox.Show($"La quantité saisie pour {TextBoxIngredient3.Text} n'est pas un entier valide.");
                        formulaireCorrect = false;
                        ingredients.Clear();
                        break;
                    }
                    else break;

                    if (TextBoxIngredient4.Text != "" && int.TryParse(TextBoxQtt4.Text, out int q4))
                    {
                        ingredients.Add(new Produit(TextBoxIngredient4.Text, q4, unite4.Text));
                        break; // Ici, tous les ingrédients sont valides.
                    }
                    else if (TextBoxIngredient4.Text != "")
                    {
                        MessageBox.Show($"La quantité saisie pour {TextBoxIngredient4.Text} n'est pas un entier valide.");
                        formulaireCorrect = false;
                        ingredients.Clear();
                        break;
                    }
                    else break;
                }
                if (formulaireCorrect)
                {
                    Recette recette = new RecetteCDR(TextBoxNomRecette.Text, TextBoxTypeRecette.Text, TextBoxDescriptif.Text,
                            prix, 0, MainWindow.currentUser.Mail, ingredients, false);
                    recettesEnAttente.Add(recette);

                    #region ajout dans la bdd en tant que recette en attente

                    string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    // Actualise les tables Recette et Compose
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
                                cmd.CommandText = $"insert into produit (Quantite_Produit, NomProduit_Produit, StockMax_Produit, StockMinimal_Produit, StockActuel_Produit, NomFournisseur_Produit, RefFournisseur_Produit, DateDerniereCommande_Produit) values ('{item.Unite}', \"{item.Nom}\",\"{6 * item.Quantite}\",\"{3 * item.Quantite}\", 0, '', 0, '{new DateTime(0)}')";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = $"update produit set StockMax_Produit=StockMax_Produit + 2*{item.Quantite}, StockMinimal_Produit=StockMinimal_Produit/2+3*{item.Quantite}";
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //Recette
                        string requete = $"insert into recette values (\"{recette.Nom}\", \"{recette.Type}\", \"{recette.Descriptif}\", \"{recette.PrixVente.ToString(new CultureInfo("en-US"))}\", {recette.Popularite}, 0)";
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
                        MessageBox.Show("Merci de votre proposition! Votre recette sera évaluée par Cooking." +
                        "\nSi elle est validée, vous pourrez la voir dans la liste des recettes disponibles très bientôt!");
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();

                    #endregion ajout dans la bdd en tant que recette en attente
                }
            }
        }
    }
}