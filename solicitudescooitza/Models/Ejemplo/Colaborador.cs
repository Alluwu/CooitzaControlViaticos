using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Ejemplo
{
    public class Colaborador
    {
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string codigoEmpleado { get; set; }
        public string gerencia { get; set; }
        public string categoria { get; set; }
        public string noCuenta { get; set; }
        public string periodoInicial { get; set; }
        public string periodoFinal { get; set; }
    }
}