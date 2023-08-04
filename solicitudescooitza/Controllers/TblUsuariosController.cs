using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using solicitudescooitza.Models;
using solicitudescooitza.Models.Dtos;

namespace solicitudescooitza.Controllers
{
    public class TblUsuariosController : Controller
    {
        private developerEntities db = new developerEntities();


        // GET: TblUsuarios
        public ActionResult Index()
        {
            var tblUsuarios = db.TblUsuarios.Include(t => t.CatGenerencia).Include(t => t.CatRoles);
            return View(tblUsuarios.ToList());
        }
        public ActionResult Login() { 
        return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM loginVm)
        {
            if (ModelState.IsValid)
            {
                // Encriptar la contraseña ingresada por el usuario para compararla con la versión encriptada en la base de datos.
                string hashedPassword = HashPassword(loginVm.contraseña);

                // Buscar al usuario en la base de datos por su correo electrónico (nombre de usuario) y contraseña encriptada.
                var user = db.TblUsuarios.SingleOrDefault(u => u.Correo == loginVm.nombre && u.Contraseña == hashedPassword);
                if (user != null)
                {
                    // Si el usuario existe, establecer la información de sesión para el usuario.
                    Session["UserId"] = user.idTblUsuarios;
                    Session["UserName"] = user.Correo;
                    Session["Gerencia"] = user.idCatGerencia;

                    // Redirigir al usuario a la página de inicio (Home).
                    return RedirectToAction("Index", "TblSolicitudes");
                }

                // Si las credenciales son inválidas, agregar un mensaje de error al modelo.
                ModelState.AddModelError("", "Usuario o Contraseña Inválida");
            }

            // Si el modelo no es válido o el inicio de sesión falla, regresar a la vista de inicio de sesión con los errores.
            return View(loginVm);
        }


        // GET: TblUsuarios/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblUsuarios tblUsuarios = db.TblUsuarios.Find(id);
            if (tblUsuarios == null)
            {
                return HttpNotFound();
            }
            return View(tblUsuarios);
        }

        // GET: TblUsuarios/Create
        public ActionResult Create()
        {
            ViewBag.idCatGerencia = new SelectList(db.CatGenerencia, "idCatGerencia", "descripcion");
            ViewBag.idRol = new SelectList(db.CatRoles, "idCatRoles", "descripcion");
            return View();
        }

        // POST: TblUsuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idTblUsuarios,primerNombre,segundoNombre,primerApellido,segundoApellido,tercerApellido,Correo,Contraseña,idRol,idCatGerencia")] TblUsuarios tblUsuarios)
        {
            if (ModelState.IsValid)
            {
                // Encriptar la contraseña antes de guardarla en la base de datos
                tblUsuarios.Contraseña = HashPassword(tblUsuarios.Contraseña);

                db.TblUsuarios.Add(tblUsuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idCatGerencia = new SelectList(db.CatGenerencia, "idCatGerencia", "descripcion", tblUsuarios.idCatGerencia);
            ViewBag.idRol = new SelectList(db.CatRoles, "idCatRoles", "descripcion", tblUsuarios.idRol);
            return View(tblUsuarios);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        // GET: TblUsuarios/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblUsuarios tblUsuarios = db.TblUsuarios.Find(id);
            if (tblUsuarios == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCatGerencia = new SelectList(db.CatGenerencia, "idCatGerencia", "descripcion", tblUsuarios.idCatGerencia);
            ViewBag.idRol = new SelectList(db.CatRoles, "idCatRoles", "descripcion", tblUsuarios.idRol);
            return View(tblUsuarios);
        }

        // POST: TblUsuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idTblUsuarios,primerNombre,segundoNombre,primerApellido,segundoApellido,tercerApellido,Correo,Contraseña,idRol,idCatGerencia")] TblUsuarios tblUsuarios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblUsuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCatGerencia = new SelectList(db.CatGenerencia, "idCatGerencia", "descripcion", tblUsuarios.idCatGerencia);
            ViewBag.idRol = new SelectList(db.CatRoles, "idCatRoles", "descripcion", tblUsuarios.idRol);
            return View(tblUsuarios);
        }

        // GET: TblUsuarios/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblUsuarios tblUsuarios = db.TblUsuarios.Find(id);
            if (tblUsuarios == null)
            {
                return HttpNotFound();
            }
            return View(tblUsuarios);
        }

        // POST: TblUsuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TblUsuarios tblUsuarios = db.TblUsuarios.Find(id);
            db.TblUsuarios.Remove(tblUsuarios);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
