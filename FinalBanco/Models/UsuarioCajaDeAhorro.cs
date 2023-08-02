using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalBanco.Models 
{ 
    public class UsuarioCajaDeAhorro
    {
        public int _id_caja { get; set; }
        public CajaDeAhorro caja { get; set; }
        public int _id_usuario { get; set; }
        public Usuario user { get; set; }
        public UsuarioCajaDeAhorro() { }
    }
}
