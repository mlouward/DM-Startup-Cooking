namespace WPF_Cooking
{
    /// <summary>
    /// Créateur de Recette: Client particulier, qui peut accéder au portail CDR et créer des recettes.
    /// </summary>
    public class CDR : Client
    {
        public int NbRecettesSoumises { get; set; }
        public int NbCommandes { get; set; }

        public CDR()
        {
        }

        public CDR(int nbRecettesSoumises)
        {
            NbRecettesSoumises = nbRecettesSoumises;
        }

        public CDR(string mail, int nbCommandes)
        {
            Mail = mail;
            NbCommandes = nbCommandes;
        }

        public CDR(string nom, string mail, int nbCommandes) : this(mail, nbCommandes)
        {
            Nom = nom;
        }

        public CDR(string mail, string nom, string numTel, string password, decimal solde, string statut)
            : base(mail, nom, numTel, password, solde, statut)
        {
        }

        public CDR(string mail, string nom, string numTel, string password, decimal solde, string statut, int nbRecettesSoumises)
            : base(mail, nom, numTel, password, solde, statut)
        {
            NbRecettesSoumises = nbRecettesSoumises;
        }

        public override string ToString()
        {
            return $"{Nom} ({Mail}), {NbRecettesSoumises} recette(s)";
        }
    }
}