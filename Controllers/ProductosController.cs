using Microsoft.AspNetCore.Mvc;
using MotoAmigoApiBD.Datos;
using WebApi.Models;

namespace MotoAmigoApiBD.Controllers
{

    [Microsoft.AspNetCore.Mvc.Route("/Productos")]
    [ApiController]
    public class ProductosController : Controller
    {
        private LogicaDeDatos _logicaDatos;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductosController(AccesoADatos context, IWebHostEnvironment hostEnvironment)
        {
            var logicaDeDatos = new LogicaDeDatos(context);
            _logicaDatos = logicaDeDatos;

            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("ObtenerListaProductos")]
        public ActionResult ObtenerListaProductos()
        {
            try
            {
                var listaProductos = _logicaDatos.ObtenerListaProductos();              
                return Ok(listaProductos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("ObtenerProductoPorId")]
        public ActionResult ObtenerProductoPorId(int idProducto)
        {
            try
            {
                var Producto = _logicaDatos.ObtenerProducto(idProducto);

                if (Producto == null)
                {
                    return BadRequest();
                }

                return Ok(Producto);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("ObtenerImagenDelProductoPorId")]
        public ActionResult ObtenerImagenDelProductoPorId(int idProducto, string tipoImagen)
        {
            try
            {
                var Producto = _logicaDatos.ObtenerProducto(idProducto);

                if (Producto == null)
                {
                    return BadRequest();
                }

                if (tipoImagen == "RepuestoPendiente")
                {
                    return File(Producto.imagenRepuestoContenido, "image/jpg");
                }
  
                return File(Producto.imagenContenido, "image/jpg");
    
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("ObtenerProductosPendientes")]
        public ActionResult ObtenerProductosPendientesDeAprobacion()
        {
            try
            {
                var listaDeProductos = _logicaDatos.ObtenerProductosPendientesDeAprobacion();

                return Ok(listaDeProductos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("UpdateEstadoProducto")]
        public ActionResult ActualizarEstadoDeUnProducto(int idProducto, string estado)
        {
            try
            {
                _logicaDatos.ActualizacionDeEstado(idProducto, estado);

                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }


        [HttpGet("ObtenerProductosPorIdUsuario")]
        public ActionResult ObtenerProductoPorIdUsuario(int idUsuario)
        {
            try
            {
                var listaDeProductos = _logicaDatos.BuscarProductosPorIdUsuario(idUsuario);               

                return Ok(listaDeProductos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("ObtenerProductosPorCantDenuncias")]
        public ActionResult ObtenerProductosPorCantDenuncias()
        {
            try
            {
                var listaDeProductos = _logicaDatos.ObtenerProductosPorDenuncias();

                return Ok(listaDeProductos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("ObtenerProductosPorUsuarioDestacado")]
        public ActionResult ObtenerProductosPorUsuarioDestacado(int idUsuario)
        {
            try
            {
                var listaDeProductos = _logicaDatos.ObtenerProductosPorDestacados(idUsuario);

                return Ok(listaDeProductos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("DenunciarProducto")]
        public ActionResult DenunciarProducto(int idProducto, int idUsuario)
        {
            try
            {
                _logicaDatos.DenunciarProducto(idProducto, idUsuario);

                return Ok();
            } 
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("ValidarProductoDenunciadoPorUsuario")]
        public ActionResult ValidarProductoDenunciadoPorUsuario(int idUsuario, int idProducto)
        {
            try
            {                
                return Ok(_logicaDatos.ValidarProductoSiEstaDenunciado(idUsuario, idProducto));
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("BuscarProducto")]
        public ActionResult BuscarProducto(string filtro)
        {
            try
            {
                var Productos =  _logicaDatos.BuscarProducto(filtro);

                return Ok(Productos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("ValidarProductoDestacado")]
        public ActionResult ValidarProductoDestacado(int idUsuarioDestacando, int idProducto)
        {
            try
            {
                var Productos = _logicaDatos.ValidarSiProductoEsDestacado(idUsuarioDestacando, idProducto);


                return Ok(Productos);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }

        }

        [HttpGet("AgregarProductoDestacado")]
        public ActionResult AgregarProductoDestacado(int idUsuarioDestacando, int idProducto)
        {
            try
            {
                _logicaDatos.InsertarProductoDestacado(idUsuarioDestacando, idProducto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpDelete("BorrarProductoDestacado")]
        public ActionResult BorrarProductoDestacado(int idUsuarioDestacando, int idProducto)
        {
            try
            {
                _logicaDatos.EliminarProductoDestacado(idUsuarioDestacando, idProducto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpPost("InsertarProducto")]
        public ActionResult InsertarProducto([FromForm] ProductoFormulario producto)
        {
            try
            {
                _logicaDatos.InsertarProducto(producto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpPut("EditarProducto")]
        public ActionResult EditarProducto([FromForm] ProductoFormulario Producto)
        {
            try
            {
                _logicaDatos.ActualizarProducto(Producto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpDelete("BorrarProducto")]
        public ActionResult BorrarProducto(int idProducto)
        {
            try
            {
                _logicaDatos.EliminarProducto(idProducto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("ObtenerProductosPorPrecio")]
        public ActionResult ObtenerProductosPorPrecio(string? precioMin, string? precioMaximo)
        {
            try
             {
                if (!int.TryParse(precioMin, out int result) && precioMin != null && precioMin != "undefined")
                {
                    return BadRequest("El precio minimo ingresado no es un numero");
                }

                if (!int.TryParse(precioMaximo, out int result2) && precioMaximo != null && precioMaximo != "undefined")
                {
                    return BadRequest("El precio maximo ingresado no es un numero");
                }
               
                return Ok(_logicaDatos.ObtenerProductosPorPrecio(precioMin, precioMaximo));
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("ObtenerProductosPorFiltroMobile")]
        public ActionResult ObtenerProductosPorFiltroMobile(string? precioMinimo, string? precioMaximo, bool? accesorioCheck, bool? repuestoCheck)
        {
            try
            {
                return Ok(_logicaDatos.ObtenerProductosPorFiltro(precioMinimo, precioMaximo, accesorioCheck, repuestoCheck));
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpGet("AprobarProductoDenunciado")]
        public ActionResult AprobarProductoDenunciado(int idProducto)
        {
            try
            {
                _logicaDatos.ActualizarProductoDenunciado(idProducto);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }
    }
}
