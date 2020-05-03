using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cooking
{
    public class Client
    {
        public string Mail { get; set; }
        public string Nom { get; set; }
        public string NumTel { get; set; }
        public string Password { get; set; }
        public decimal Solde { get; set; }
        public string Statut { get; set; }
        public Client(string mail, string nom, string numTel, string password, decimal solde, string statut)
        {
            Mail = mail;
            Nom = nom;
            NumTel = numTel;
            Password = password;
            Solde = solde;
            Statut = statut;
        }

        public Client()
        {
        }
    }
}
