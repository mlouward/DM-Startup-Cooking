using System;
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
        public string MailCreateur { get; set; }

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
        {
            Nom = nom;
            Type = type;
            Descriptif = descriptif;
            PrixVente = prixVente;
            Popularite = popularite;
            Remuneration = popularite < 50 ? 2 : 4;
            MailCreateur = mailCreateur;
        }

        public override string ToString()
        {
            return $"{Nom} ({Type}) : {Descriptif}. Prix : {PrixVente.ToString(new CultureInfo("en-US"))} cooks";
        }
    }
}