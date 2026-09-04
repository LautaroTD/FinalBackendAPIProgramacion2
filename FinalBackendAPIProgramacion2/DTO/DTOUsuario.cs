using FinalBackendAPIProgramacion2.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOUsuario
    { //solo el administrador puede editar usuarios, por ende, tiene acceso a todos los campos menos id porque es irrelevante
        //el ID es irrelevante, pues todos los nombres deben ser distintos antes de ser cargados a la base de datos.
        //DEBERIAN SERLO, POR LO MENOS.
        [Required]
        public int Id { get; set; } 
        [Required]
        [MaxLength(250)]
        public required string Nombre { get; set; }
        [Required]
        [MaxLength(300)]
        public required string Contrasena { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(500)]
        public required string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Rol { get; set; }
    }
}
