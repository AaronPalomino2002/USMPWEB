using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
    [Table("t_alumnos")]
    public class Alumnos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set;}
        public string? Nombres {get; set;}
        public string? Apellidos {get; set;}
        public string? DNI {get; set;}
        public string? Facultad {get; set;}
    }
}