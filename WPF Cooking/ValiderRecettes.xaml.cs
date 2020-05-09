using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Cooking
{
    /// <summary>
    /// Interaction logic for ValiderRecettes.xaml
    /// </summary>
    public partial class ValiderRecettes : Window
    {
        private List<Recette> listeRecettes = new List<Recette>();
        public static Recette selectionne = null;

        public ValiderRecettes()
        {
            InitializeComponent();
            DataContext = this;

            string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"select * from recette where Validation_Recette = \"false\"";
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listeRecettes.Add(new Recette(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4), Convert.ToBoolean(rdr.GetBoolean(5))));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
            DatagridRecettesAtt.ItemsSource = listeRecettes;
        }

        /// <summary>
        /// On n'affiche pas les colonnes Ingrédients, Validation et MailCréateur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatagridRecettesAtt_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Validation" || e.Column.Header.ToString() == "MailCreateur")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
        }

        private void BoutonSupprRecette_Click(object sender, RoutedEventArgs e)
        {
            Recette selectionne = (Recette)DatagridRecettesAtt.SelectedItem;
            if (DatagridRecettesAtt.SelectedItem is null)
                MessageBox.Show("Sélectionnez une recette à supprimer");
            else
            {
                string requete = "";
                var res = MessageBox.Show($"Vous avez choisi {selectionne.Nom}. Voulez-vous la supprimer définitivement ?",
                        "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.OK)
                {
                    requete = $"delete from recette where NomRecette_Recette = \"{selectionne.Nom}\"";
                    listeRecettes.Remove(selectionne);
                    FormulaireNewRecette.recettesEnAttente.Remove(selectionne);
                }
                string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
                if (requete != "")
                {
                    try
                    {
                        connection.Open();
                        MySqlCommand command = new MySqlCommand(requete, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();
                }
            }
            DatagridRecettesAtt.ItemsSource = null;
            DatagridRecettesAtt.ItemsSource = listeRecettes;
        }

        private void BoutonValiderRecette_Click(object sender, RoutedEventArgs e)
        {
            Recette selectionne = (Recette)DatagridRecettesAtt.SelectedItem;
            if (DatagridRecettesAtt.SelectedItem is null)
                MessageBox.Show("Sélectionnez une recette à valider");
            else
            {
                var res = MessageBox.Show($"Vous avez choisi {selectionne.Nom}. Voulez-vous la valider ?",
                        "Validation", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                if (res == MessageBoxResult.OK)
                {
                    listeRecettes.Remove(selectionne);
                    FormulaireNewRecette.recettesEnAttente.Remove(selectionne);

                    #region Actualise la valeur de Validation à 'True' dans la BDD.

                    // Actualise les tables Recette
                    string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
                    try
                    {
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand { Connection = connection };

                        string requete = $"update recette set Validation_Recette = 1 where NomRecette_Recette = \"{selectionne.Nom}\"";
                        cmd.CommandText = requete;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();

                    #endregion Actualise la valeur de Validation à 'True' dans la BDD.
                }
            }
            DatagridRecettesAtt.ItemsSource = null;
            DatagridRecettesAtt.ItemsSource = listeRecettes;
        }

        private void BoutonIngredRecette_Click(object sender, RoutedEventArgs e)
        {
            selectionne = (Recette)DatagridRecettesAtt.SelectedItem;
            if (selectionne is null)
                MessageBox.Show("Aucune recette selectionnée");
            else
            {
                IngredientsRecette i = new IngredientsRecette();
                i.Show();
            }
        }
    }
}