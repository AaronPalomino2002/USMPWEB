using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
    [Table("t_certificados")]
    public class Certificados
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public string? NombreCertificado { get; set; }
        public string? Descripcion { get; set; }
        public long? CategoriaId { get; set; }
        public string? Imagen { get; set; }
        public long? subCategoriaId { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [ForeignKey("subCategoriaId")]
        public virtual SubCategoria? SubCategoria { get; set; }
        public DateOnly FechaExpedicion { get; set; }
    }
    
}