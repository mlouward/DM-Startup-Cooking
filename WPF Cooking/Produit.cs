namespace WPF_Cooking
{
    public class Produit
    {
        public string Nom { get; set; }
        public string Categorie { get; set; }
        public string Unite { get; set; }
        public int StockActuel { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public string NomFournisseur { get; set; }
        public string RefFournisseur { get; set; }

        public Produit(string nom, string categorie, string unite)
        {
            Nom = nom;
            Categorie = categorie;
            Unite = unite;
        }

        public Produit(string nom)
        {
            Nom = nom;
        }
    }
}