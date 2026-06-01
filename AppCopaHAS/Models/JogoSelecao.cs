using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopaHAS.Models
{
    //Classe JogoSelecao
    public class JogoSelecao
    {
        public int JogoId { get; set; }
        public int SelecaoId { get; set; }
        public int Gols { get; set; }        
        public int GolsDecisaoPenaltis { get; set; }        
    }
}