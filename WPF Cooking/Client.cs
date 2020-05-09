namespace WPF_Cooking
{
    /// <summary>
    /// Client de l'application Cooking.
    /// </summary>
    public class Client
    {
        public string Mail { get; set; }
        public string Nom { get; set; }
        public string NumTel { get; set; }
        public string Password { get; set; }
        public decimal Solde { get; set; }

        /// <summary>
        /// Statut: "client"/"cdr"/"admin"
        /// </summary>
        public string Statut { get; set; }

        public Client()
        {
        }

        public Client(string mail, string nom, string numTel, decimal solde, string statut)
        {
            Mail = mail;
            Nom = nom;
            NumTel = numTel;
            Solde = solde;
            Statut = statut;
        }

        public Client(string mail, string nom, string numTel, string password, decimal solde, string statut) :
            this(mail, nom, numTel, solde, statut)
        {
            Password = password;
        }

        public override string ToString()
        {
            return $"\"{Mail}\", \"{Nom}\", \"{NumTel}\", \"{Password}\", {Solde}, \"{Statut}\"";
        }
    }
}