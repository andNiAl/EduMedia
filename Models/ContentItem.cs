namespace EduMedia.Web.Models
{
    public class ContentItem
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        public string Descripcion { get; set; } = "Sin descripción";
    }
}
