using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;

public class GetDatosClima
{
    private string keyApiClima = "b34e9feea7d4533f021c1d3f1be80ddf";
    public DatosClimaActualInterfaz ObtenerDatosActuales(string longitud, string latitud)
    {
        try
        {
            string urlClima = "https://api.openweathermap.org/data/2.5/weather?lat=" + latitud + "&lon=" + longitud + "&lang=sp&appid=" + this.keyApiClima + "&units=metric";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlClima);
            request.Method = "POST";
            request.ContentType = "application/json";
            var responseApi = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                StreamReader responseStream = new StreamReader(response.GetResponseStream());
                responseApi = responseStream.ReadToEnd();
            }

            var datosJson = JsonConvert.DeserializeObject<DatosClimaActualJson>(responseApi);

            return MapeoDatosClimaActual(datosJson);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<ClimaPorDia> ObtenerDatosSemanales(string longitud, string latitud)
    {
        try
        {
            string urlClima = "https://api.openweathermap.org/data/2.5/forecast?lat=" + latitud + "&lon=" + longitud + "&lang=sp&appid=" + this.keyApiClima + "&units=metric";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlClima);
            request.Method = "POST";
            request.ContentType = "application/json";
            var responseApi = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                StreamReader responseStream = new StreamReader(response.GetResponseStream());
                responseApi = responseStream.ReadToEnd();
            }

            var datosJson = JsonConvert.DeserializeObject<DatosClimaSemanalJson>(responseApi);

            return MapeoDatosClimaSemanal(datosJson);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DatosClimaActualInterfaz MapeoDatosClimaActual(DatosClimaActualJson datosClimaJson)
    {
        DatosClimaActualInterfaz datosMapeados = new DatosClimaActualInterfaz();

        datosMapeados.temperatura = datosClimaJson.main.temp.ToString("0").Replace(',', '.');
        datosMapeados.sensacionTermica = datosClimaJson.main.feels_like.ToString("0").Replace(',', '.'); ;
        datosMapeados.descripcionClima = datosClimaJson.weather[0].description;
        datosMapeados.humedad = datosClimaJson.main.humidity;
        datosMapeados.icono = datosClimaJson.weather[0].icon;

        return datosMapeados;
    }

    public List<ClimaPorDia> MapeoDatosClimaSemanal(DatosClimaSemanalJson datosClimaJson)
    {
        DatosClimaSemanalInterfaz datosMapeados = new DatosClimaSemanalInterfaz();
        var listaSemanal = new List<ListaDatosPorDiA>();

        if (datosClimaJson.cnt > 0)
        {
            var listaPorDia = new ListaDatosPorDiA();
            bool fechaEsIgual = false;

            foreach (var datoClimaJSON in datosClimaJson.list) // recorrer lista de datos json
            {
                fechaEsIgual = false;

                if (listaSemanal.Count > 0)
                {
                    foreach (var listaDia in listaSemanal) //lista de la semana
                    {
                        if (listaDia.fechaDeLista == datoClimaJSON.dt_txt.Date)
                        {
                            listaDia.listaDatosClimaPorDia.Add(datoClimaJSON);
                            fechaEsIgual = true;
                            break;
                        }
                    }
                    if (fechaEsIgual)
                    {
                        continue;
                    }
                    else
                    {
                        var listaPorDiaAux = new ListaDatosPorDiA();
                        listaPorDiaAux.fechaDeLista = datoClimaJSON.dt_txt.Date;
                        listaPorDiaAux.listaDatosClimaPorDia = new List<List>();
                        listaPorDiaAux.listaDatosClimaPorDia.Add(datoClimaJSON);
                        listaSemanal.Add(listaPorDiaAux);
                        continue;
                    }
                }
                else
                {
                    listaPorDia.fechaDeLista = datoClimaJSON.dt_txt.Date;
                    listaPorDia.listaDatosClimaPorDia = new List<List>();
                    listaPorDia.listaDatosClimaPorDia.Add(datoClimaJSON);
                    listaSemanal.Add(listaPorDia);
                    continue;
                }
            }
        }

        datosMapeados.listaClimaPorDiaInterfaz = new List<ClimaPorDia>();

        foreach (var dia in listaSemanal)
        {
            var diaInterfaz = new ClimaPorDia();
            diaInterfaz.temperaturaMaxima = ObtenerTempMaxDia(dia.listaDatosClimaPorDia).ToString("0").Replace(',', '.');
            diaInterfaz.temperaturaMinima = ObtenerTempMinXDia(dia.listaDatosClimaPorDia).ToString("0").Replace(',', '.');
            diaInterfaz.probabilidadLluviaMaxima = ObtenerPrecipitacionesPromedio(dia.listaDatosClimaPorDia).ToString("0").Replace(',', '.'); 
            CultureInfo ci = new CultureInfo("es-ES");
            diaInterfaz.diaDeLaSemana = dia.listaDatosClimaPorDia[0].dt_txt.DayOfWeek.ToString("D", new CultureInfo("es-ES"));
            diaInterfaz.fecha = dia.listaDatosClimaPorDia[0].dt_txt.Date.ToString("dd MMMM");
            diaInterfaz.descripcionClima = ObtenerDescripcionDelDia(dia.listaDatosClimaPorDia);
            diaInterfaz.icono = ObtenerIconoDelDia(dia.listaDatosClimaPorDia);
            datosMapeados.listaClimaPorDiaInterfaz.Add(diaInterfaz);
            if (datosMapeados.listaClimaPorDiaInterfaz.Count == 5)
            {
                break;
            }
        }

        return datosMapeados.listaClimaPorDiaInterfaz;
    }

