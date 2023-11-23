using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Repuestos
    {

        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public string Precio { get; set; }
        public string Caracteristicas { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
    }
}
