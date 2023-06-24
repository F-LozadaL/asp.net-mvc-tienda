using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;

namespace CapaPresentacionTienda.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();

        }
        public ActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje=String.Empty;

            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "" : objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;

            if(objeto.Clave== objeto.ConfirmarClave)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }
            else
            {
                resultado = new CN_Clientes().Registrar(objeto, out mensaje);

                if (resultado > 0)
                {
                    ViewBag.Error = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = mensaje;
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Cliente oCliente = null;

            oCliente = new CN_Clientes().Listar().Where(u=>u.Correo==correo && u.Clave==CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();
            if (oCliente==null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
            else
            {
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarClave","Acceso");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(oCliente.Correo, false);
                    Session["Cliente"] = oCliente;
                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Tienda");
                }
            }
        }


        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente oCliente = new Cliente();

            oCliente = new CN_Clientes().Listar().Where(u => u.Correo == correo).FirstOrDefault();

            if (oCliente == null)
            {
                ViewBag.Error = "No se encontro un cliente relacionado a ese correo";
                return View();
            }
            string mensaje = String.Empty;
            bool respuesta = new CN_Clientes().ReestablecerClave(oCliente.IdCliente, correo, out mensaje);
            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }


        [HttpPost]
        public ActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmarclave)
        {
            Cliente oCliente = new Cliente();

            oCliente = new CN_Clientes().Listar().Where(u => u.IdCliente == int.Parse(idcliente)).FirstOrDefault();

            if (oCliente.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {

                TempData["IdCliente"] = oCliente.IdCliente;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual no es correcta";

                return View();
            }
            else if (nuevaclave != confirmarclave)
            {
                TempData["IdCliente"] = oCliente.IdCliente;
                ViewData["vclave"] = claveactual;
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";
            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = String.Empty;

            bool respuesta = new CN_Clientes().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);
            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = oCliente.IdCliente;
                ViewBag.Error = mensaje;
                return View();
            }

        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");

        }
    }
}