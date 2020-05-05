using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Administration.xaml
    /// </summary>
    public partial class Administration : Window
    {
        public static List<Recette> allRecettes = new List<Recette>();

        public Administration()
        {
            InitializeComponent();
            allRecettes.Clear();
            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom} (administrateur).";

            //Retrouver le CDR de la semaine
            //On récupère la date d'il y a 7 jours
            DateTime dateActuelle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan semaine = new TimeSpan(7, 0, 0, 0);
            DateTime dateVoulue = dateActuelle - semaine;
            string date = dateVoulue.Year.ToString("0000") + dateVoulue.Month.ToString("00") + dateVoulue.Day.ToString("00")
                        + dateVoulue.Hour.ToString("00") + dateVoulue.Minute.ToString("00") + dateVoulue.Second.ToString("00");

            //Retourne le mail du CDR de la semaine, son nom et son nb de recettes commandées dans la derniere semaine.
            string requete = $"select cr.Mail_Client, cl.Nom_Client, sum(nombreplats) as s from commande c join crée cr on cr.Nomrecette_Recette = c.Nomrecette_recette join client cl on cl.Mail_Client = cr.Mail_Client where c.Date_Commande > \'{date}\' group by cr.Mail_Client order by s desc limit 1";

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = command.ExecuteReader();
                //Si aucune commande n'a été passée dans la derniere semaine
                if (!rdr.Read()) TextBlockCDR.Text = "Aucune commande passée dans la dernière semaine.";
                else
                {
                    //sinon, affiche le CDR de la semaine
                    CDRSemaine c = new CDRSemaine(rdr.GetString(1), rdr.GetString(0), rdr.GetInt32(2));
                    TextBlockCDR.Text += c.ToString();
                }
                rdr.Close();

                //Top 5 des recettes par popularité.
                List<Recette> Top5 = new List<Recette>();
                command.CommandText = "select * from recette order by popularité_recette desc limit 5";
                rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    Top5.Add(new RecetteTop(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                ListViewTop5.ItemsSource = Top5;
                rdr.Close();

                //Toutes les recettes triées par popularité.
                command.CommandText = "select * from recette order by popularité_recette desc";
                rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    allRecettes.Add(new RecetteCDR(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                ListViewToutesRecettes.ItemsSource = allRecettes;
                rdr.Close();

                //CDR d'Or (le plus de commandes)
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
                    command.CommandText = $"select * from recette natural join crée where Mail_Client = \"{mailCdrOr}\" order by Popularité_recette desc limit 5";
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
                try
                {
                    string index = ListViewToutesRecettes.SelectedItem.ToString().Split('(')[0];
                    Recette r = allRecettes.Find(x => x.Nom == index.Trim());
                    var res = MessageBox.Show($"La recette \"{r.Nom}\" va etre supprimée. Cette opération est irréversible.\nÊtes-vous sur de vouloir continuer?",
                            "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.OK)
                    {
                        string requete = $"delete from recette where Nomrecette_recette = \"{r.Nom}\"";
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(requete, connection);
                        cmd.ExecuteNonQuery();
                        //On actualise la liste des recettes.
                        Close();
                        Administration administration = new Administration();
                        administration.Show();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                connection.Close();

                #endregion Suppression sécurisée de la recette de la BDD
            }
        }

        private void BoutonReapp_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BoutonValider_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BoutonClients_Click(object sender, RoutedEventArgs e)
        {
            ListeClients lc = new ListeClients();
            lc.Show();
        }
    }
}