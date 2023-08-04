using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Dtos
{
    public class SolicitudesPorId
    {
        public long? idTblSolicitudes { get; set; }
        public List<TblSolicitudesVM> Solicitudes { get; set; }
    }
}