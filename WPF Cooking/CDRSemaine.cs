namespace WPF_Cooking
{
    internal class CDRSemaine : CDR
    {
        public CDRSemaine(string nom, string mail, int nbCommandes) : base(nom, mail, nbCommandes)
        {
        }

        public override string ToString()
        {
            return $"{Nom} ({Mail}) : {NbCommandes} commandes";
        }
    }
}