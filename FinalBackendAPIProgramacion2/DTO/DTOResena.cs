using FinalBackendAPIProgramacion2.Models;
using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOResena
    {
        [Required]
        public int Id { get; set; } //deberia mostrar la id de la resena.
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public int IdArticulo { get; set; }
        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; }
        [MaxLength(1000)]
        public string Descripcion { get; set; }
        [Required]
        public string NombrePublicador { get; set; }
        [Required]
        public string NombreProducto { get; set; }
    }
}
