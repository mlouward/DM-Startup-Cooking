using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cooking
{
    class CDRSemaine : CDR
    {

        public CDRSemaine(string mail, int nbCommandes) : base(mail, nbCommandes)
        {
        }
        public override string ToString()
        {
            return $"{Mail} : {NbCommandes} commandes";
        }
    }
}
