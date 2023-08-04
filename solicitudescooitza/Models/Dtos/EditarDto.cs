using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Dtos
{
    public class EditarDto
    {
        public long? idTblSolicitudesCatRubros { get; set; }
        public long? idTblSolicitudes { get; set; }
        public long? idCatProveedores { get; set; }
        public long? idCatRubros { get; set; }
        public decimal? monto { get; set; }
        public long? cantidad { get; set; }
        public string imagen { get; set; }
        public string razon {get; set; }
        public string fecha { get; set; }
        public HttpPostedFileBase imagenFile { get; set; }
    }
}