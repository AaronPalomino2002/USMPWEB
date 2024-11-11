using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{

    [Table("t_campanas")]
    public class Campanas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Los requisitos son requeridos")]
        public string? Requisitos { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public long? CategoriaId { get; set; }

        [Required(ErrorMessage = "La imagen es requerida")]
        public string? Imagen { get; set; }

        [Required(ErrorMessage = "Debe seleccionar entre 1 y 3 subcategorías")]
        [NotMapped]
        public List<long> SubCategoriaIds { get; set; } = new List<long>();

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateOnly FechaFin { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        public virtual ICollection<SubCategoria> SubCategorias { get; set; } = new List<SubCategoria>();
    }

}