using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopaHAS.Models
{
    //Classe Estadio
    public class Estadio
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public decimal Capacidade { get; set; }        
    }
}