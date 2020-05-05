using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Cooking
{
    /// <summary>
    /// Interaction logic for ListeClients.xaml
    /// </summary>
    public partial class ListeClients : Window
    {
        private static List<Client> listeClients = new List<Client>();

        public ListeClients()
        {
            InitializeComponent();
            listeClients.Clear();
            string requete = "select * from client";
            string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(requete, connection);
                MySqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    listeClients.Add(new Client(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetDecimal(4), rdr.GetString(5)));
                }
                DataGridListeClients.ItemsSource = listeClients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridListeClients_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Password")
            {
                e.Column.Visibility = Visibility.Hidden;
            }
            if (e.Column.Header.ToString() == "Statut")
            {
                e.Column.Width = 60;
            }
        }

        private void BoutonSupprClient_Click(object sender, RoutedEventArgs e)
        {
            Client selectionne = (Client)DataGridListeClients.SelectedItem;
            if (DataGridListeClients.SelectedItem is null)
            {
                MessageBox.Show("Sélectionnez un client à supprimer");
            }
            else
            {
                string requete = "";
                if (selectionne.Statut.ToLower() == "cdr")
                {
                    var res = MessageBox.Show($"Vous avez choisi {selectionne.Nom} (cdr). Voulez-vous le supprimer définitivement (oui), ou le passer en client (non)",
                        "Attention", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.Yes)
                    {
                        requete = $"delete from client where mail_client=\"{selectionne.Mail}\"";
                        listeClients.Remove(selectionne);
                    }
                    else if (res == MessageBoxResult.No)
                    {
                        requete = $"update client set statut_client = \"client\" where mail_client=\"{selectionne.Mail}\"";
                        selectionne.Statut = "client";
                    }
                    string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    try
                    {
                        if (requete != "")
                        {
                            connection.Open();
                            MySqlCommand command = new MySqlCommand(requete, connection);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();
                }
                else
                {
                    var res = MessageBox.Show($"Vous avez choisi {selectionne.Nom} (client). Voulez-vous le supprimer définitivement?",
                        "Attention", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.OK)
                    {
                        requete = $"delete from client where mail_client=\"{selectionne.Mail}\"";
                        listeClients.Remove(selectionne);
                    }
                    string connectionString = "SERVER = localhost; PORT = 3306; DATABASE = cooking; UID = root; PASSWORD = maxime";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    try
                    {
                        if (requete != "")
                        {
                            connection.Open();
                            MySqlCommand command = new MySqlCommand(requete, connection);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    connection.Close();
                }
            }
            DataGridListeClients.ItemsSource = null;
            DataGridListeClients.ItemsSource = listeClients;
        }
    }
}