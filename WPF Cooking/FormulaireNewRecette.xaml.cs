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

                if (TextBoxIngredient1.Text != "") ingredients.Add(new Produit(TextBoxIngredient1.Text, int.Parse(TextBoxQtt1.Text), unite1.Text));
                if (TextBoxIngredient2.Text != "") ingredients.Add(new Produit(TextBoxIngredient2.Text, int.Parse(TextBoxQtt2.Text), unite2.Text));
                if (TextBoxIngredient3.Text != "") ingredients.Add(new Produit(TextBoxIngredient3.Text, int.Parse(TextBoxQtt3.Text), unite3.Text));
                if (TextBoxIngredient4.Text != "") ingredients.Add(new Produit(TextBoxIngredient4.Text, int.Parse(TextBoxQtt4.Text), unite4.Text));

                RecetteCDR recette = new RecetteCDR(TextBoxNomRecette.Text, TextBoxTypeRecette.Text, TextBoxDescriptif.Text,
                    prix, 0, MainWindow.currentUser.Mail, ingredients, false);

                MessageBox.Show("Merci de votre proposition! Votre recette sera évaluée par Cooking." +
                    "\nSi elle est validée, vous pourrez la voir dans la liste des recettes disponibles très bientôt!");

                Close();
            }
        }
    }
}