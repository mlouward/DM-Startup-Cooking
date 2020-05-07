using System;

namespace WPF_Cooking
{
    public class Produit
    {
        public string Nom { get; set; }
        public int Quantite { get; set; }
        public string Unite { get; set; }
        public int StockActuel { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public string NomFournisseur { get; set; }
        public DateTime DateDerniereCommande { get; set; }

        public Produit(string nom)
        {
            Nom = nom;
        }

        public Produit(string nom, int quantite, string unite) : this(nom)
        {
            Quantite = quantite;
            Unite = unite;
        }

        public Produit(string nom, int quantite, string unite, int stockActuel, int stockMax, int stockMin, string nomFournisseur, DateTime dateDerniereCommande)
        : this(nom, quantite, unite)
        {
            StockActuel = stockActuel;
            StockMax = stockMax;
            StockMin = stockMin;
            NomFournisseur = nomFournisseur;
            //DateTime date = new DateTime(
                //int.Parse(dateDerniereCommande.Substring(0, 4)), //Année
                //int.Parse(dateDerniereCommande.Substring(4, 2)), // Mois
                //int.Parse(dateDerniereCommande.Substring(6, 2))); // Jour
            DateDerniereCommande = dateDerniereCommande;
        }

        public override string ToString()
        {
            return $"{Nom} ({Quantite} {Unite})";
        }
    }
}