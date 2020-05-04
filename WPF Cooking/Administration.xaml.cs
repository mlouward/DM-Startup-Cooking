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
            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom} (administrateur).";

            //Retrouver le CDR de la semaine
            //On récupère la date d'il y a 7 jours
            DateTime dateActuelle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan semaine = new TimeSpan(7, 0, 0, 0);
            DateTime dateVoulue = dateActuelle - semaine;
            string date = dateVoulue.Year.ToString("0000") + dateVoulue.Month.ToString("00") + dateVoulue.Day.ToString("00")
                        + dateVoulue.Hour.ToString("00") + dateVoulue.Minute.ToString("00") + dateVoulue.Second.ToString("00");

            //Retourne le mail du CDR de la semaine, son nom et son nb de recettes commandées dans la derniere semaine.
            string requete = $"select client.Mail_Client, client.Nom_Client, count(distinct r.nomrecette_recette), sum(distinct popularité_recette) as nb " +
                $"from client natural join crée cr join recette r on r.nomrecette_recette = cr.nomrecette_recette " +
                $"join commande c on c.nomrecette_recette = r.nomrecette_recette where c.Date_Commande > \'{date}\' " +
                $"group by client.mail_client order by nb desc limit 1";

            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                //Si aucune commande n'a été passée dans la derniere semaine
                if (rdr.IsDBNull(0)) TextBlockCDR.Text = "Aucune commande passée dans la dernière semaine";
                else
                {
                    //sinon, affiche le CDR de la semaine
                    CDR c = new CDR(rdr.GetString(1), rdr.GetString(0), rdr.GetInt32(2));
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

                //Toutes les recettes
                command.CommandText = "select * from recette order by popularité_recette asc";
                rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    allRecettes.Add(new RecetteCDR(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(3), rdr.GetInt32(4)));
                }
                ListViewToutesRecettes.ItemsSource = allRecettes;
                rdr.Close();

                //CDR d'Or (all time)
                command.CommandText = "select * from recette order by popularité_recette asc";
            }
            catch (Exception)
            {
                throw;
            }
            connection.Close();
        }

        private void BoutonSuppr_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewToutesRecettes.SelectedItem is null)
                MessageBox.Show("Veuillez sélectionner une recette à supprimer.");
            else
            {
                #region Suppression de la recette de la BDD

                string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    string index = ListViewToutesRecettes.SelectedItem.ToString().Split('(')[0];
                    Recette r = allRecettes.Find(x => x.Nom == index);
                    MessageBox.Show(r.ToString());
                    var res = MessageBox.Show($"La recette \"{r.Nom}\" va etre supprimée. Cette opération est irréversible.\nÊtes-vous sur de vouloir continuer?",
                            "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.OK)
                    {
                        string requete = $"delete from recette where Nomrecette_recette = \"{r.Nom}\"";
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(requete, connection);
                        cmd.ExecuteNonQuery();
                        //On actualise la liste des recettes.
                        this.Close();
                        Administration administration = new Administration();
                        administration.Show();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                connection.Close();

                #endregion Suppression de la recette de la BDD
            }
        }

        private void BoutonReapp_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}