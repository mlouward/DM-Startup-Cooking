namespace WPF_Cooking
{
    public class CDR : Client
    {
        public int NbRecettesSoumises { get; set; }
        public int NbCommandes { get; set; }

        public CDR(string mail, string nom, string numTel, string password, decimal solde, string statut, int nbRecettesSoumises)
            : base(mail, nom, numTel, password, solde, statut)
        {
            NbRecettesSoumises = nbRecettesSoumises;
        }

        public CDR(string mail, string nom, string numTel, string password, decimal solde, string statut)
            : base(mail, nom, numTel, password, solde, statut)
        {
        }

        public CDR(int nbRecettesSoumises)
        {
            NbRecettesSoumises = nbRecettesSoumises;
        }

        public CDR()
        {
        }

        public CDR(string mail, int nbRecettes)
        {
            Mail = mail;
            NbRecettesSoumises = nbRecettes;
        }
        public CDR(string nom, string mail, int nbRecettes) : this(mail, nbRecettes)
        {
            Nom = nom;
        }

        public override string ToString()
        {
            return $"{Nom} ({Mail}), {NbRecettesSoumises} recette(s)";
        }
    }
}