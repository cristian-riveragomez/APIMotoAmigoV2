using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MotoAmigoApiBD.Datos;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("/Mail")]
    [ApiController]
    public class MailController : Controller
    {
        private LogicaDeDatos _logicaDatos;

        public MailController(AccesoADatos context)
        {
            var logicaDeDatos = new LogicaDeDatos(context);
            _logicaDatos = logicaDeDatos;
        }

        [HttpPost("InsertarMail")]
        public ActionResult InsertarMail(Mail mail)
        {
            try
            {
                _logicaDatos.InsertarMail(mail);
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
