using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
     [Table("t_inscripciones")]
    public class Inscripciones
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set;}
        public string? Alumno {get; set;}
        public string? Proceso {get; set;}
        public string? Culminado {get; set;}

    }
}