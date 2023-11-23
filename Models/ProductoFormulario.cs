using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class ProductoFormulario
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public string Precio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int IdUsuario { get; set; }
        public string? TipoProducto { get; set; } 
        public IFormFile? imagen { get; set; }
        public string? Estado { get; set; }
        public IFormFile? imagenRepuesto { get; set; }
        public string? tipoRepuesto { get; set; }
        
    }
}
