using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Administration.xaml
    /// </summary>
    public partial class Administration : Window
    {
        /// <summary>
        /// Liste de toutes les recettes disponibles sur Cooking
        /// </summary>
        public static List<Recette> allRecettes = new List<Recette>();

        /// <summary>
        /// Liste des produits à réapprovisionner
        /// </summary>
        private static List<Produit> listReapp = new List<Produit>();

        public Administration()
        {
            InitializeComponent();
            allRecettes.Clear();
            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom} (administrateur).";

            // Retrouver le CDR de la semaine
            // On récupère la date d'il y a 7 jours
            DateTime dateActuelle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan semaine = new TimeSpan(7, 0, 0, 0);
            DateTime dateVoulue = dateActuelle - semaine;
            string date = dateVoulue.Year.ToString("0000") + dateVoulue.Month.ToString("00") + dateVoulue.Day.ToString("00")
                        + dateVoulue.Hour.ToString("00") + dateVoulue.Minute.ToString("00") + dateVoulue.Second.ToString("00");

            // Retourne le mail du CDR de la semaine, son nom et son nb de recettes commandées dans la derniere semaine.
            string requete = $"select cr.Mail_Client, cl.Nom_Client, sum(nombreplats) as s from commande c join crée cr on cr.Nomrecette_Recette = c.Nomrecette_recette join client cl on cl.Mail_Client = cr.Mail_Client where c.Date_Commande > \'{date}\' group by cr.Mail_Client order by s desc limit 1";

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = command.ExecuteReader();
                // Si la requête ne retourne rien
                if (!rdr.Read()) TextBlockCDR.Text = "Aucune commande passée dans la dernière semaine.";
                else
                {
                    // sinon, affiche le CDR de la semaine
                    CDRSemaine c = new CDRSemaine(rdr.GetString(1), rdr.GetString(0), rdr.GetInt32(2));
                    TextBlockCDR.Text += c.ToString();
                }
                rdr.Close();

                // Top 5 des recettes par popularité.
                List<Recette> Top5 = new List<Recette>();
                command.CommandText = "select * from recette where Validation_Recette = 1 order by popularité_recette desc limit 5";
                rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Top5.Add(new RecetteTop(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                ListViewTop5.ItemsSource = Top5;
                rdr.Close();

                // Toutes les recettes triées par popularité.
                command.CommandText = "select * from recette where Validation_Recette = 1 order by popularité_recette desc";
                rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    allRecettes.Add(new RecetteCDR(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4), Convert.ToBoolean(rdr.GetBoolean(5))));
                }
                ListViewToutesRecettes.ItemsSource = allRecettes;
                rdr.Close();

                // CDR d'Or (le plus de commandes)
                command.CommandText = "select cl.Nom_Client, cr.Mail_Client, sum(NombrePlats) as s from commande c join crée cr on cr.Nomrecette_Recette = c.Nomrecette_recette join client cl on cl.Mail_Client = cr.Mail_Client group by cr.Mail_Client order by s desc limit 1";
                rdr = command.ExecuteReader();
                if (rdr.Read())
                {
                    string mailCdrOr = rdr.GetString(1);
                    CDRSemaine cdr = new CDRSemaine(rdr.GetString(0), mailCdrOr, rdr.GetInt32(2));
                    TextBlockCDROr.Text += cdr;
                    rdr.Close();
                    //Liste des 5 meilleures recettes du CDR d'or
                    List<Recette> recettesCdrOr = new List<Recette>();
                    command.CommandText = $"select * from recette natural join crée where Mail_Client = \"{mailCdrOr}\" and Validation_Recette = 1 order by Popularité_recette desc limit 5";
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        recettesCdrOr.Add(new RecetteTop(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                    }
                    ListViewCdrOrTop5.ItemsSource = recettesCdrOr;
                    rdr.Close();
                }
                else
                {
                    TextBlockCDROr.Text = "Aucun cdr d'or.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }

        private void BoutonSuppr_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewToutesRecettes.SelectedItem is null)
                MessageBox.Show("Veuillez sélectionner une recette à supprimer.");
            else
            {
                #region Suppression sécurisée de la recette de la BDD

                string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                MySqlConnection connection = new MySqlConnection(connectionString);
                // Retourne le nom de la recette sélectionnée.
                string index = ListViewToutesRecettes.SelectedItem.ToString().Split('(')[0];
                // On récupère ensuite l'objet Recette.
                Recette r = allRecettes.Find(x => x.Nom == index.Trim());
                var res = MessageBox.Show($"La recette \"{r.Nom}\" va etre supprimée. Cette opération est irréversible.\nÊtes-vous sur de vouloir continuer?",
                        "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res == MessageBoxResult.OK)
                {
                    try
                    {
                        string requete = $"delete from recette where Nomrecette_recette = \"{r.Nom}\"";
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(requete, connection);
                        cmd.ExecuteNonQuery();
                        // On actualise la liste des recettes en fermant et rouvrant la fenêtre
                        Close();
                        Administration administration = new Administration();
                        administration.Show();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    connection.Close();
                }

                #endregion Suppression sécurisée de la recette de la BDD
            }
        }

        /// <summary>
        /// Divise par 2 les QttMax et Min des produits non utilisés depuis plus de 30 jours et génère le
        /// fichier XML de commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoutonReapp_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateActuelle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan mois = new TimeSpan(30, 0, 0, 0);
            DateTime dateVoulue = dateActuelle - mois;
            // date au format de MySQL (il y a 30 jours)
            string date = dateVoulue.Year.ToString("0000") + dateVoulue.Month.ToString("00") + dateVoulue.Day.ToString("00")
                        + dateVoulue.Hour.ToString("00") + dateVoulue.Minute.ToString("00") + dateVoulue.Second.ToString("00");

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string requete = $" update produit set StockMinimal_Produit = StockMinimal_Produit / 2, StockMax_Produit = StockMax_Produit / 2 where DateDerniereCommande_Produit < '{date}'";
            string requete2 = $"select * from produit where StockActuel_Produit < StockMinimal_Produit order by NomFournisseur_Produit, NomProduit_Produit;";
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(requete, connection);
                cmd.ExecuteNonQuery();
                cmd.CommandText = requete2;
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Produit p = new Produit(rdr.GetString(0), rdr.GetString(2), rdr.GetInt32(3), rdr.GetInt32(7), rdr.GetInt32(4), rdr.GetString(5), rdr.GetDateTime(8), rdr.GetInt32(6));
                    p.QttACommander = p.StockMax - p.StockActuel;
                    listReapp.Add(p);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();

            CommandeXml();
        }

        private void BoutonValider_Click(object sender, RoutedEventArgs e)
        {
            ValiderRecettes v = new ValiderRecettes();
            v.Show();
        }

        private void BoutonClients_Click(object sender, RoutedEventArgs e)
        {
            ListeClients lc = new ListeClients();
            lc.Show();
        }

        /// <summary>
        /// Génère le fichier de commande XML à partir de la liste listReapp.
        /// </summary>
        private static void CommandeXml()
        {
            try
            {
                // Création de la racine "Commande"
                XmlRootAttribute root = new XmlRootAttribute("Commande");
                XmlSerializer serializer = new XmlSerializer(typeof(List<Produit>), root);
                XmlElement myElement = new XmlDocument().CreateElement("Commande", "ns");
                TextWriter wr = new StreamWriter("Xml//commande.xml");
                // On sérialise la liste des produits qu'il faut racheter.
                serializer.Serialize(wr, listReapp);
                wr.Close();
                MessageBox.Show("Les quantités ont été mises à jour!" + $" Le fichier de commande peut etre trouvé ici :\n{ Path.GetFullPath("Xml//commande.xml")}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Déconnecte l'utilisateur
            MainWindow.currentUser = new Client();
        }
    }
}