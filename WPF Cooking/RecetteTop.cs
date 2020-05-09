using System.Globalization;

namespace WPF_Cooking
{
    /// <summary>
    /// Définit une classe pour les recettes affichée sur la page administration.
    /// Utilisée seulement pour définir un ToString() différent dans les listview.
    /// </summary>
    public class RecetteTop : Recette
    {
        public RecetteTop(string nom, string type, string descriptif, decimal prixVente, int popularite) : base(nom, type, descriptif, prixVente, popularite)
        {
        }

        public override string ToString()
        {
            return $"{Nom} : {PrixVente.ToString(new CultureInfo("en-US"))} cooks, {Popularite} commandes.";
        }
    }
}