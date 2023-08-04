using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using solicitudescooitza.Models;

namespace solicitudescooitza.Controllers
{
    public class CatProveedoresController : Controller
    {
        private developerEntities db = new developerEntities();

        // GET: CatProveedores
        public ActionResult Index()
        {
            return View(db.CatProveedores.ToList());
        }

        // GET: CatProveedores/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatProveedores catProveedores = db.CatProveedores.Find(id);
            if (catProveedores == null)
            {
                return HttpNotFound();
            }
            return View(catProveedores);
        }

        // GET: CatProveedores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatProveedores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCatproveedores,descipcion")] CatProveedores catProveedores)
        {
            if (ModelState.IsValid)
            {
                db.CatProveedores.Add(catProveedores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catProveedores);
        }

        // GET: CatProveedores/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatProveedores catProveedores = db.CatProveedores.Find(id);
            if (catProveedores == null)
            {
                return HttpNotFound();
            }
            return View(catProveedores);
        }

        // POST: CatProveedores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCatproveedores,descipcion")] CatProveedores catProveedores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catProveedores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catProveedores);
        }

        // GET: CatProveedores/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatProveedores catProveedores = db.CatProveedores.Find(id);
            if (catProveedores == null)
            {
                return HttpNotFound();
            }
            return View(catProveedores);
        }

        // POST: CatProveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CatProveedores catProveedores = db.CatProveedores.Find(id);
            db.CatProveedores.Remove(catProveedores);
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
