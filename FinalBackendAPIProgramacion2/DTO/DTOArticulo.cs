using System.ComponentModel.DataAnnotations;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOArticulo
    {
        [Required]
        public int Id { get; set; } 
        [Required]
        public string NombrePublicador { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Url { get; set; }
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")] 
        public decimal Precio { get; set; }
        [MaxLength(1000)]
        public string Descripcion { get; set; }
        [Required]
        [MaxLength(300)]
        public string NombreProducto { get; set; }
    }
}
