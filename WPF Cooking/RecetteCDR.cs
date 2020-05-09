using System.Collections.Generic;

namespace WPF_Cooking
{
    /// <summary>
    /// Recette avec une liste d'ingrédients soumise par un CDR.
    /// </summary>
    public class RecetteCDR : Recette
    {
        public List<Produit> Ingredients { get; set; }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite) : base(nom, type, descriptif, prixVente, popularite)
        {
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur)
            : base(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur, bool val)
            : this(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
            Validation = val;
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, bool val)
            : base(nom, type, descriptif, prixVente, popularite)
        {
            Validation = val;
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur, List<Produit> ingredients, bool v)
            : this(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
            Ingredients = ingredients;
            Validation = v;
        }

        public override string ToString()
        {
            string val = Validation ? "Validée" : "En attente";
            return base.ToString() + $", commandée {Popularite} fois ({val})";
        }
    }
}