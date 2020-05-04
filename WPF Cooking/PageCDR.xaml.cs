using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour PageCDR.xaml
    /// </summary>
    public partial class PageCDR : Window
    {
        public PageCDR()
        {
            InitializeComponent();
            List<Recette> recettes = new List<Recette>();

            #region Récupère les recettes d'un CDR dans la BDD

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                string requete = $"select * from recette natural join crée where Mail_Client = \"{MainWindow.currentUser.Mail}\"";
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    //Liste pour y accéder en tant qu'objets.
                    recettes.Add(new RecetteCDR(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4), rdr.GetString(5)));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            connection.Close();

            #endregion Récupère les recettes d'un CDR dans la BDD

            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom}";
            TextBlockSoldeCompte.Text = $"Mon solde : {MainWindow.currentUser.Solde} cooks";
            ListViewRecettes.ItemsSource = recettes;
        }

        private void BoutonAjoutRecette_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}