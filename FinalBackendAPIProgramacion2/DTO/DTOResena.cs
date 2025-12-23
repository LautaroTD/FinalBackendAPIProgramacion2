using FinalBackendAPIProgramacion2.Models;

namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOResena
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }

        public int IdArticulo { get; set; }

        public int Calificacion { get; set; }

        public string Descripcion { get; set; }
    }
}
