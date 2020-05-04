using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour Administration.xaml
    /// </summary>
    public partial class Administration : Window
    {
        public Administration()
        {
            InitializeComponent();
            TextBlockNomCompte.Text = $"Compte de {MainWindow.currentUser.Nom}";
            CDR cdr = new CDR();
            cdr.NbRecettesSoumises = 5;
            cdr.Nom = "Marco";
            cdr.Mail = "mail";
            TextBlockCDR.Text += cdr.ToString();
            DateTime dateActuelle = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            TimeSpan semaine = new TimeSpan(7, 0, 0, 0);
            DateTime dateVoulue = dateActuelle - semaine;
            string date = dateVoulue.Year.ToString("0000") + dateVoulue.Month.ToString("00") + dateVoulue.Day.ToString("00")
                        + dateVoulue.Hour.ToString("00") + dateVoulue.Minute.ToString("00") + dateVoulue.Second.ToString("00");
            //Retourne le mail du CDR d'or, son nom et son nb de recettes
            string requete = $"select client.Nom_Client, crée.Mail_Client, sum(commande.NombrePlats) as nb from client natural join commande " +
                $"join crée on crée.NomRecette_Recette = commande.NomRecette_Recette where commande.Date_Commande > \'{date}\' order by nb desc limit 1";
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
                    //sinon, affiche le CDR d'or!
                    CDR c = new CDR(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2));
                    TextBlockCDR.Text += c.ToString();
                }
                rdr.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}