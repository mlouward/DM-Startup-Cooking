using MySql.Data.MySqlClient;
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
    /// Logique d'interaction pour ListeRecettes.xaml
    /// </summary>

    public partial class ListeRecettes : Window
    {

        static Dictionary<string, int> recettesNoms = new Dictionary<string, int>();
        static List<Recette> listeRecettes = new List<Recette>();
        public ListeRecettes()
        {
            recettesNoms.Clear(); //Remet le panier à 0
            InitializeComponent();
            List<Recette> recettes = new List<Recette>();
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";

            //Récupérer les recettes de la BDD
            MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = "Select * From recette";
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    //Liste pour y accéder en tant qu'objets.
                    recettes.Add(new Recette(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            connection.Close();
            lvRecettes.ItemsSource = recettes;
        }

        private void Commander_Click(object sender, RoutedEventArgs e)
        {
            List<string> panier = new List<string>();
            Paiement paiement = new Paiement();
            paiement.Show();
        }

        private void BoutonAjout_Click(object sender, RoutedEventArgs e)
        {
            foreach (Recette recette in lvRecettes.SelectedItems)
            {
                textePrix.Text = Convert.ToString(decimal.Parse(textePrix.Text) + recette.PrixVente);
                if (recettesNoms.ContainsKey(recette.Nom) && recettesNoms[recette.Nom] < 20)
                {
                    recettesNoms[recette.Nom]++;
                }
                else
                {
                    listeRecettes.Add(recette);
                    recettesNoms.Add(recette.Nom, 1);
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
                string index = lvRecap.SelectedItem.ToString().Split(',')[0].Substring(1);
                Recette r = listeRecettes.Find(x => x.Nom == index);
                textePrix.Text = Convert.ToString(decimal.Parse(textePrix.Text) - r.PrixVente);
                if (recettesNoms[index] == 1)
                {
                    recettesNoms.Remove(index);
                    listeRecettes.Remove(listeRecettes.Find(x => x.Nom == index));
                }
                else
                    recettesNoms[index]--;
                lvRecap.ItemsSource = null;
                lvRecap.ItemsSource = recettesNoms;
            }
        }

        private void textePrix_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            MessageBox.Show("cc");
            if (int.Parse(textePrix.Text) < 2)
                change.Text = "cook";
            else
                change.Text = "cooks";
        }
    }
}
