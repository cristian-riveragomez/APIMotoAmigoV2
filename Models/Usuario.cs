using System.ComponentModel.DataAnnotations;

public class Usuario
{
    [Key]
    public int Id { get; set; }
    public string Nombre {get; set;}
    public string Apellido {get; set;}
    public string NombreUsuario {get; set;}
    public string Contrasena {get; set;}
    public string Email { get; set; }
    public bool EsAdmin { get; set; }   
}

