namespace WPF_Cooking
{
    public class Produit
    {
        public string Nom { get; set; }
        public string Quantite { get; set; }
        public string Unite { get; set; }

        public Produit(string nom, string quantite, string unite)
        {
            Nom = nom;
            Quantite = quantite;
            Unite = unite;
        }

        public Produit(string nom)
        {
            Nom = nom;
        }

        public override string ToString()
        {
            return $"{Nom} ({Quantite} {Unite})";
        }
    }
}