using System;
using System.ComponentModel.DataAnnotations;

namespace USMPWEB.Models
{
    public class PagoEfectivoViewModel
    {
        [Required]
        public string? NumeroRecibo { get; set; }

        [Required]
        [Display(Name = "Monto a Pagar")]
        public decimal Monto { get; set; }

        [Required]
        [Display(Name = "Fecha de Vencimiento")]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        [Display(Name = "Nombre del Estudiante")]
        public string? NombreEstudiante { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        public string? Estado { get; set; }
    }
}