using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//NOTA CLAVE: Asegurate de tener instalado el paquete Microsoft.EntityFrameworkCore en tu proyecto para poder usar las funcionalidades de Entity Framework Core.
//COMO ToListAsync, si te deja hacer ToList pero no ToListAsync, es porque te falta ese paquete.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalBackendAPIProgramacion2.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<UsuarioService> _logger;
        public UsuarioService(Final_Programacion_2Context context, ILogger<UsuarioService> logger)
        {
            _context = context;
            _logger = logger;
        }
        // Implementacion de los metodos de la interfaz IUsuarioService
        // en get no hace falta un try catch, solo al cambiar la base de datos.
        public async Task<IEnumerable<DTOUsuario?>> ObtenerTodos()
        {
            var usuarioAsync= await _context.Usuario.ToListAsync();
            
            List<DTOUsuario> lista = new List<DTOUsuario>();
            
            foreach (var usuario in usuarioAsync)
            {
                DTOUsuario user = new DTOUsuario
                {
                    Nombre = usuario.Nombre,
                    Contrasena = usuario.Contrasena,
                    Email = usuario.Email,
                    Rol = usuario.Rol
                };
                lista.Add(user);
            }

            return lista;
        }
        public async Task<DTOUsuario?> ObtenerPorId(int id)
        {
            var usuario = _context.Usuario.FindAsync(id);

            if (await usuario is null)
            {
                throw new KeyNotFoundException($"Usuario con id {id} no encontrado.");
            }

            DTOUsuario user = new DTOUsuario
            {
                Id = (await usuario).Id,
                Nombre = (await usuario).Nombre,
                Contrasena = (await usuario).Contrasena,
                Email = (await usuario).Email,
                Rol = (await usuario).Rol
            };

            return user;
        }
        public async Task<bool> Crear(DTOUsuario nuevoUsuario)
        {

            if (string.IsNullOrWhiteSpace(nuevoUsuario.Email) || string.IsNullOrWhiteSpace(nuevoUsuario.Contrasena) || string.IsNullOrWhiteSpace(nuevoUsuario.Rol) || string.IsNullOrWhiteSpace(nuevoUsuario.Nombre))
            {
                throw new ArgumentException("Todos los campos son obligatorios, rellene los campos e intente de nuevo.");
            }

            //debido a que en esta linea flataba el await, te daba un error silencioso en el swagger diciendote que estas haciendo mas de una llamada al dbcontext
            var usuario = await _context.Usuario.FirstOrDefaultAsync(e => e.Nombre == nuevoUsuario.Nombre);

            if (usuario is not null)
            {
                throw new ArgumentException("El nombre ya esta tomado");
            }

            Usuario usuarioParaCrear = new Usuario
            {
                Nombre = nuevoUsuario.Nombre,
                Contrasena = nuevoUsuario.Contrasena,
                Email = nuevoUsuario.Email,
                Rol = nuevoUsuario.Rol
            };

            bool ocupado = true;
            var random = new Random(); //esto es importante, el "new Random();" debe estar FUERA de la repeticion do{}while. y el random.Next debe estar DENTRO de la repeticion.

            do
            {
                int numero = random.Next();

                var user = _context.Usuario.Find(numero);

                if (user is null)
                {
                    ocupado = false;
                    usuarioParaCrear.Id = numero;
                }

            } while (ocupado == true);

            _context.Usuario.Add(usuarioParaCrear);

            try
            {
                //asi NO: await _context.SaveChanges();
                //asi SI: await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario {Nombre}", nuevoUsuario.Nombre);
                return false;
            }
            
            return true;
        }

        public async Task<bool> Editar(DTOUsuario usuarioActualizado)
        {
            var usuarioExistente = await _context.Usuario.FindAsync(usuarioActualizado.Id);
            if (usuarioExistente is null)
            {
                throw new ArgumentException("El usuario que intenta acceder no existe en la base de datos.");
            }
            
            if(string.IsNullOrWhiteSpace(usuarioActualizado.Email) || string.IsNullOrWhiteSpace(usuarioActualizado.Contrasena) || string.IsNullOrWhiteSpace(usuarioActualizado.Rol) || string.IsNullOrWhiteSpace(usuarioActualizado.Nombre))
            {
                throw new ArgumentException("Alguno de los campos esta vacio, rellene los campos e intente de nuevo.");
            }

            usuarioExistente.Email = usuarioActualizado.Email;
            usuarioExistente.Contrasena = usuarioActualizado.Contrasena;
            usuarioExistente.Rol = usuarioActualizado.Rol;
            usuarioExistente.Nombre = usuarioActualizado.Nombre;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios del usuario con Nombre {Nombre}", usuarioActualizado.Nombre);
                return false;
            }
            
            return true;
        }
        public async Task<bool> Eliminar(int id)
        {
            var usuarioExistente = await _context.Usuario.FindAsync(id);

            if (usuarioExistente is null)
            {
                throw new ArgumentException("No se encontro el usuario en la base de datos.");
            }

            _context.Usuario.Remove(usuarioExistente);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con Nombre {Nombre}", usuarioExistente.Nombre);
                return false;
            }
            
            return true;
        }
    }
}
