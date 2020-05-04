using System.Collections.Generic;

namespace WPF_Cooking
{
    public class RecetteCDR : Recette
    {
        public List<Produit> Ingredients { get; set; }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur)
            : base(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur, List<Produit> ingredients)
            : this(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
            Ingredients = ingredients;
        }

        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite) : base(nom, type, descriptif, prixVente, popularite)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $" (commandée {Popularite} fois)";
        }
        public string AfficherIngrédients()
        {
            string s = "";
            foreach (Produit item in Ingredients)
            {
                s += item.ToString() + "\n";
            }
            return s;
        }
    }
}