    private string ObtenerIconoDelDia(IList<List> listaDatosClimaPorDia)
    {
        string iconoDelDia = string.Empty;
        var diccionarioDescripcionCantidad = new Dictionary<string, int>();

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (!diccionarioDescripcionCantidad.ContainsKey(datoClimaPorHora.weather[0].icon))
            {
                diccionarioDescripcionCantidad.Add(datoClimaPorHora.weather[0].icon, 1);
                continue;
            }
            else
            {
                foreach (var item in diccionarioDescripcionCantidad)
                {
                    if (datoClimaPorHora.weather[0].icon == item.Key)
                    {
                        var contador = item.Value;
                        contador++;
                        diccionarioDescripcionCantidad[item.Key] = contador;
                        continue;
                    }
                }

            }
        }

        var valorMaximoDescripcion = diccionarioDescripcionCantidad.Values.Max();
        var descripcionCantMaxima = diccionarioDescripcionCantidad.FirstOrDefault(x => x.Value == valorMaximoDescripcion).Key;

        return descripcionCantMaxima;
    }

    public double ObtenerTempMaxDia(IList<List> listaDatosClimaPorDia)
    {
        double temperaturaMaximaPorDia = 0;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (temperaturaMaximaPorDia == 0 || datoClimaPorHora.main.temp_max > temperaturaMaximaPorDia)
            {
                temperaturaMaximaPorDia = datoClimaPorHora.main.temp_max;
            }
            else
            {
                continue;
            }
        }

        return temperaturaMaximaPorDia;
    }

    public double ObtenerTempMinXDia(IList<List> listaDatosClimaPorDia)
    {
        double temperaturaMinimaPorDia = 0;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (temperaturaMinimaPorDia == 0 || datoClimaPorHora.main.temp_min < temperaturaMinimaPorDia)
            {
                temperaturaMinimaPorDia = datoClimaPorHora.main.temp_min;
            }
            else
            {
                continue;
            }
        }

        return temperaturaMinimaPorDia;
    }

    public double ObtenerPrecipitacionesPromedio(IList<List> listaDatosClimaPorDia)
    {
        double precipitacionProm = 0;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (precipitacionProm == 0 || datoClimaPorHora.pop > precipitacionProm)
            {
                precipitacionProm = datoClimaPorHora.pop;
            }
            else
            {
                continue;
            }
        }
        precipitacionProm = precipitacionProm * 100;

        return precipitacionProm;
    }

    public double ObtenerVelocidadVientoMax(IList<List> listaDatosClimaPorDia)
    {
        double velocidadMax = 0;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (velocidadMax == 0 || datoClimaPorHora.wind.speed > velocidadMax)
            {
                velocidadMax = datoClimaPorHora.wind.speed;
            }
            else
            {
                continue;
            }
        }

        return velocidadMax;
    }

    public double ObtenerVelocidadVientoMin(IList<List> listaDatosClimaPorDia)
    {
        double velocidadMin = 0;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (velocidadMin == 0 || datoClimaPorHora.wind.speed < velocidadMin)
            {
                velocidadMin = datoClimaPorHora.wind.speed;
            }
            else
            {
                continue;
            }
        }

        return velocidadMin;
    }

    public string ObtenerDescripcionDelDia(IList<List> listaDatosClimaPorDia)
    {
        var diccionarioDescripcionCantidad = new Dictionary<string, int>();
        var descripcionDelDia = string.Empty;

        foreach (var datoClimaPorHora in listaDatosClimaPorDia)
        {
            if (!diccionarioDescripcionCantidad.ContainsKey(datoClimaPorHora.weather[0].description))
            {
                diccionarioDescripcionCantidad.Add(datoClimaPorHora.weather[0].description, 1);
                continue;
            }
            else
            {
                foreach (var item in diccionarioDescripcionCantidad)
                {
                    if (datoClimaPorHora.weather[0].description == item.Key)
                    {
                        var contador = item.Value;
                        contador++;
                        diccionarioDescripcionCantidad[item.Key] = contador;
                        continue;
                    }
                }
                
            }
        }

        var valorMaximoDescripcion = diccionarioDescripcionCantidad.Values.Max();
        var descripcionCantMaxima  = diccionarioDescripcionCantidad.FirstOrDefault(x => x.Value == valorMaximoDescripcion).Key;
        return descripcionDelDia = descripcionCantMaxima;
    }
}

public class ListaDatosPorDiA
{
    public DateTime fechaDeLista { get; set; }
    public IList<List> listaDatosClimaPorDia { get; set; }
}
