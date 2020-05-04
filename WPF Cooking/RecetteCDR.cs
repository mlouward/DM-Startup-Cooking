namespace WPF_Cooking
{
    public class RecetteCDR : Recette
    {
        public RecetteCDR(string nom, string type, string descriptif, decimal prixVente, int popularite, string mailCreateur)
            : base(nom, type, descriptif, prixVente, popularite, mailCreateur)
        {
        }

        public override string ToString()
        {
            return base.ToString() + $" (commandée {Popularite} fois)";
        }
    }
}