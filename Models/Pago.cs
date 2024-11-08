using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USMPWEB.Models
{
    [Table("t_pagos")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InscripcionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? NumeroRecibo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [MaxLength(3)]
        public string MonedaCodigo { get; set; } = "PEN";

        [Required]
        [MaxLength(20)]
        public string? Estado { get; set; }

        [Required]
        [MaxLength(20)]
        public string? MetodoPago { get; set; }

        [MaxLength(50)]
        public string? CodigoPago { get; set; }

        public DateTime? FechaExpiracion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaPago { get; set; }

        [ForeignKey("InscripcionId")]
        public virtual CampanaInscripcion? Inscripcion { get; set; }
    }
}