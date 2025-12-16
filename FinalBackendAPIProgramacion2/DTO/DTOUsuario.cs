using FinalBackendAPIProgramacion2.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOUsuario
    { //solo el administrador puede editar usuarios, por ende, tiene acceso a todos los campos menos id porque es irrelevante
        //el ID es irrelevante, pues todos los nombres deben ser distintos antes de ser cargados a la base de datos.
        //DEBERIAN SERLO, POR LO MENOS.
        public string Nombre { get; set; } //usa el nombre para localizarlo en la base de datos.
        public string Contrasena { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}
