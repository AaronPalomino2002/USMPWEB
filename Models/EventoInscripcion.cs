using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using USMPWEB.Models;
[Table("t_evento_inscripcion")]
public class EventoInscripcion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    
    [Required]
    public long EventoId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string? NumeroRecibo { get; set; }
    
    [Required]
    public string? Nombres { get; set; }
    
    [Required]
    public string? Apellidos { get; set; }
    
    [Required]
    public string? Matricula { get; set; }
    
    [Required]
    public string? Facultad { get; set; }
    
    [Required]
    public string? Carrera { get; set; }
    
    [Required]
    public string? Direccion { get; set; }
    
    [Required]
    public string? Telefono { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }
    
    public DateTime FechaInscripcion { get; set; }
    public string? Estado { get; set; }
    public bool AceptoTerminos { get; set; }
    
    [ForeignKey("EventoId")]
    public virtual EventosInscripciones? Evento { get; set; }
}