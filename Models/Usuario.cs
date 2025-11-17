using System;
using System.ComponentModel.DataAnnotations;

namespace EduMedia.Web.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasena { get; set; }

        [StringLength(50)]
        public string Rol { get; set; } = "Usuario";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
