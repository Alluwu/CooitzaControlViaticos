using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace solicitudescooitza.Models.Dtos
{
    public class LoginVM
    {
        public string Correo { get; set; }
        public string nombre { get; set; }
        public string contraseña { get; set;}
        public string idCatGerencia { get; set; }
    }
}