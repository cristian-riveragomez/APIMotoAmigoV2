using System.ComponentModel.DataAnnotations;

public class Productos
{
    [Key]
    public int IdProducto { get; set; }
    public string Nombre { get; set; }
    public string Detalle { get; set; }
    public int Precio { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int IdUsuario { get; set; }
    public bool Destacado { get; set; }
    public int CantDenuncias { get; set; }
    public string TipoProducto { get; set; }
    public byte[] imagenContenido { get; set; }
    public string imagenNombre { get; set; }
    public string Estado { get; set; }
    public string? imagenRepuestoNombre { get; set; }
    public byte[]? imagenRepuestoContenido { get; set; }
    public string? tipoRepuesto { get; set; }
    
}

