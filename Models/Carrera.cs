using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
     [Table("t_carrera")]
    public class Carrera
    {
        [Key]  
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdCarrera { get; set;}
        public string? nomCarrera {get; set;}
        public Facultad? Facultad { get; set; }

    }
}