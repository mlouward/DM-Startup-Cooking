﻿namespace WPF_Cooking
{
    public class RecetteTop : Recette
    {
        public RecetteTop(string nom, string type, string descriptif, decimal prixVente, int popularite) : base(nom, type, descriptif, prixVente, popularite)
        {
        }
        public override string ToString()
        {
            return $"{Nom} : {PrixVente} cooks, {Popularite} commandes.";
        }
    }
}