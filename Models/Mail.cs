using System.ComponentModel.DataAnnotations;

public class Mail
{
    [Key]
    public int Id { get; set; }
    public bool Enviado { get; set; }
    public int IdUsuarioReceptor { get; set; }
    public string Titulo { get; set; }
    public string Cuerpo { get; set; }    
}

