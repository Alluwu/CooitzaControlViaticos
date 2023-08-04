using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Ejemplo
{
    public class Rubros
    {
        public int idCatRubros { get; set; }
        public string fecha { get; set; }
        public decimal desayuno { get; set; }
        public decimal almuerzo { get; set; }
        public decimal cena { get; set; }
        public decimal hospedaje { get; set; }
        public decimal totalDia { get; set; }
    }
}