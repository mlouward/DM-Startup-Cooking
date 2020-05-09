namespace WPF_Cooking
{
    /// <summary>
    /// Cdr de la semaine, créé pour avoir une méthode ToString() différente.
    /// </summary>
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