using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Dtos
{
    public class TblSolicitudesDto
    {
        
        public long idTblSolicitudes { get; set; }
        [Required]
        public Nullable<long> idCatProveedores { get; set; }
        [Required]
        public Nullable<long> idCatRubros { get; set; }
        [Required]
        public string razon { get; set; }
        [Required]
        public string monto { get; set; }
        [Required]
        public string fecha { get; set; }
        [Required]
        public Nullable<long> cantidad { get; set; }

        public string imagen { get; set; }
        public HttpPostedFileBase imagenFile { get; set; }
    }
}