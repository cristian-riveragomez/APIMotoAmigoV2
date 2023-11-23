using ApiClima.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace ApiClima.Controllers
{
    [ApiController]
    [Route("apiMotoAmigo")]
    public class ClimaController : ControllerBase
    { 
        
        [HttpPost("ObtencionDatosClimaActual")]
        public async Task<IActionResult> ObtencionDatosClimaActual(DatosLocalizacion datosLocalizacion)
        {
            try
            {
                if (datosLocalizacion == null)
                {
                    return BadRequest("Request nulo");
                }

                if (string.IsNullOrEmpty(datosLocalizacion.latitud.ToString()) || string.IsNullOrEmpty(datosLocalizacion.longitud.ToString()))
                {
                    return BadRequest("Parametros request vacio.");
                }

                var getDatosClima = new GetDatosClima();
                var datosInterfaz = getDatosClima.ObtenerDatosActuales(datosLocalizacion.longitud, datosLocalizacion.latitud);

                if (datosInterfaz == null)
                {
                    return BadRequest("Error al cargar los datos actuales");
                }

                return Ok(datosInterfaz);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("ObtencionDatosClimaSemanal")]
        public async Task<IActionResult> ObtencionDatosClimaSemanal(DatosLocalizacion datosLocalizacion)
        {
            try
            {
                if (datosLocalizacion == null)
                {
                    return BadRequest("Request nulo");
                }

                if (string.IsNullOrEmpty(datosLocalizacion.latitud.ToString()) || string.IsNullOrEmpty(datosLocalizacion.longitud.ToString()))
                {
                    return BadRequest("Parametros request vacio.");
                }

                var getDatosClima = new GetDatosClima();
                var datosInterfaz = getDatosClima.ObtenerDatosSemanales(datosLocalizacion.longitud, datosLocalizacion.latitud);

                if (datosInterfaz == null)
                {
                    return BadRequest("Error al cargar los datos semanales");
                }

                return Ok(datosInterfaz);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    return BadRequest(ex.Message);
                }

                return BadRequest(ex.InnerException);
            }
        }

    }

}