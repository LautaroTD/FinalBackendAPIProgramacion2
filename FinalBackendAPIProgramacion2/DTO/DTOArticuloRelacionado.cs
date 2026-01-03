using System.ComponentModel.DataAnnotations;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOArticuloRelacionado
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int IdPrimerArticulo { get; set; }
        [Required]
        public int IdSegundoArticulo { get; set; }
        [Required]
        public int IdPublicador { get; set; }
        [Required]
        public string NombrePublicador { get; set; }
        [Required]
        public string NombrePrimerArticulo { get; set; }
        [Required]
        public string NombreSegundoArticulo { get; set; }

    }
}
