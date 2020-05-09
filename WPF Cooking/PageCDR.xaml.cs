using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Page des CDR : liste de eur recettes et statut (En attente ou Validée), et outil de création de Recette.
    /// </summary>
    public partial class PageCDR : Window
    {
        public PageCDR()
        {
            InitializeComponent();
            List<Recette> recettes = new List<Recette>();

            #region Récupère les recettes d'un CDR dans la BDD

            string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = $"select * from recette natural join crée where Mail_Client = \"{MainWindow.currentUser.Mail}\"";
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    recettes.Add(new RecetteCDR(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4), rdr.GetString(6), rdr.GetBoolean(5)));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();

            #endregion Récupère les recettes d'un CDR dans la BDD

            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom} (CDR).";
            TextBlockSoldeCompte.Text = $"Mon solde : {MainWindow.currentUser.Solde} cooks";
            ListViewRecettes.ItemsSource = recettes;
        }

        private void BoutonAjoutRecette_Click(object sender, RoutedEventArgs e)
        {
            // Ouvre une page avec un formulaire de création de recette (Comment faire pour les ingrédients sans
            // savoir leur nombre à l'avance ?)
            FormulaireNewRecette f = new FormulaireNewRecette();
            f.Show();
        }

        /// <summary>
        /// Permet aux CDR de supprimer une de leur recettes s'ils le veulent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoutonSupprRecette_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewRecettes.SelectedItem is null) MessageBox.Show("Aucune recette à supprimer n'a été sélectionnée!");
            else
            {
                #region Supprime une recette de la BDD (un créateur ne peut supprimer que ses recettes)

                Recette toDelete = (Recette)ListViewRecettes.SelectedItem;
                string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}"; MySqlConnection connection = new MySqlConnection(connectionString);
                var res = MessageBox.Show($"La recette \"{toDelete.Nom}\" va etre supprimée. Cette opération est irréversible.\nÊtes-vous sur de vouloir continuer?",
                    "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.OK)
                {
                    string requete = $"delete from recette where Nomrecette_recette = \"{toDelete.Nom}\"";
                    try
                    {
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(requete, connection);
                        cmd.ExecuteNonQuery();
                        // On actualise la liste des recettes en fermant/rouvrant la fenêtre.
                        Close();
                        PageCDR pageCDR = new PageCDR();
                        pageCDR.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();
                }

                #endregion Supprime une recette de la BDD (un créateur ne peut supprimer que ses recettes)
            }
        }
    }
}