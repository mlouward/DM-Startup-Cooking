using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cooking
{
    public class Recette
    {

        public string Nom { get; set; }
        public string Type { get; set; }
        public string Descriptif { get; set; }
        public decimal PrixVente { get; set; }
        public int Popularite { get; set; }
        public Recette(string nom, string type, string descriptif, decimal prixVente, int popularite)
        {
            Nom = nom;
            Type = type;
            Descriptif = descriptif;
            PrixVente = prixVente;
            Popularite = popularite;
        }
        public override string ToString()
        {
            return $"{Nom} ({Type}) : {Descriptif}. Prix : {PrixVente} cooks";
        }
    }
}
