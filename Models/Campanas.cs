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
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public long? CategoriaId { get; set; }
        public string? Imagen { get; set; }
        [Required(ErrorMessage = "Debe seleccionar entre 1 y 3 subcategorías")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos 1 subcategoría")]
        [MaxLength(3, ErrorMessage = "No puede seleccionar más de 3 subcategorías")]
        public List<long> SubCategoriaIds { get; set; } = new List<long>();
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        public virtual ICollection<SubCategoria> SubCategorias { get; set; } = new List<SubCategoria>();
    }

}