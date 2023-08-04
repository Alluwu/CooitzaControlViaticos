using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using solicitudescooitza.Models;
using solicitudescooitza.Models.Dtos;
using solicitudescooitza.Models.Ejemplo;

namespace solicitudescooitza.Controllers
{
    public class TblSolicitudesController : Controller
    {
        private developerEntities db = new developerEntities();
        public ActionResult Detalles(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);
            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }

            // Obtener todos los registros de TblSolicitudesCatRubros que tengan el mismo ID de solicitud
            var rubros = db.TblSolicitudesCatRubros.Where(c => c.idTblSolicitudes == id).ToList();

            // Asignar los registros a ViewBag
            ViewBag.TblSolicitudesCatRubros = rubros;

            return View(tblSolicitudes);
        }


        public ActionResult SolicitudesEstado2(string searchString)
        {
            try
            {
                // Obtener todas las solicitudes con estado 2 o 5
                var solicitudes = db.TblSolicitudes
                    .Where(s => s.idCatEstados == 2 || s.idCatEstados == 5);

                // Filtrar las solicitudes por nombre de usuario si se especifica en el parámetro searchString
                if (!string.IsNullOrEmpty(searchString))
                {
                    solicitudes = solicitudes
                        .Where(s => s.TblUsuarios.primerNombre.Contains(searchString) ||
                                    s.TblUsuarios.primerApellido.Contains(searchString));
                }

                // Pasar la lista de solicitudes filtradas a la vista "SolicitudesEstado2"
                return View(solicitudes.ToList());
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, puedes mostrar una vista de error o redirigir a la página de inicio.
                return RedirectToAction("Index", "TblSolicitudes");
            }
        }

        [HttpPost]
        public ActionResult ProcesarLiquidacion(long idTblSolicitudes, string observacionesLiquidacion, string accion)
        {
            try
            {
                var solicitud = db.TblSolicitudes.Find(idTblSolicitudes);

                if (solicitud != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    solicitud.idTblUsuarioModificado = userId;
                    solicitud.observacionesLiquidacion = observacionesLiquidacion;

                    if (accion == "Rechazar")
                    {
                        solicitud.idCatEstados = 7; // Cambiar el estado de la solicitud a 4 (rechazada)
                    }
                    else if (accion == "Aprobar")
                    {
                        solicitud.idCatEstados = 6; // Cambiar el estado de la solicitud a 3 (aprobada)
                    }

                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("SolicitudesEstado2", "TblSolicitudes");
                }
                else
                {
                    // Si la solicitud no fue encontrada, puedes mostrar una vista de error o redirigir a la página de inicio.
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, puedes mostrar una vista de error o redirigir a la página de inicio.
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ProcesarSolicitud(long idTblSolicitudes, string observacionesAutorizacion, string accion)
        {
            try
            {
                var solicitud = db.TblSolicitudes.Find(idTblSolicitudes);

                if (solicitud != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    solicitud.idTblUsuarioModificado = userId;
                    solicitud.observacionesAutorizacion = observacionesAutorizacion;

                    if (accion == "Rechazar")
                    {
                        solicitud.idCatEstados = 4; // Cambiar el estado de la solicitud a 4 (rechazada)
                    }
                    else if (accion == "Aprobar")
                    {
                        solicitud.idCatEstados = 3; // Cambiar el estado de la solicitud a 3 (aprobada)
                    }

                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("SolicitudesEstado2", "TblSolicitudes");
                }
                else
                {
                    // Si la solicitud no fue encontrada, puedes mostrar una vista de error o redirigir a la página de inicio.
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, puedes mostrar una vista de error o redirigir a la página de inicio.
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EnviarSolicitud(long idTblSolicitudes)
        {
            try
            {
                var solicitud = db.TblSolicitudes.Find(idTblSolicitudes);
                if (solicitud != null)
                {
                    solicitud.idCatEstados = 2; // Cambiar el estado de 1 a 2 (o el valor que corresponda)

                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();

                    // Obtener los usuarios con el rol de "Administracion"
                    var adminUsers = db.TblUsuarios
                        .Where(u => u.idCatGerencia == 1 && u.idCatGerencia == 1)
                        .ToList();

                    // Agregar la solicitud a la tabla de "Solicitudes pendientes" para cada usuario administrador encontrado
                    foreach (var adminUser in adminUsers)
                    {
                        var solicitudPendiente = new TblSolicitudes
                        {
                            idTblSolicitudes = idTblSolicitudes,
                            idTblUsuario = adminUser.idTblUsuarios,

                        };

                    }
                    // Obtener las solicitudes con estado 2
                    var solicitudesEstado2 = db.TblSolicitudes
                        .Where(s => s.idCatEstados == 2)
                        .ToList();

                    // Pasar la lista de solicitudes con estado 2 a la vista "SolicitudesEstado2"
                    return RedirectToAction("TodasSolicitudes");

                }
                else
                {
                    // Si la solicitud no fue encontrada, puedes mostrar una vista de error o redirigir a la página de inicio.
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, puedes mostrar una vista de error o redirigir a la página de inicio.
                return RedirectToAction("Index", "Home");
            }
        }


        public ActionResult Edit1(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);

            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCatProveedores = new SelectList(db.CatProveedores, "idCatProveedores", "descipcion");
            ViewBag.idCatRubros = new SelectList(db.CatRubros, "idCatRubros", "descripcion");
            ViewBag.idTblUsuario = new SelectList(db.TblUsuarios, "idTblUsuarios", "primerNombre");
            return View(tblSolicitudes);
        }
        [HttpPost]
        public ActionResult Edit1(TblSolicitudes tblSolicitudes)
        {
            if (ModelState.IsValid)
            {
                tblSolicitudes.idCatEstados = 1;
                db.Entry(tblSolicitudes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblSolicitudes);
        }

        // GET: TblSolicitudes
        public ActionResult Index()
        {

            ViewBag.idCatProveedoreses = new SelectList(db.CatProveedores, "idCatProveedoreses", "descipcion");
            ViewBag.idCatRubros = new SelectList(db.CatRubros, "idCatRubros", "descripcion");

            return View();
        }
        public ActionResult ResolverPdf(int? id)
        {
            TblSolicitudes solicitud = db.TblSolicitudes.Find(id);

            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View();

        }
        public ActionResult GenerarPDF(int? id)
        {
            // Obtener la solicitud de TblSolicitudes
            TblSolicitudes solicitud = db.TblSolicitudes.Find(id);

            if (solicitud == null)
            {
                return HttpNotFound();
            }

            // Obtener los registros de TblSolicitudesCatRubros asociados a la solicitud
            var solicitudesCatRubros = db.TblSolicitudesCatRubros.Where(r => r.idTblSolicitudes == id).ToList();

            // Crear un documento PDF utilizando iTextSharp
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50); // Ajustar los márgenes según tus necesidades
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Agregar un encabezado personalizado
                string imagePath = "C:\\Users\\DELL\\Downloads\\Avanze\\logo.jpeg";
                Image headerImage = Image.GetInstance(imagePath);
                headerImage.ScaleAbsolute(150f, 80f);
                headerImage.SetAbsolutePosition(document.PageSize.Width / 2 - headerImage.ScaledWidth / 2, document.PageSize.Height - 100);
                document.Add(headerImage);

                // Encabezado de la solicitud
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.DARK_GRAY);
                Paragraph solicitudTitulo = new Paragraph("Solicitud de Viáticos", headerFont);
                solicitudTitulo.Alignment = Element.ALIGN_CENTER; // Alineación en el centro
                solicitudTitulo.SpacingAfter = 20; // Espacio después del título
                document.Add(solicitudTitulo);

                // Detalles de la solicitud
                Font detailsFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                Paragraph detallesSolicitud = new Paragraph();
                detallesSolicitud.Alignment = Element.ALIGN_LEFT; // Alineación a la izquierda

                detallesSolicitud.Add($"Número de Solicitud: {solicitud.idTblSolicitudes}\n");
                detallesSolicitud.Add($"Creado por: {solicitud.TblUsuarios.primerNombre} {solicitud.TblUsuarios.segundoNombre} {solicitud.TblUsuarios.primerApellido}\n");
                detallesSolicitud.Add($"Lugar: {solicitud.lugarOrigenComision}, Fecha: {solicitud.fechaAlta}\n");
                detallesSolicitud.Add($"Motivo de la Comisión: {solicitud.motivoComision}\n");
                detallesSolicitud.Add($"Fecha de Inicio: {solicitud.fechaInicio}, Fecha de Fin: {solicitud.fechaFin}\n");

                // ... Agregar otros detalles de la solicitud ...

                detallesSolicitud.SpacingAfter = 30; // Espacio después de los detalles
                document.Add(detallesSolicitud);

                // Detalles de TblSolicitudesCatRubros
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Establecer el borde de la tabla
                table.DefaultCell.Border = Rectangle.BOX; // Borde completo alrededor de las celdas

                // Establecer colores de fondo para el encabezado de la tabla
                table.HeaderRows = 1; // Indicar que la primera fila es el encabezado
                table.DefaultCell.BackgroundColor = BaseColor.LIGHT_GRAY; // Fondo de color para el encabezado
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER; // Alineación en el centro
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE; // Alineación vertical en el centro

                // Agregar el encabezado de la tabla
                table.AddCell(new PdfPCell(new Phrase("Proveedor", detailsFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase("Rubro", detailsFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase("Monto", detailsFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase("Fecha", detailsFont)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase("Cantidad", detailsFont)) { Padding = 5 });

                // Restablecer el estilo para las celdas de los datos
                table.DefaultCell.BackgroundColor = BaseColor.WHITE; // Fondo blanco para las celdas de datos
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT; // Alineación a la izquierda

                // Agregar los detalles de cada solicitud en filas de la tabla
                foreach (var detalle in solicitudesCatRubros)
                {
                    table.AddCell(new PdfPCell(new Phrase(detalle.CatProveedores.descipcion, detailsFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(detalle.CatRubros.descripcion, detailsFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(detalle.monto.ToString(), detailsFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(detalle.fechaFactura.ToString(), detailsFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(detalle.cantidad.ToString(), detailsFont)) { Padding = 5 });
                }

                // Agregar la tabla al documento
                document.Add(table);
                // Agregar el total, tipo de transacción y rembolso/reintegro al final del documento
                Paragraph totalTransaccion = new Paragraph($"Total: {solicitud.totalQuetzales},  {solicitud.tipoTransaccion},  {solicitud.rembolsoReintegro}", detailsFont);
                totalTransaccion.Alignment = Element.ALIGN_RIGHT; // Alineación a la derecha
                document.Add(totalTransaccion);
                // Cerrar el documento
                document.Close();

                // Convertir el MemoryStream a un arreglo de bytes para enviar el PDF como respuesta
                byte[] pdfBytes = memoryStream.ToArray();

                // Pasar el arreglo de bytes como modelo a la vista
                return File(pdfBytes, "application/pdf", "SolicitudViaticos.pdf");
            }
        }

        public ActionResult Deletew(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);
            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }

            // Eliminar registros de la tabla relacionada (TblSolicitudesCatRubros) primero
            var idTblSolicitudCatRubros = db.TblSolicitudesCatRubros.Where(r => r.idTblSolicitudes == id);

            foreach (var item in idTblSolicitudCatRubros)
            {
                db.TblSolicitudesCatRubros.Remove(item);
            }

            // Luego, eliminar la solicitud (TblSolicitudes)
            db.TblSolicitudes.Remove(tblSolicitudes);

            db.SaveChanges();
            return RedirectToAction("Index"); // O cualquier otra acción que desees redireccionar después de la eliminación.
        }
        public ActionResult CrearNuevoRegistro()
        {
            return View();
        }
        public ActionResult TodasSolicitudes()
        {
            // Verificar si el usuario ha iniciado sesión y obtener su ID de usuario de la sesión.
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int loggedInUserId))
            {
                // Filtrar las solicitudes del usuario que ha iniciado sesión.
                var solicitudesDelUsuario = db.TblSolicitudes
                    .Where(s => s.idTblUsuario == loggedInUserId) // Reemplaza "UserId" con el nombre de la propiedad en tu tabla "TblSolicitudes" que almacena el ID de usuario.
                    .ToList()
                    .Select(s => new TblSolicitudesVM
                    {
                        lugarOrigenComision = s.lugarOrigenComision,
                        fechaAlta = String.Format("{0:dd/MM/yyyy}", s.fechaAlta),
                        idCatEstado = s.idCatEstados,
                        idTblSolicitudes = s.idTblSolicitudes,
                        fechaInicio = String.Format("{0:dd/MM/yyyy}", s.fechaInicio),
                        fechaFin = String.Format("{0:dd/MM/yyyy}", s.fechaFin),
                        montoTotal = s.montoTotal,
                        motivoComision = s.motivoComision
                    })
                    .ToList();

                return View(solicitudesDelUsuario);
            }

            return View("MensajeDeError"); // Reemplaza "MensajeDeError" con el nombre de la vista que muestra el mensaje de error.
        }
        [HttpPost]
        public ActionResult CrearNuevoRegistro(TblSolicitudes tblSolicitudes)
        {
            int userId = (int)(long)Session["UserId"];
            tblSolicitudes.fechaAlta = DateTime.Now;
            tblSolicitudes.idCatEstados = 1;
            tblSolicitudes.idTblUsuario = userId;
            db.TblSolicitudes.Add(tblSolicitudes);
            db.SaveChanges();
            return RedirectToAction("TodasSolicitudes", "TblSolicitudes");

        }


        [HttpPost]
        public ActionResult CompletarSolicitud(long idTblSolicitudes, HttpPostedFileBase imagenTransaccion)
        {
            try
            {
                var solicitud = db.TblSolicitudes.Find(idTblSolicitudes);
                if (solicitud == null)
                {
                    return Json(new { codigo = 0, descripcion = "La solicitud no existe." });
                }

                // Obtener el total de monto y montoTotal
                decimal? totalQ = db.TblSolicitudesCatRubros
                    .Where(r => r.idTblSolicitudes == idTblSolicitudes)
                    .Sum(r => r.monto) ?? 0;

                decimal? montoTotal = db.TblSolicitudes
                    .Where(s => s.idTblSolicitudes == idTblSolicitudes)
                    .Select(s => s.montoTotal)
                    .SingleOrDefault();

                decimal? diferencia = montoTotal - totalQ;

                string tipoTransaccion = diferencia < 0 ? "REEMBOLSO" : "REINTEGRO";

                // Actualizar el campo tipoTransaccion en la base de datos
                solicitud.tipoTransaccion = tipoTransaccion;

                if (tipoTransaccion == "REINTEGRO")
                {
                    Console.WriteLine("Entró al bloque del REINTEGRO"); // Agregamos registro de depuración

                    if (imagenTransaccion != null && imagenTransaccion.ContentLength > 0)
                    {
                        Console.WriteLine("Archivo recibido"); // Agregamos registro de depuración

                        // Define el directorio donde se almacenarán las imágenes de reintegro
                        string directorioImagenes = "~/ImagenesReintegro/";

                        // Asegurarse de que el directorio exista
                        string rutaCompleta = Server.MapPath(directorioImagenes);
                        if (!Directory.Exists(rutaCompleta))
                        {
                            Directory.CreateDirectory(rutaCompleta);
                        }

                        string nombreArchivo = Path.GetFileName(imagenTransaccion.FileName);
                        string rutaCompletaArchivo = Path.Combine(rutaCompleta, nombreArchivo);

                        // Subir la imagen al directorio
                        imagenTransaccion.SaveAs(rutaCompletaArchivo);

                        // Guardar la dirección de la imagen en el campo imagenTransaccion de la base de datos
                        solicitud.imagenTransaccion = Path.Combine(directorioImagenes, nombreArchivo);
                    }
                    else
                    {
                        Console.WriteLine("No se recibió archivo"); // Agregamos registro de depuración
                    }
                }

                solicitud.totalQuetzales = totalQ;
                solicitud.rembolsoReintegro = diferencia;
                solicitud.idCatEstados = 5; // Cambiar el estado de 1 a 5
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { codigo = 1, descripcion = "La solicitud se ha completado correctamente." });
            }
            catch (Exception ex)
            {
                // Registrar los detalles específicos del error para depuración
                Console.WriteLine("Error al completar la solicitud: " + ex.Message);

                return Json(new { codigo = 0, descripcion = "Error al completar la solicitud." });
            }
        }

        [HttpPost]
        public ActionResult EliminarSolicitud(long? idTblSolicitudesCatRubros)
        {
            try
            {
                var solicitudCatRubros = db.TblSolicitudesCatRubros.Find(idTblSolicitudesCatRubros);

                if (solicitudCatRubros == null)
                {
                    return Json(new { codigo = 0, descripcion = "La solicitud no existe." });
                }

                db.TblSolicitudesCatRubros.Remove(solicitudCatRubros);
                db.SaveChanges();

                return Json(new { codigo = 1, descripcion = "La solicitud se ha eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { codigo = 0, descripcion = "Error al eliminar la solicitud: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult ActualizarSolicitud(EditarDto editarDto)
        {
            try
            {
                var solicitudCatRubros = db.TblSolicitudesCatRubros.Find(editarDto.idTblSolicitudesCatRubros);
                if (solicitudCatRubros == null)
                {
                    return Json(new { codigo = 0, descripcion = "La solicitud no existe." });
                }

                solicitudCatRubros.idCatProveedores = editarDto.idCatProveedores;
                solicitudCatRubros.idCatRubros = editarDto.idCatRubros;
                solicitudCatRubros.monto = editarDto.monto;
                solicitudCatRubros.cantidad = editarDto.cantidad;
                solicitudCatRubros.detalle = editarDto.razon;
                string[] formatosPosibles = { "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd", "yyyy/M/dd", "dd/M/yyyy", "dd/MM/yyyy" };
                DateTime fechaObjeto;
                bool exito = DateTime.TryParseExact(editarDto.fecha, formatosPosibles, null, System.Globalization.DateTimeStyles.None, out fechaObjeto);
                solicitudCatRubros.fechaFactura = fechaObjeto;

                // Actualizar la imagen solo si se ha seleccionado una nueva
                if (editarDto.imagenFile != null && editarDto.imagenFile.ContentLength > 0)
                {
                    // Obtener el nombre del archivo y generar una ruta física
                    var nombreArchivo = Path.GetFileName(editarDto.imagenFile.FileName);
                    var rutaFisica = Path.Combine(Server.MapPath("~/Images/"), nombreArchivo);

                    // Guardar la imagen físicamente en la carpeta
                    editarDto.imagenFile.SaveAs(rutaFisica);

                    // Actualizar la dirección de la imagen en la base de datos
                    solicitudCatRubros.imagen = "/Images/" + nombreArchivo;
                }

                db.Entry(solicitudCatRubros).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { codigo = 1, descripcion = "La solicitud se ha actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { codigo = 0, descripcion = "Error al actualizar la solicitud: " + ex.Message });
            }
        }



        [HttpGet]
        public ActionResult GetSolicitudById(long? idTblSolicitudesCatRubros)
        {
            try
            {
                var solicitudCatRubros = db.TblSolicitudesCatRubros.Find(idTblSolicitudesCatRubros);

                if (solicitudCatRubros == null)
                {
                    return Json(new { codigo = 0, descripcion = "La solicitud no existe." }, JsonRequestBehavior.AllowGet);
                }

                EditarDto editarDto = new EditarDto();
                editarDto.idTblSolicitudesCatRubros = solicitudCatRubros.idTblSolicitudesCatRubros;
                editarDto.idCatProveedores = solicitudCatRubros.idCatProveedores;
                editarDto.idCatRubros = solicitudCatRubros.idCatRubros;
                editarDto.idTblSolicitudes = solicitudCatRubros.idTblSolicitudes;
                editarDto.monto = solicitudCatRubros.monto;
                editarDto.cantidad = solicitudCatRubros.cantidad;
                editarDto.razon = solicitudCatRubros.detalle;

                if (solicitudCatRubros.fechaFactura != null)
                {
                    editarDto.fecha = solicitudCatRubros.fechaFactura.Value.ToString("yyyy-MM-dd"); // Formatea la fecha al formato requerido
                }
                else
                {
                    editarDto.fecha = null; // Asigna null si la fecha es nula en la base de datos
                }

                editarDto.imagen = solicitudCatRubros.imagen; // La dirección de la imagen

                return Json(new { codigo = 1, descripcion = "Éxito", solicitud = editarDto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { codigo = 0, descripcion = "Error al obtener los datos de la solicitud: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetSolicicitudes(long? idTblSolicitudes)
        {
            var respuesta = new
            {
                codigo = 0,
                descripcion = string.Empty,
                listado = new List<TblSolicitudesVM>()
            };
            List<TblSolicitudesVM> solicitudes = new List<TblSolicitudesVM>();
            try
            {
                solicitudes = (from d in db.TblSolicitudesCatRubros
                               where d.idCatEstados == 1 && d.idTblSolicitudes == idTblSolicitudes
                               orderby d.fechaAlta descending
                               select new
                               {
                                   Consulta = d
                               })
              .ToList()
              .Select(result => new TblSolicitudesVM
              {
                  idTblSolicitudesCatRubros = result.Consulta.idTblSolicitudesCatRubros == null ? 0 : result.Consulta.idTblSolicitudesCatRubros,
                  idTblSolicitudes = (long)((long)result.Consulta.idTblSolicitudes == null ? 0 : result.Consulta.idTblSolicitudes),
                  Proveedor = result.Consulta.CatProveedores == null ? string.Empty : result.Consulta.CatProveedores.descipcion,
                  Rubro = result.Consulta.CatRubros == null ? string.Empty : result.Consulta.CatRubros.descripcion,
                  Detalle = result.Consulta.detalle,
                  Monto = result.Consulta.monto == 0 ? "Q0.00" : String.Format(CultureInfo.InvariantCulture, "Q{0:0,0.00}", result.Consulta.monto),
                  Fecha = String.Format("{0:dd/MM/yyyy}", result.Consulta.fechaAlta),
                  Cantidad = result.Consulta.cantidad,
                  Imagen = result.Consulta.imagen == null ? string.Empty : result.Consulta.imagen
              }).ToList();
            }
            catch (Exception ex)
            {
                respuesta = new
                {
                    codigo = 0,
                    descripcion = ex.Message,
                    listado = new List<TblSolicitudesVM>()
                };
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }


            respuesta = new
            {
                codigo = 1,
                descripcion = "CORRECTO",
                listado = solicitudes
            };
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        // GET: TblSolicitudes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);
            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }
            return View(tblSolicitudes);
        }

        public ActionResult AddSolicitud()
        {

            TblSolicitudes tblSolicitudes = new TblSolicitudes();
            tblSolicitudes.fechaAlta = DateTime.Now;
            tblSolicitudes.idCatEstados = 1;
            db.TblSolicitudes.Add(tblSolicitudes);
            db.SaveChanges();

            return RedirectToAction("Edit/" + tblSolicitudes.idTblSolicitudes, "TblSolicitudes");


        }
        // GET: TblSolicitudes/Create
        public ActionResult Create()
        {
            ViewBag.idCatProveedoreses = new SelectList(db.CatProveedores, "idCatProveedores", "descipcion");
            ViewBag.idCatRubros = new SelectList(db.CatRubros, "idCatRubros", "descripcion");
            ViewBag.idTblUsuario = new SelectList(db.TblUsuarios, "idTblUsuarios", "primerNombre");
            return View();
        }

        // POST: TblSolicitudes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TblSolicitudes tblSolicitudes)
        {
            if (ModelState.IsValid)
            {
                db.TblSolicitudes.Add(tblSolicitudes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idCatProveedoreses = new SelectList(db.CatProveedores, "idCatProveedores", "Descripcion_", tblSolicitudes.idCatProveedores);
            ViewBag.idCatRubros = new SelectList(db.CatRubros, "idCatRubros", "descripcion", tblSolicitudes.idCatRubros);
            ViewBag.idTblUsuario = new SelectList(db.TblUsuarios, "idTblUsuarios", "primerNombre", tblSolicitudes.idTblUsuario);
            return View(tblSolicitudes);
        }

        // GET: TblSolicitudes/Edit/5 VISTA
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);

            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCatProveedores = new SelectList(db.CatProveedores, "idCatProveedores", "descipcion");
            ViewBag.idCatRubros = new SelectList(db.CatRubros, "idCatRubros", "descripcion");
            ViewBag.idTblUsuario = new SelectList(db.TblUsuarios, "idTblUsuarios", "primerNombre");
            return View(tblSolicitudes);
        }


        public ActionResult ObtenerTotalQuetzales(long idTblSolicitudes, HttpPostedFileBase imagen)
        {
            // Obtener el total de monto y montoTotal
            decimal? totalMonto = db.TblSolicitudesCatRubros
            .Where(r => r.idTblSolicitudes == idTblSolicitudes)
            .Sum(r => r.monto) ?? 0; // Utilizará 0 si el resultado de la suma es nulo


            decimal? montoTotal = (decimal)db.TblSolicitudes
                .Where(s => s.idTblSolicitudes == idTblSolicitudes)
                .Select(s => s.montoTotal)
                .SingleOrDefault();

            decimal? diferencia = montoTotal - totalMonto;

            string tipoTransaccion = diferencia < 0 ? "Rembolso" : "Reintegro";

            // Actualizar el campo tipoTransaccion en la base de datos (opcional, si no deseas guardar los cambios en la base de datos)
            var solicitud = db.TblSolicitudes.SingleOrDefault(s => s.idTblSolicitudes == idTblSolicitudes);
            if (solicitud != null)
            {
                solicitud.tipoTransaccion = tipoTransaccion;

                if (tipoTransaccion == "Reintegro" && imagen != null && imagen.ContentLength > 0)
                {
                    // Define el directorio donde se almacenarán las imágenes de reintegro
                    string directorioImagenes = "~/ImagenesReintegro/";
                    string nombreArchivo = Path.GetFileName(imagen.FileName);
                    string rutaCompleta = Path.Combine(Server.MapPath(directorioImagenes), nombreArchivo);

                    // Sube la imagen al directorio
                    imagen.SaveAs(rutaCompleta);

                    // Guarda la dirección de la imagen en el campo imagenTransaccion de la base de datos
                    solicitud.imagenTransaccion = Path.Combine(directorioImagenes, nombreArchivo);
                }


            }

            return Json(new
            {
                totalQuetzales = totalMonto,
                rembolsoReintegro = diferencia,
                tipoTransaccion
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: TblSolicitudes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(TblSolicitudesDto tblSolicitudesDto, HttpPostedFileBase imagenFile)
        {

            var result = new
            {
                codigo = 0,
                descripcion = string.Empty,
            };

            if (!ModelState.IsValid)
            {
                var entradasErroneas = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .Select(x => x.Key.Split('.').Last())
                    .ToList();

                result = new
                {
                    codigo = 0,
                    descripcion = $"Es necesario que brindes todos los datos requeridos: {string.Join(",", entradasErroneas)}"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                TblSolicitudesCatRubros tblSolicitudesCatRubros = new TblSolicitudesCatRubros();
                tblSolicitudesCatRubros.idTblSolicitudes = tblSolicitudesDto.idTblSolicitudes;
                tblSolicitudesCatRubros.idCatProveedores = tblSolicitudesDto.idCatProveedores;
                tblSolicitudesCatRubros.idCatRubros = tblSolicitudesDto.idCatRubros;
                tblSolicitudesCatRubros.detalle = tblSolicitudesDto.razon;
                tblSolicitudesCatRubros.monto = tblSolicitudesDto.monto == null ? 0 : Convert.ToDecimal(tblSolicitudesDto.monto);

                string[] formatosPosibles = { "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd", "yyyy/M/dd", "dd/M/yyyy", "dd/MM/yyyy" };
                DateTime fechaObjeto;
                bool exito = DateTime.TryParseExact(tblSolicitudesDto.fecha, formatosPosibles, null, System.Globalization.DateTimeStyles.None, out fechaObjeto);

                if (!exito)
                {
                    result = new
                    {
                        codigo = 0,
                        descripcion = "IMPOSIBLE RESOLVER LA FECHA",
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                tblSolicitudesCatRubros.fechaFactura = fechaObjeto;
                tblSolicitudesCatRubros.cantidad = tblSolicitudesDto.cantidad;
                tblSolicitudesCatRubros.fechaAlta = DateTime.Now;
                tblSolicitudesCatRubros.idTblUsuarios = 1;
                tblSolicitudesCatRubros.idCatEstados = 1;

                if (imagenFile != null && imagenFile.ContentLength > 0)
                {
                    string nombreArchivo = Path.GetFileName(imagenFile.FileName);
                    string rutaGuardarImagen = Path.Combine(Server.MapPath("~/Images"), nombreArchivo);

                    try
                    {
                        imagenFile.SaveAs(rutaGuardarImagen);
                        tblSolicitudesCatRubros.imagen = "/Images/" + nombreArchivo;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al guardar la imagen: " + ex.Message);
                    }
                }
                else
                {
                    tblSolicitudesCatRubros.imagen = string.IsNullOrEmpty(tblSolicitudesDto.imagen) ? string.Empty : tblSolicitudesDto.imagen;
                }

                db.TblSolicitudesCatRubros.Add(tblSolicitudesCatRubros);
                db.SaveChanges();


                TblSolicitudes solicitud = db.TblSolicitudes.Find(tblSolicitudesDto.idTblSolicitudes);
                if (solicitud != null)
                {

                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();
                }
                result = new
                {
                    codigo = 1,
                    descripcion = "REGISTRO GUARDADO CORRECTAMENTE",

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                result = new
                {
                    codigo = 0,
                    descripcion = "ERROR AL GUARDAR LOS DATOS" + ex.Message,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: TblSolicitudes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);
            if (tblSolicitudes == null)
            {
                return HttpNotFound();
            }
            return View(tblSolicitudes);
        }

        // POST: TblSolicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TblSolicitudes tblSolicitudes = db.TblSolicitudes.Find(id);
            db.TblSolicitudes.Remove(tblSolicitudes);
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
