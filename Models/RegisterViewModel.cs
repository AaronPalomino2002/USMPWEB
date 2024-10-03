using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
     public class RegisterViewModel
    {
        [Required]
        public string? Nombre { get; set; }

        [Required]
        public string? Apellidos { get; set; }

        [Required]
        [EmailAddress]
        public string? Correo { get; set; }

        [Required]
        [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100 años")]
        public int Edad { get; set; }

        [Required]
        [Phone]
        public string? Celular { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
    }
}