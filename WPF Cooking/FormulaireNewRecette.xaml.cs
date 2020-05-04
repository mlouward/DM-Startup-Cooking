using System.Windows;

namespace WPF_Cooking
{
    /// <summary>
    /// Logique d'interaction pour FormulaireNewRecette.xaml
    /// </summary>
    public partial class FormulaireNewRecette : Window
    {
        public FormulaireNewRecette()
        {
            InitializeComponent();
        }

        private void BoutonValiderRecette_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxNomRecette.Text == "" || TextBoxPrixRecette.Text == "" || !decimal.TryParse(TextBoxPrixRecette.Text, out decimal prix))
            {
                MessageBox.Show("Veuillez rentrer un nom et un prix (valide) pour la recette!");
            }
            else
            {
                RecetteCDR recette = new RecetteCDR(TextBoxNomRecette.Text, TextBoxTypeRecette.Text, TextBoxDescriptif.Text, prix, 0, MainWindow.currentUser.Mail);
                MessageBox.Show("Merci de votre proposition! Votre recette sera évaluée par Cooking." +
                    "\nSi elle est validée, vous pourrez la voir dans la liste des recettes disponibles très bientôt!");
                this.Close();
            }
        }
    }
}