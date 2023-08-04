using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Ejemplo
{
    public class ResumenSolicitud
    {
        public int idTblSOlicitudes { get; set; }
        public Colaborador colaborador { get; set; }
        public List<Rubros> rubros { get; set; }
    }
}