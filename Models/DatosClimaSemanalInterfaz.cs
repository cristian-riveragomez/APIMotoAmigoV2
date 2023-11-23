public class DatosClimaSemanalInterfaz
{
    public List<ClimaPorDia> listaClimaPorDiaInterfaz{ get; set; }
}

public class ClimaPorDia
{
    public string temperaturaMaxima { get; set; }
    public string temperaturaMinima { get; set; }
    public string descripcionClima { get; set; }
    public string probabilidadLluviaMaxima { get; set; }    
    public string diaDeLaSemana { get; set; }
    public string fecha { get; set; }
    public string icono { get; set; }
}

