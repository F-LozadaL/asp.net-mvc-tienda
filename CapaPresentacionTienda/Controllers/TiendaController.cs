using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaNegocio;
using System.IO;
using System.Web.Services.Description;


namespace CapaPresentacionTienda.Controllers
{
    public class TiendaController : Controller
    {
        // GET: Tienda
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetalleProducto(int idproducto=0)
        {
            Producto oProducto = new Producto();
            bool conversion;

            oProducto= new CN_Productos().Listar().Where(p=> p.IdProducto==idproducto).FirstOrDefault();

            if (oProducto != null)
            {
                oProducto.Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);
                oProducto.Extension = Path.GetExtension(oProducto.NombreImagen); 
            }



            return View(oProducto);
        }



        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> lista = new List<Categoria>();

            lista = new CN_Categorias().Listar();

            return Json(new{ data=lista },JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ListarMarcaPorCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();

            lista = new CN_Marcas().ListarMarcaPorCategoria(idcategoria);

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ListarProductos(int idcategoria, int idmarca)
        {
            List<Producto> lista = new List<Producto>();
            bool conversion;


            lista = new CN_Productos().Listar().Select(p=>new Producto()
            {
                IdProducto=p.IdProducto,
                Nombre=p.Nombre,
                Descripcion=p.Descripcion,
                oMarca=p.oMarca,
                oCategoria = p.oCategoria,
                Precio=p.Precio,
                Stock=p.Stock,
                RutaImagen=p.RutaImagen,
                Base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.RutaImagen,p.NombreImagen), out conversion ),
                Extension=Path.GetExtension(p.NombreImagen),
                Activo=p.Activo
            }).Where(p=>
                p.oCategoria.IdCategoria==(idcategoria==0? p.oCategoria.IdCategoria : idcategoria) &&
                p.oMarca.IdMarca == (idmarca == 0 ? p.oMarca.IdMarca : idmarca) &&
                p.Stock>0 &&
                p.Activo).ToList();

            var jsonresult = Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;

            return jsonresult;
        }


        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto)
        {

            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            bool existe = new CN_Carrito().ExisteCarrito(idcliente, idproducto);
            bool respuesta = false;
            string mensaje = String.Empty;
            if (existe) {
                mensaje = "El producto ya existe en carrito";
            }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            }
            return Json(new { respuesta, mensaje },JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CantidadEnCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            int cantidad = new CN_Carrito().CantidadEnCarrito(idcliente);
            return Json(new { cantidad }, JsonRequestBehavior.AllowGet);
        }
    }
}