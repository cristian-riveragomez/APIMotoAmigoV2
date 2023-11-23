using Microsoft.EntityFrameworkCore;
using MotoAmigoApiBD.Datos;
using System.Net.Mail;
using WebApi.Models;
using System.Net;
using System.Drawing;

public class LogicaDeDatos
{
    private readonly AccesoADatos _context;
    public LogicaDeDatos(AccesoADatos context)
    {
        _context = context;
    }

    #region AccionesUsuario

    public List<Usuario> ObtenerListaUsuarios()
    {
        return _context.Usuarios.Where(u=> u.EsAdmin == false).ToList();    
    }

    public Usuario ObtenerUsuario(int IdUsuario)
    {
        try
        {
            var usuarioBd = _context.Usuarios.FirstOrDefault(m => m.Id == IdUsuario);

            return usuarioBd;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public Usuario InsertarUsuario(Usuario usuario)
    {
        try
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var id = usuario.Id;

            var usuarioInsertado = _context.Usuarios.FirstOrDefault(m => m.Id == id);
            
            if (usuarioInsertado == null)
            {
                return null;
            }

            return usuarioInsertado;
        }
        catch (Exception ex)
        {
            throw ex;
        }        
    }

    public void ActualizarUsuario(Usuario usuarioModificado)
    {
        try
        {
            var usuarioBd = _context.Usuarios.FirstOrDefault(m => m.Id == usuarioModificado.Id);
            usuarioBd.Nombre = usuarioModificado.Nombre;
            usuarioBd.Apellido = usuarioModificado.Apellido;
            usuarioBd.NombreUsuario = usuarioModificado.NombreUsuario;
            usuarioBd.Contrasena = usuarioModificado.Contrasena;
            usuarioBd.Email = usuarioModificado.Email;
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }       
    }

    public Usuario ValidarUsuarioDuplicado(Usuario nuevoUsuario)
    {
        try
        {
            return  _context.Usuarios.FirstOrDefault(u => u.Email== nuevoUsuario.Email && u.NombreUsuario == nuevoUsuario.NombreUsuario);         
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool EliminarUsuario(int idUsuario)
    {
        try
        {
            var productosDelUsuario = ObtenerListaProductosPorUsuario(idUsuario);

            if (productosDelUsuario.Count > 0)
            {
                foreach (var producto in productosDelUsuario)
                {
                    EliminarProducto(producto.IdProducto);
                }                
            }
            
             var usuarioAEliminar = _context.Usuarios.FirstOrDefault(m => m.Id == idUsuario);
            _context.Usuarios.Remove(usuarioAEliminar);
            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }

    public Usuario Login(UsuarioLogin usuarioLogin)
    {
        try
        {
            var usuario = _context.Usuarios.FirstOrDefault(m => (m.Email == usuarioLogin.emailUsuario && m.Contrasena == usuarioLogin.contrasena) || (m.NombreUsuario == usuarioLogin.emailUsuario && m.Contrasena == usuarioLogin.contrasena) );

            if (usuario == null)
            {
                return null;
            }

            return usuario;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region Productos
    public List<Productos> ObtenerListaProductos()
    {
        try
        {
            return _context.Productos.Where(p=> p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public List<Productos> ObtenerListaProductosPorUsuario(int idUsuario)
    {
        try
        {
            return _context.Productos.Where(p => p.IdUsuario == idUsuario).ToList();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public Productos ObtenerProducto(int IdProducto)
    {
        try
        {
            var producto = _context.Productos.FirstOrDefault(m => m.IdProducto == IdProducto);

            return producto;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public List<Productos> ObtenerProductosPorDenuncias()
    {
        try
        {
            var results = _context.Productos.Where(p => p.CantDenuncias >= 3).ToList();

            return results;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public List<Productos> ObtenerProductosPendientesDeAprobacion()
    {
        try
        {
            var results = _context.Productos.Where(p => p.Estado == "PENDIENTE").ToList();

            return results;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public List<Productos> ObtenerProductosPorPrecio(string? precioMin, string? precioMax)
    {
        try
        {            
            var resultados =  new List<Productos>();
            if ((precioMin == null || precioMin == "undefined") && (precioMax == null || precioMax == "undefined"))
            {
                resultados = this.ObtenerListaProductos();
            }
            else if ((precioMin != null && precioMin != "undefined") && (precioMax != null && precioMax != "undefined"))
            {
                var precioMinimo = int.Parse(precioMin);
                var precioMaximo = int.Parse(precioMax);

                resultados = _context.Productos.Where(p => p.Precio >= precioMinimo && p.Precio <= precioMaximo && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if ((precioMin != null && precioMin != "undefined") && (precioMax == null || precioMax == "undefined"))
            {
                var precioMinimo = int.Parse(precioMin);

                resultados = _context.Productos.Where(p => p.Precio >= precioMinimo && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if ((precioMin == null || precioMin == "undefined") && (precioMax != null || precioMax != "undefined"))
            {
                var precioMaximo = int.Parse(precioMax);
                resultados = _context.Productos.Where(p => p.Precio <= precioMaximo && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }

            return resultados;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<Productos> ObtenerProductosPorFiltro(string? precioMinimo, string? precioMaximo, bool? accesorioCheck, bool? repuestoCheck)
    {
        try
        {
            var resultados = new List<Productos>();

            if (precioMinimo != "undefined" && precioMaximo == "undefined" && accesorioCheck == false && repuestoCheck == false)
            {
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio >= precioMinimoEntero && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo == "undefined" && precioMaximo != "undefined" && accesorioCheck == false && repuestoCheck == false)
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                resultados = _context.Productos.Where(p => p.Precio <= precioMaxEntero && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if ((precioMinimo != "undefined" && precioMaximo != "undefined" && accesorioCheck == false && repuestoCheck == false) ||
                    (precioMinimo != "undefined" && precioMaximo != "undefined" && accesorioCheck == true && repuestoCheck == true))
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio >= precioMinimoEntero && p.Precio <= precioMaxEntero && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo != "undefined" && precioMaximo != "undefined" && accesorioCheck == true && repuestoCheck == false)
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio >= precioMinimoEntero && p.Precio <= precioMaxEntero && p.tipoRepuesto == "A" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo != "undefined" && precioMaximo != "undefined" && accesorioCheck == false && repuestoCheck == true)
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio >= precioMinimoEntero && p.Precio <= precioMaxEntero && p.tipoRepuesto == "R" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo == "undefined" && precioMaximo == "undefined" && accesorioCheck == true && repuestoCheck == false)
            {
                resultados = _context.Productos.Where(p => p.TipoProducto == "A" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo == "undefined" && precioMaximo == "undefined" && accesorioCheck == false && repuestoCheck == true)
            {
                resultados = _context.Productos.Where(p => p.TipoProducto == "R" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo == "undefined" && precioMaximo != "undefined" && accesorioCheck == true && repuestoCheck == false)
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                resultados = _context.Productos.Where(p => p.Precio <= precioMaxEntero && p.TipoProducto == "A" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo != "undefined" && precioMaximo == "undefined" && accesorioCheck == true && repuestoCheck == false)
            {
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio <= precioMinimoEntero && p.TipoProducto == "A" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo == "undefined" && precioMaximo != "undefined" && accesorioCheck == false && repuestoCheck == true)
            {
                var precioMaxEntero = int.Parse(precioMaximo);
                resultados = _context.Productos.Where(p => p.Precio <= precioMaxEntero && p.TipoProducto == "R" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }
            else if (precioMinimo != "undefined" && precioMaximo == "undefined" && accesorioCheck == false && repuestoCheck == true)
            {
                var precioMinimoEntero = int.Parse(precioMinimo);
                resultados = _context.Productos.Where(p => p.Precio <= precioMinimoEntero && p.TipoProducto == "R" && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();
            }           
            else
            {
                resultados = this.ObtenerListaProductos();
            }
 
            return resultados;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


        public void ActualizarProductoDenunciado(int idProducto)
    {
        try
        {
            var producto = this.ObtenerProducto(idProducto);

            producto.CantDenuncias = 0;

            var borrarProductoDenunciadosPorIdProducto = $"DELETE FROM ProductoUsuarioDenunciado where ProductoId = {idProducto}";

            _context.Database.ExecuteSqlRaw(borrarProductoDenunciadosPorIdProducto);

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public bool ActualizacionDeEstado(int idProducto, string nuevoEstado)
    {
        try
        {
            var producto = this.ObtenerProducto(idProducto);

            producto.Estado = nuevoEstado;

            _context.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }

    public List<Productos> ObtenerProductosPorDestacados(int idUsuario)
    {
        try
        {
            var listaDestacados = new List<Productos>();

            var productosUsuarioDestacados = _context.ProductoUsuarioDestacado.Where(p=> p.UsuarioId == idUsuario).ToList();

            foreach (var productoUsuario in productosUsuarioDestacados)
            {
                listaDestacados.Add(ObtenerProducto(productoUsuario.ProductoId));
            }

            return listaDestacados;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
    public void DenunciarProducto(int idProducto, int idUsuario)
    {
        try
        {            
            var productoADenunciar = _context.Productos.Where(p=> p.IdProducto == idProducto).FirstOrDefault();

            productoADenunciar.CantDenuncias++;

            _context.SaveChanges();

            var insertProductoDestacado = $"INSERT INTO ProductoUsuarioDenunciado (UsuarioId, ProductoId) VALUES ('{idUsuario}', {idProducto})";

            _context.Database.ExecuteSqlRaw(insertProductoDestacado);
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public bool ValidarProductoSiEstaDenunciado(int idUsuario, int idProducto)
    {
        try
        {
            var productoDenunciadoPorId = _context.ProductoUsuarioDenunciado.Where(p => p.UsuarioId == idUsuario && p.ProductoId == idProducto).FirstOrDefault();

            if (productoDenunciadoPorId == null)
            {
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public List<Productos> BuscarProductosPorIdUsuario(int  idUsuario)
    {
        try
        {
            var results = _context.Productos.Where(p => p.IdUsuario == idUsuario).ToList();

            return results;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public List<Productos> BuscarProducto(string filtro)
    {
        try
        {
            var results = _context.Productos.Where(p => p.Nombre.Contains(filtro) && p.CantDenuncias < 3 && p.Estado == "HABILITADO").ToList();

            return results;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public bool ValidarSiProductoEsDestacado(int idUsuarioDestacado, int idProducto)
    {
        try
        {
            var esDestacado = false;

            var results = _context.ProductoUsuarioDestacado.FirstOrDefault(p => p.UsuarioId == idUsuarioDestacado && p.ProductoId == idProducto);

            if (results != null)
            {
                esDestacado = true;
            }

            return esDestacado;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public void InsertarProductoDestacado(int idUsuarioDestacado, int idProducto) 
    {
        try
        {
            var insertProductoDestacado = $"INSERT INTO ProductoUsuarioDestacado (UsuarioId, ProductoId) VALUES ('{idUsuarioDestacado}', {idProducto})";

            _context.Database.ExecuteSqlRaw(insertProductoDestacado);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void EliminarProductoDestacado(int idUsuarioDestacado, int idProducto)
    {
        try
        {
            var borrarProductoDestacado = $"DELETE FROM ProductoUsuarioDestacado where UsuarioId = {idUsuarioDestacado} AND ProductoId = {idProducto}";

            _context.Database.ExecuteSqlRaw(borrarProductoDestacado);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void InsertarProducto(ProductoFormulario productoForm)
    {
        try
        {
            var producto = new Productos();
            producto.Nombre = productoForm.Nombre;
            producto.Detalle = productoForm.Detalle;
            producto.Precio = int.Parse(productoForm.Precio);
            producto.Marca = productoForm.Marca;
            producto.Modelo = productoForm.Modelo;
            producto.TipoProducto = productoForm.TipoProducto;
            producto.IdUsuario = productoForm.IdUsuario;
            producto.CantDenuncias = 0;
            producto.Destacado = false;
            producto.Estado = productoForm.Estado;
            producto.tipoRepuesto = productoForm.tipoRepuesto;
            
            const int limiteTamanioImagen = 1 * 1024 * 1024; // 1MB

            if (productoForm.imagen != null && productoForm.imagen.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {                   
                    if (productoForm.imagen.Length <= limiteTamanioImagen)
                    {
                        productoForm.imagen.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();

                        producto.imagenNombre = productoForm.imagen.FileName;
                        producto.imagenContenido = fileBytes;
                    }
                    else
                    {
                        byte[] imagenRedimensionada = RedimensionarImagen(productoForm.imagen, 450, 500);

                        producto.imagenNombre = productoForm.imagen.FileName;
                        producto.imagenContenido = imagenRedimensionada;
                    }                    
                }
            }

            if (productoForm.imagenRepuesto != null && productoForm.imagenRepuesto.Length > 0)
            {
                using (var memoryStream2 = new MemoryStream())
                {
                    if (productoForm.imagenRepuesto.Length <= limiteTamanioImagen)
                    {
                        productoForm.imagenRepuesto.CopyToAsync(memoryStream2);
                        byte[] fileBytes = memoryStream2.ToArray();

                        producto.imagenRepuestoNombre = productoForm.imagenRepuesto.FileName;
                        producto.imagenRepuestoContenido = fileBytes;
                    }
                    else
                    {
                        byte[] imagenRedimensionada = RedimensionarImagen(productoForm.imagenRepuesto, 450, 500);

                        producto.imagenRepuestoNombre = productoForm.imagenRepuesto.FileName;
                        producto.imagenRepuestoContenido = imagenRedimensionada;
                    }
                }
            }

            _context.Productos.Add(producto);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public byte[] RedimensionarImagen(IFormFile imagen, int nuevoAncho, int nuevoAlto)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            imagen.CopyTo(memoryStream);

            using (Image imagenOriginal = Image.FromStream(memoryStream))
            {
                int ancho = imagenOriginal.Width;
                int alto = imagenOriginal.Height;

                string tipoImagen = string.Empty;

                if (ancho > alto) // Horizontal
                {        
                    tipoImagen = "H";
                }
                else // Vertical
                {
                    tipoImagen = "V";
                }

                using (Bitmap nuevaImagen = new Bitmap(nuevoAncho, nuevoAlto))
                {
                    using (Graphics graficos = Graphics.FromImage(nuevaImagen))
                    {
                        if (tipoImagen == "H")
                        {
                            graficos.DrawImage(imagenOriginal, 0, 0, nuevoAncho, nuevoAlto);
                        }
                        else if (tipoImagen == "V")
                        {
                            graficos.DrawImage(imagenOriginal, 0, 0, nuevoAlto, nuevoAncho);
                        }

                        using (MemoryStream nuevoMemoryStream = new MemoryStream())
                        {
                            nuevaImagen.Save(nuevoMemoryStream, imagenOriginal.RawFormat);
                            return nuevoMemoryStream.ToArray();
                        }
                    }
                }
            }
        }
    }

    public void ActualizarProducto(ProductoFormulario ProductoModificado)
    {
        try
        {
            var ProductoBD = _context.Productos.FirstOrDefault(m => m.IdProducto == ProductoModificado.IdProducto);

            ProductoBD.Nombre = ProductoModificado.Nombre;
            ProductoBD.Detalle = ProductoModificado.Detalle;
            ProductoBD.Precio = int.Parse(ProductoModificado.Precio);
            ProductoBD.Marca = ProductoModificado.Marca;
            ProductoBD.Modelo = ProductoModificado.Modelo;

            if (ProductoModificado.Estado != null)
            {
                ProductoBD.Estado = ProductoModificado.Estado;
            }

            const int limiteTamanioImagen = 1 * 1024 * 1024; // 1MB
            if (ProductoModificado.imagenRepuesto != null && ProductoModificado.imagenRepuesto.Length > 0)
            {
                using (var memoryStream2 = new MemoryStream())
                {
                    if (ProductoModificado.imagenRepuesto.Length <= limiteTamanioImagen)
                    {
                        ProductoModificado.imagenRepuesto.CopyToAsync(memoryStream2);
                        byte[] fileBytes = memoryStream2.ToArray();

                        ProductoBD.imagenRepuestoNombre = ProductoModificado.imagenRepuesto.FileName;
                        ProductoBD.imagenRepuestoContenido = fileBytes;
                    }
                    else
                    {
                        byte[] imagenRedimensionada = RedimensionarImagen(ProductoModificado.imagenRepuesto, 450, 500);

                        ProductoBD.imagenRepuestoNombre = ProductoModificado.imagenRepuesto.FileName;
                        ProductoBD.imagenRepuestoContenido = imagenRedimensionada;
                    }
                }
            }

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool EliminarProducto(int idProducto)
    {
        try
        {
            if (EliminarRelacionProducto(idProducto))
            {
                var productoAEliminar = _context.Productos.FirstOrDefault(m => m.IdProducto == idProducto);
                _context.Productos.Remove(productoAEliminar);
                _context.SaveChanges();                
            }
            return true;
        }
        catch (Exception ex)
        {            
            throw ex;
            return false;
        }
    }

    public bool EliminarRelacionProducto(int idProducto)
    {
        try
        {
            var borrarProductoDestacadoPorIdProducto = $"DELETE FROM ProductoUsuarioDestacado where ProductoId = {idProducto}";

            _context.Database.ExecuteSqlRaw(borrarProductoDestacadoPorIdProducto);

            var borrarProductoDenunciadosPorIdProducto = $"DELETE FROM ProductoUsuarioDenunciado where ProductoId = {idProducto}";

            _context.Database.ExecuteSqlRaw(borrarProductoDenunciadosPorIdProducto);
            
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    #endregion

    #region Mail

    public void InsertarMail(Mail mailAEnviar)
    {
        try
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var mailEmisor   = "cristianriverag95@hotmail.com";
            var contraseña   = "rivera95";

            var usuarioReceptor = this.ObtenerUsuario(mailAEnviar.IdUsuarioReceptor);

            var mailReceptor = usuarioReceptor.Email;
            var asuntoMail = mailAEnviar.Titulo;
            var bodyMail = mailAEnviar.Cuerpo;
            var puertoSmtp = 587;
            var host = "smtp.office365.com";            

            MailMessage mail = new MailMessage(mailEmisor, mailReceptor);

            SmtpClient client = new SmtpClient();
            client.Port = puertoSmtp;
            client.Host = host;
            client.Credentials = new NetworkCredential(mailEmisor, contraseña);
            client.EnableSsl = true;
            mail.Subject = asuntoMail;
            mail.Body = bodyMail;
            client.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion
}

