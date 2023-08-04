using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Dtos
{
    public class Operaciones
    {
        public decimal? totalQuetzales { get; set; }
        public decimal? rembolsoReintegro { get; set; }
        public string tipoTransaccion { get; set; }
        public string imagenTransaccion { get; set; }

    }
}