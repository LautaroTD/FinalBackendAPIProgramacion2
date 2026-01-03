using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FinalBackendAPIProgramacion2.Services
{
    public class ArticuloRelacionadoService : IArticuloRelacionadoService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<ArticuloRelacionadoService > _logger;

        public ArticuloRelacionadoService(Final_Programacion_2Context context, ILogger<ArticuloRelacionadoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DTOArticuloRelacionado?>> ObtenerPorId(int id)
        {
            //nota: si no haces await en este bloque se rompe porque intenta hacer mas de una llamada al dbcontext al mismo tiempo.
            var relaciones = await _context.ArticuloRelacionado.ToListAsync();
            var usuarios = await _context.Usuario.ToListAsync();
            var articulos = await _context.Articulo.ToListAsync();

            List<DTOUsuario> listaUsuarioReducida = new List<DTOUsuario>();

            foreach (var usuario in usuarios)
            {
                DTOUsuario usuarioTemp = new DTOUsuario
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre
                };
                listaUsuarioReducida.Add(usuarioTemp);
            }

            List<DTOArticulo> listaArticuloReducida = new List<DTOArticulo>();

            foreach (var articulo in articulos)
            {
                DTOArticulo articuloTemp = new DTOArticulo
                {
                    Id = articulo.Id,
                    NombreProducto = articulo.Nombre
                };
                listaArticuloReducida.Add(articuloTemp);
            }

            List<DTOArticuloRelacionado> listaArticuloRelacionado = new List<DTOArticuloRelacionado>();

            foreach (var relacion in relaciones)
            {
                var publicador = listaUsuarioReducida.FirstOrDefault(e=> e.Id == relacion.IdUsuario);
                if(publicador is null)
                {
                    publicador = new DTOUsuario
                    {
                        Nombre = "Usuario no encontrado"
                    };
                }

                var articuloPrimero = listaArticuloReducida.FirstOrDefault(e => e.Id == relacion.IdPrimerArticulo);
                if(articuloPrimero is null){
                    articuloPrimero = new DTOArticulo
                    {
                        NombreProducto = "Articulo no encontrado"
                    };
                }

                var articuloSegundo = listaArticuloReducida.FirstOrDefault(e => e.Id == relacion.IdSegundoArticulo);
                if (articuloSegundo is null)
                {
                    articuloSegundo = new DTOArticulo
                    {
                        NombreProducto = "Articulo no encontrado"
                    };
                }
                
                if(relacion.Id == id)
                {
                    DTOArticuloRelacionado articuloRelacionadoTemp = new DTOArticuloRelacionado
                    {
                        Id = relacion.Id,
                        NombrePublicador = publicador.Nombre,
                        NombrePrimerArticulo = articuloPrimero.NombreProducto,
                        NombreSegundoArticulo = articuloSegundo.NombreProducto,
                        IdPublicador = publicador.Id,
                        IdPrimerArticulo = articuloPrimero.Id,
                        IdSegundoArticulo = articuloSegundo.Id
                    };

                    listaArticuloRelacionado.Add(articuloRelacionadoTemp);
                }
            }

            return listaArticuloRelacionado;
        }

        public async Task<bool> Crear(DTOArticuloRelacionado nuevaRelacion)
        {

            if (string.IsNullOrWhiteSpace(nuevaRelacion.NombrePublicador) || string.IsNullOrWhiteSpace(nuevaRelacion.NombrePrimerArticulo) || string.IsNullOrWhiteSpace(nuevaRelacion.NombreSegundoArticulo)) //se usa 0m para definir que ese 0 es decimal, como la variable precio.
            {
                throw new ArgumentException("No se rellenaron los campos de la nueva relacion correctamente, intente de nuevo.");
            }

            //debido a que en esta linea faltaba el await, te daba un error silencioso en el swagger diciendote que estas haciendo mas de una llamada al dbcontext
            var usuario = await _context.Usuario.FirstOrDefaultAsync(e => e.Nombre == nuevaRelacion.NombrePublicador);

            if (usuario is null)
            {
                throw new ArgumentException("Ingrese un nombre de usuario que exista en el sistema");
            }

            var articuloPrimero = await _context.Articulo.FirstOrDefaultAsync(e => e.Nombre == nuevaRelacion.NombrePrimerArticulo);
            
            if(articuloPrimero is null)
            {
                throw new ArgumentException("El primer articulo ingresado no existe en el sistema, ingrese un articulo valido e intente de nuevo.");
            }

            var articuloSegundo = await _context.Articulo.FirstOrDefaultAsync(e => e.Nombre == nuevaRelacion.NombreSegundoArticulo);

            if (articuloSegundo is null)
            {
                throw new ArgumentException("El segundo articulo ingresado no existe en el sistema, ingrese un articulo valido e intente de nuevo.");
            }

            var idPublicador = usuario.Id;
            var idPrimerArticulo = articuloPrimero.Id;
            var idSegundoArticulo = articuloSegundo.Id;

            ArticuloRelacionado relacionACrear = new ArticuloRelacionado
            {
                IdUsuario = idPublicador,
                IdPrimerArticulo = idPrimerArticulo,
                IdSegundoArticulo = idSegundoArticulo
            };

            bool ocupado = true;
            var random = new Random(); //esto es importante, el "new Random();" debe estar FUERA de la repeticion do{}while. y el random.Next debe estar DENTRO de la repeticion.

            do
            {
                int numero = random.Next();

                var relacion = _context.ArticuloRelacionado.Find(numero);

                if (relacion is null)
                {
                    ocupado = false;
                    relacionACrear.Id = numero;
                }

            } while (ocupado == true);

            _context.ArticuloRelacionado.Add(relacionACrear);

            try
            {
                //asi NO: await _context.SaveChanges();
                //asi SI: await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear la relacion del usuario: {nuevaRelacion.NombrePublicador}, relacionando el articulo: {nuevaRelacion.NombrePrimerArticulo}, y el articulo:{nuevaRelacion.NombreSegundoArticulo}.");
                return false;
            }

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var relacionExistente = await _context.ArticuloRelacionado.FindAsync(id);

            if (relacionExistente is null)
            { 
                throw new ArgumentException("No se encontro la relacion en la base de datos.");
            }

            _context.ArticuloRelacionado.Remove(relacionExistente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la relacion de id: {id}.");
                return false;
            }

            return true;
        }
    }
}
