using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USMPWEB.Models
{
    [Table("t_campana_inscripciones")]
    public class CampanaInscripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string NumeroRecibo { get; set; } = ""; // Agregar esta propiedad

        [Required]
        public int CampanaId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Nombres { get; set; }

        [Required]
        [StringLength(50)]
        public string? Apellidos { get; set; }

        [Required]
        [StringLength(20)]
        public string? Matricula { get; set; }

        [Required]
        public string? Facultad { get; set; }

        [Required]
        public string? Carrera { get; set; }

        [Required]
        public string? Direccion { get; set; }

        [Required]
        [Phone]
        public string? Telefono { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime FechaInscripcion { get; set; }

        public bool AceptoTerminos { get; set; }

        public string? Estado { get; set; }

        [ForeignKey("CampanaId")]
        public virtual Campanas? Campana { get; set; }
         // Relación con los pagos
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}