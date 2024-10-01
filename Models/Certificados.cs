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
        public DateOnly FechaExpedicion { get; set; }
    }
}