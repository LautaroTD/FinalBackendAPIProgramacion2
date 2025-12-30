using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalBackendAPIProgramacion2.Services
{
    public class ResenaService : IResenaService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<ResenaService> _logger;
        public ResenaService(Final_Programacion_2Context context, ILogger<ResenaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DTOResena?>> ObtenerTodos()
        {
            var resenaAsync = await _context.Resena.ToListAsync();
            var usuarioAsync = await _context.Usuario.ToListAsync();
            var articuloAsync = await _context.Articulo.ToListAsync();

            List<DTOUsuario> listaUsuarioReducida = new List<DTOUsuario>();

            foreach (var usuario in usuarioAsync)
            {
                DTOUsuario usuarioTemp = new DTOUsuario
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre
                };
                listaUsuarioReducida.Add(usuarioTemp);
            }

            List<DTOArticulo> listaArticuloReducida = new List<DTOArticulo>();

            foreach (var articulo in articuloAsync)
            {
                DTOArticulo articuloTemp = new DTOArticulo
                {
                    Id = articulo.Id,
                    NombreProducto = articulo.Nombre
                };
                listaArticuloReducida.Add(articuloTemp);
            }

            List<DTOResena> listaResena = new List<DTOResena>();

            foreach (var resena in resenaAsync)
            {
                var publicador = listaUsuarioReducida.Find(e => e.Id == resena.IdUsuario);

                if (publicador is null)
                {
                    publicador = new DTOUsuario
                    {
                        Nombre = "Usuario no encontrado"
                    };
                }

                var articuloInfo = listaArticuloReducida.Find(e => e.Id == resena.IdArticulo);

                if (articuloInfo is null)
                {
                    articuloInfo = new DTOArticulo
                    {
                        NombreProducto = "Articulo no encontrado"
                    };
                }

                DTOResena resenaTemp = new DTOResena
                {
                    Id = resena.Id,
                    IdArticulo = articuloInfo.Id,
                    IdUsuario = publicador.Id,
                    NombrePublicador = publicador.Nombre,
                    NombreProducto = articuloInfo.NombreProducto,
                    Calificacion = resena.Calificacion,
                    Descripcion = resena.Descripcion
                };
                listaResena.Add(resenaTemp);
            }

            return listaResena;
        }

        public async Task<DTOResena?> ObtenerPorId(int id)
        {
            var resena = _context.Resena.FindAsync(id);

            if (await resena is null)
            {
                throw new KeyNotFoundException($"Reseña con id {id} no encontrado.");
            }

            int IdTempArticulo = (await resena).IdArticulo;

            var articulo = _context.Articulo.FindAsync(IdTempArticulo);

            string nombreArticulo = (await articulo)?.Nombre ?? "Articulo no encontrado";

            int IdTempUsuario = (await resena).IdUsuario;

            var usuario = _context.Usuario.FindAsync(IdTempUsuario);

            string nombrePublicador = (await usuario)?.Nombre ?? "Usuario no encontrado";

            DTOResena resenaSalida = new DTOResena
            {
                Id = (await resena).Id,
                Calificacion = (await resena).Calificacion,
                Descripcion = (await resena).Descripcion,
                NombreProducto = nombreArticulo,
                NombrePublicador = nombrePublicador
            };

            return resenaSalida;
        }

        public async Task<bool> Crear(DTOResena nuevaResena)
        {

            if (string.IsNullOrWhiteSpace(nuevaResena.NombrePublicador) || string.IsNullOrWhiteSpace(nuevaResena.NombreProducto) || nuevaResena.Calificacion < 0 || nuevaResena.Calificacion > 5)
            {
                throw new ArgumentException("No se rellenaron los campos de la reseÑa correctamente, intente de nuevo.");
            }

            //debido a que en esta linea faltaba el await, te daba un error silencioso en el swagger diciendote que estas haciendo mas de una llamada al dbcontext
            var usuario = await _context.Usuario.FirstOrDefaultAsync(e => e.Nombre == nuevaResena.NombrePublicador);

            if (usuario is null)
            {
                throw new ArgumentException("Ingrese un nombre de usuario que exista en el sistema");
            }

            var articulo = await _context.Articulo.FirstOrDefaultAsync(e => e.Nombre == nuevaResena.NombreProducto);

            if (articulo is null)
            {
                throw new ArgumentException("Ingrese un nombre de articulo que exista en el sistema");
            }

            var idDeArticulo = articulo.Id;
            var idDeVendedor = usuario.Id;

            Resena resenaACrear = new Resena
            {
                IdUsuario = idDeVendedor,
                IdArticulo = idDeArticulo,
                Calificacion = nuevaResena.Calificacion,
                Descripcion = nuevaResena.Descripcion
            };

            bool ocupado = true;
            var random = new Random(); //esto es importante, el "new Random();" debe estar FUERA de la repeticion do{}while. y el random.Next debe estar DENTRO de la repeticion.

            do
            {
                int numero = random.Next();

                var resena = _context.Resena.Find(numero);

                if (resena is null)
                {
                    ocupado = false;
                    resenaACrear.Id = numero;
                }

            } while (ocupado == true);

            _context.Resena.Add(resenaACrear);

            try
            {
                //asi NO: await _context.SaveChanges();
                //asi SI: await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear la reseña del producto {nuevaResena.NombreProducto} y el usuario {nuevaResena.NombrePublicador}");
                return false;
            }

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var resenaAEliminar = await _context.Resena.FindAsync(id);
            if (resenaAEliminar is null)
            {
                throw new KeyNotFoundException($"Reseña con id {id} no encontrado.");
            }
            _context.Resena.Remove(resenaAEliminar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la reseña con id {id}");
                return false;
            }
            return true;
        }

        public async Task<bool> Editar(int id, int calificacion, string descripcion)
        {
            if (calificacion < 1 || calificacion > 5)
            {
                throw new ArgumentException("Todos los campos son obligatorios, rellene los campos e intentelo de nuevo.");
            }
            
            var resenaExistente = await _context.Resena.FindAsync(id);

            if (resenaExistente is null)
            {
                throw new ArgumentException("No se encontro la resena que quiere editar, intente de nuevo.");
            }

            resenaExistente.Calificacion = calificacion;
            resenaExistente.Descripcion = descripcion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar los cambios de la resena con id {resenaExistente.Id}");
                return false;
            }

            return true;
        }

    }
}
