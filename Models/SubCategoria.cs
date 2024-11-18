using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
    [Table("t_subCategorias")]
    public class SubCategoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdSubCategoria { get; set;}
        public string? nomSubCategoria{get; set;}
        public string? imgSubCategoria {get; set;}
        [JsonIgnore]
        public virtual ICollection<Campanas> Campanas { get; set; } = new List<Campanas>();
        public virtual ICollection<EventosInscripciones> EventosInscripciones { get; set; } = new List<EventosInscripciones>();
        public virtual ICollection<Certificados> Certificados { get; set; } = new List<Certificados>();
    }
}