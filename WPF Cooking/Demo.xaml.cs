using MySql.Data.MySqlClient;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Demo.xaml
    /// </summary>
    public partial class Demo : Window
    {
        public Demo()
        {
            InitializeComponent();
            int nb;
            List<CDR> listeCdr = new List<CDR>();
            List<Produit> listeProd = new List<Produit>();
            string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("select count(*) from client", connection);
            try
            {
                connection.Open();
                var rdr = cmd.ExecuteReader();
                rdr.Read();
                nb = rdr.GetInt32(0);
                NbClients.Text = $"Nb de clients : {nb}";
                rdr.Close();

                cmd.CommandText = "select cl.Nom_Client, cl.Mail_Client, sum(NombrePlats) as s from commande c join crée cr on cr.Nomrecette_Recette = c.Nomrecette_recette join client cl on cl.Mail_Client = cr.Mail_Client group by cr.Mail_Client order by s desc";
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listeCdr.Add(new CDRSemaine(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2)));
                }
                ListeCdr.ItemsSource = listeCdr;
                rdr.Close();

                cmd.CommandText = "select count(*) from client where Statut_Client = 'cdr'";
                rdr = cmd.ExecuteReader();
                rdr.Read();
                NbCdr.Text = $"Nb de CDR : {rdr.GetInt32(0)}";
                rdr.Close();

                cmd.CommandText = "select count(*) from recette";
                rdr = cmd.ExecuteReader();
                rdr.Read();
                NbRecette.Text = $"Nb de recettes : {rdr.GetInt32(0)}";
                rdr.Close();

                cmd.CommandText = "select p.NomProduit_Produit, p.StockActuel_Produit, p.Quantite_Produit from produit p where StockActuel_Produit < 2 * StockMinimal_Produit";
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    listeProd.Add(new Produit(rdr.GetString(0), rdr.GetInt16(1), rdr.GetString(2)));
                }
                ListeProduits.ItemsSource = listeProd;
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            connection.Close();
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            string produit = SaisieProduit.Text;
            List<Produit> lp = new List<Produit>();
            string requete = $"select c.* from compose c natural join produit p where p.nomproduit_produit = '{produit}'";
            string connectionString = $"SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = {MainWindow.idBdd}; PASSWORD = {MainWindow.mdpBdd}";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(requete, connection);
            try
            {
                connection.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lp.Add(new Produit(rdr.GetString(0), rdr.GetInt32(2), rdr.GetString(3)));
                }
                LP.ItemsSource = lp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Double cliquer sur un produit de la listview le rentre dans la textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListeProduits_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SaisieProduit.Text = ListeProduits.SelectedItem.ToString().Split('(')[0].Trim();
        }
    }
}