using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOImagenEntrada
    {
        [Required]
        public string Ruta { get; set; } //Ruta, al crear el objeto, es la ruta donde ira la imagen. Ruta cuando se busca la imagen es la ruta + el nombre GUID de la imagen, que se puede ver en la base de datos para buscarlo.
        [Required]
        public int IdRelacionado { get; set; }
        [Required]
        public string TipoDeRelacion { get; set; }
        [Required]
        public int IdUsuario { get; set; }

        //visto en el Video
        [NotMapped]
        [Required]
        public IFormFile archivoDeImagen { get; set; }

    }
}
