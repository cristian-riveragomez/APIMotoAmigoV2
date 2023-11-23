using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotoAmigoApiBD.Datos;
using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace MotoAmigoApiBD.Controllers
{
    [Route("/Usuarios")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private LogicaDeDatos _logicaDatos;
        public UsuariosController( AccesoADatos context)
        {
            var logicaDeDatos = new LogicaDeDatos(context);
            _logicaDatos = logicaDeDatos;
        }

        [HttpGet("ObtenerListaUsuarios")]
        public ActionResult ObtenerListaUsuarios()
        {
            try
            {
                var listaUsuarios =_logicaDatos.ObtenerListaUsuarios();
                return Ok(listaUsuarios);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        // GET: UsuariosController/Details/5
        [HttpGet("ObtenerUsuario")]
        public ActionResult ObtenerUsuario(string idUsuario)
        {
            try
            {

                var usuario = _logicaDatos.ObtenerUsuario(int.Parse(idUsuario));

                if (usuario == null)
                {
                    return BadRequest();
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }            
        }


        // POST: UsuariosController/Create
        [HttpPost("InsertarUsuario")]
        public ActionResult InsertarUsuario([FromForm] Usuario usuario)
        {
            try
            {
                var usuarioEncontrado = _logicaDatos.ValidarUsuarioDuplicado(usuario);
                if (usuarioEncontrado != null)
                {
                    return BadRequest("Error: usuario duplicado. El nombre de usuario o email ya estan registrado en el sistema");
                }

                var usuarioInsertado = _logicaDatos.InsertarUsuario(usuario);   
                return Ok(usuarioInsertado);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpPost("EditarUsuario")]
        public ActionResult EditarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _logicaDatos.ActualizarUsuario(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpDelete("BorrarUsuario")]
        public ActionResult BorrarUsuario(int idUsuario)
        {
            try
            {
                _logicaDatos.EliminarUsuario(idUsuario);
                return Ok();
            }
            catch (Exception ex) 
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }

        [HttpPost("LoginUsuario")]
        public ActionResult loginUsuario(UsuarioLogin usuario)
        {
            try
            {
                var usuarioLogin = _logicaDatos.Login(usuario);
                
                if (usuarioLogin == null)
                {
                    return BadRequest("Los datos ingresados son incorrectos"); 
                }

                return Ok(usuarioLogin);
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(error);
            }
        }
    }
}
