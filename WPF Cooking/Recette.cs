using System.Collections.Generic;
using System.Globalization;

namespace WPF_Cooking
{
    public class Recette
    {
        public string Nom { get; set; }
        public string Type { get; set; }
        public string Descriptif { get; set; }
        public decimal PrixVente { get; set; }
        public int Popularite { get; set; }
        public int Remuneration { get; set; }
        public bool Validation { get; set; }
        public string MailCreateur { get; set; }
        public List<Produit> Ingredients { get; set; }

        public Recette(string nom, string type, string descriptif, decimal prixVente, int popularite)
        {
            Nom = nom;
            Type = type;
            Descriptif = descriptif;
            PrixVente = prixVente;
            Popularite = popularite;
            Remuneration = popularite < 50 ? 2 : 4;
        }

        public Recette(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur)
            : this(nom, type, descriptif, prixVente, popularite)
        {
            Remuneration = popularite < 50 ? 2 : 4;
            MailCreateur = mailCreateur;
        }

        public Recette(string nom, string type, string descriptif, decimal prixVente, int popularite, bool validation)
             : this(nom, type, descriptif, prixVente, popularite)
        {
            Remuneration = popularite < 50 ? 2 : 4;
            Validation = validation;
        }

        public override string ToString()
        {
            return $"{Nom} ({Type}) : {Descriptif}. Prix : {PrixVente.ToString(new CultureInfo("en-US"))} cooks";
        }
    }
}