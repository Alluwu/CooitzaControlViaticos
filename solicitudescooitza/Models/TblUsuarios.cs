//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace solicitudescooitza.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblUsuarios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblUsuarios()
        {
            this.CatRubros = new HashSet<CatRubros>();
            this.TblSolicitudes = new HashSet<TblSolicitudes>();
            this.TblSolicitudesPendientes = new HashSet<TblSolicitudesPendientes>();
        }
    
        public long idTblUsuarios { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string tercerApellido { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string codigoEmpleado { get; set; }
        public string numeroCuenta { get; set; }
        public long idRol { get; set; }
        public string categoria { get; set; }
        public Nullable<long> idCatGerencia { get; set; }
    
        public virtual CatGenerencia CatGenerencia { get; set; }
        public virtual CatRoles CatRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatRubros> CatRubros { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSolicitudes> TblSolicitudes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSolicitudesPendientes> TblSolicitudesPendientes { get; set; }
    }
}