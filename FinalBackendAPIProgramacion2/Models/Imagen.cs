using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalBackendAPIProgramacion2.Models
{
    public class Imagen
    {
        //Nota Importante: esta tabla se encuentra AISLADA en la base de datos porque su forma de relacionarse no usa Foreign Key y lo vuelve intolerable para la base de datos
        [Required]
        public int Id { get; set; }
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
        public IFormFile? archivoDeImagen{ get; set; }

    }
}
