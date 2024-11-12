using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{

    [Table("t_eventosInscripciones")]
    public class EventosInscripciones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Required]
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Vacantes { get; set; }
        public string? Culminado { get; set; }
        [NotMapped]
        public bool CulminadoCheckbox
        {
            get => Culminado == "Si";
            set => Culminado = value ? "Si" : "No";
        }
        public long? CategoriaId { get; set; }
        public string? Imagen { get; set; }
        public long? subCategoriaId { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [ForeignKey("subCategoriaId")]
        public virtual SubCategoria? SubCategoria { get; set; }
    }
}