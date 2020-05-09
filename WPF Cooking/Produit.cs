using System;
using System.Xml.Serialization;

namespace WPF_Cooking
{
    /// <summary>
    /// Classe définissant les produits disponibles pour réaliser les recettes (ingrédients).
    /// </summary>
    public class Produit
    {
        public string Nom { get; set; }
        public int Quantite { get; set; }

        /// <summary>
        /// Unité, à choisir entre 'g', 'mL' et ' '
        /// </summary>
        public string Unite { get; set; }

        public int StockActuel { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }

        [XmlElement("QuantitéÀCommander")]
        public int QttACommander { get; set; }

        public string NomFournisseur { get; set; }
        public int RefFournisseur { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime DateDerniereCommande { get; set; }

        public Produit()
        {
        }
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
            DateDerniereCommande = dateDerniereCommande;
        }

        public Produit(string nom, string unite, int stockActuel, int stockMin, int stockMax, string nomFournisseur, DateTime dateDerniereCommande, int refFournisseur)
        {
            Nom = nom;
            Unite = unite;
            StockActuel = stockActuel;
            StockMin = stockMin;
            StockMax = stockMax;
            NomFournisseur = nomFournisseur;
            DateDerniereCommande = dateDerniereCommande;
            RefFournisseur = refFournisseur;
        }

        public override string ToString()
        {
            return $"{Nom} ({Quantite} {Unite})";
        }
    }
}