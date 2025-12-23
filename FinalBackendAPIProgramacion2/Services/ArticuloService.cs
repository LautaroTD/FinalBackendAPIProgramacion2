using FinalBackendAPIProgramacion2.DTO;
using FinalBackendAPIProgramacion2.Interfaces;
using FinalBackendAPIProgramacion2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalBackendAPIProgramacion2.Services
{
    public class ArticuloService : IArticuloService
    {
        private readonly Final_Programacion_2Context _context;
        private readonly ILogger<ArticuloService> _logger;

        public ArticuloService(Final_Programacion_2Context context, ILogger<ArticuloService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DTOArticulo?>> ObtenerTodos()
        {
            var articuloAsync = await _context.Articulo.ToListAsync();
            var usuarioAsync = await _context.Usuario.ToListAsync();

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

            List<DTOArticulo> listaArticulo = new List<DTOArticulo>();

            foreach (var articulo in articuloAsync)
            {
                var publicador = listaUsuarioReducida.Find(e=> e.Id == articulo.Id);

                if(publicador is null)
                {
                    publicador = new DTOUsuario
                    {
                        Nombre = "Usuario no encontrado"
                    };
                }

                DTOArticulo articuloTemp = new DTOArticulo
                {
                    Precio = articulo.Precio,
                    Url = articulo.Url,
                    Descripcion = articulo.Descripcion,
                    NombrePublicador = publicador.Nombre
                };
                listaArticulo.Add(articuloTemp);
            }

            return listaArticulo;
        }

        public async Task<DTOArticulo?> ObtenerPorId(int id)
        {
            var articulo = _context.Articulo.FindAsync(id);

            if (await articulo is null)
            {
                throw new KeyNotFoundException($"Articulo con id {id} no encontrado.");
            }

            int IdTemp = (await articulo).IdUsuario;

            var usuario = _context.Usuario.FindAsync(IdTemp);

            string nombrePublicador = (await usuario)?.Nombre ?? "Usuario no encontrado";

            DTOArticulo user = new DTOArticulo
            {
                Id = (await articulo).Id,
                Url = (await articulo).Url,
                Precio = (await articulo).Precio,
                Descripcion = (await articulo).Descripcion,
                NombreProducto = (await articulo).Nombre,
                NombrePublicador = nombrePublicador
            };

            return user;
        }

        public async Task<bool> Crear(DTOArticulo nuevoArticulo)
        {

            if (string.IsNullOrWhiteSpace(nuevoArticulo.NombrePublicador) || string.IsNullOrWhiteSpace(nuevoArticulo.Url) || string.IsNullOrWhiteSpace(nuevoArticulo.NombreProducto) || nuevoArticulo.Precio <= 0m) //se usa 0m para definir que ese 0 es decimal, como la variable precio.
            {
                throw new ArgumentException("No se rellenaron los campos del Articulo correctamente, intente de nuevo.");
            }

            //debido a que en esta linea faltaba el await, te daba un error silencioso en el swagger diciendote que estas haciendo mas de una llamada al dbcontext
            var usuario = await _context.Usuario.FirstOrDefaultAsync(e => e.Nombre == nuevoArticulo.NombrePublicador);

            if (usuario is null)
            {
                throw new ArgumentException("Ingrese un nombre de usuario que exista en el sistema");
            }

            var idDeVendedor = usuario.Id;

            Articulo articuloACrear = new Articulo
            {
                IdUsuario = idDeVendedor,
                Url = nuevoArticulo.Url,
                Precio = nuevoArticulo.Precio,
                Descripcion = nuevoArticulo.Descripcion,
                Nombre = nuevoArticulo.NombreProducto
            };

            bool ocupado = true;
            var random = new Random(); //esto es importante, el "new Random();" debe estar FUERA de la repeticion do{}while. y el random.Next debe estar DENTRO de la repeticion.

            do
            {
                int numero = random.Next();

                var articulo = _context.Articulo.Find(numero);

                if (articulo is null)
                {
                    ocupado = false;
                    articuloACrear.Id = numero;
                }

            } while (ocupado == true);

            _context.Articulo.Add(articuloACrear);

            try
            {
                //asi NO: await _context.SaveChanges();
                //asi SI: await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el articulo {Nombre}", nuevoArticulo.NombreProducto);
                return false;
            }

            return true;
        }


        public async Task<bool> Editar(DTOArticulo articuloActualizado)
        {
            var articuloExistente = await _context.Articulo.FindAsync(articuloActualizado.Id);
            
            if (articuloExistente is null)
            {
                throw new ArgumentException("No se encontro el articulo que quiere editar, intente de nuevo.");
            }

            if (string.IsNullOrWhiteSpace(articuloActualizado.NombrePublicador) || string.IsNullOrWhiteSpace(articuloActualizado.Url) || string.IsNullOrWhiteSpace(articuloActualizado.Descripcion) || string.IsNullOrWhiteSpace(articuloActualizado.NombreProducto))
            {
                throw new ArgumentException("Todos los campos son obligatorios, rellene los campos e intentelo de nuevo.");
            }

            var usuario = await _context.Usuario.FirstOrDefaultAsync(e => e.Nombre == articuloActualizado.NombrePublicador);

            if (usuario is null)
            {
                throw new ArgumentException("El nombre del publicador no existe, ingrese un nombre de usuario que exista en el sistema e intente de nuevo.");
            }

            articuloExistente.Url = articuloActualizado.Url;
            articuloExistente.Precio = articuloActualizado.Precio;
            articuloExistente.Descripcion = articuloActualizado.Descripcion;
            articuloExistente.Nombre = articuloActualizado.NombreProducto;
            articuloExistente.IdUsuario = usuario.Id;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los cambios del articulo con Nombre {Nombre}", articuloActualizado.NombreProducto);
                return false;
            }

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var articuloExistente = await _context.Articulo.FindAsync(id);

            if (articuloExistente is null)
            {
                throw new ArgumentException("No se encontro el articulo en la base de datos.");
            }

            _context.Articulo.Remove(articuloExistente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con Nombre {Nombre}", articuloExistente.Nombre);
                return false;
            }

            return true;
        }
    }
}
