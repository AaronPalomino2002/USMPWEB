using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace USMPWEB.Models
{
   
   public class RegisterViewModel
{
    [Key]
    [Required]
    public string? numMatricula { get; set; }

    [Required]
    public string? Nombre { get; set; }

    [Required]
    public string? apePat { get; set; }

    [Required]
    public string? apeMat { get; set; }

    [Required]
    public string? Correo { get; set; }

    [Required]
    public int Edad { get; set; }

    [Required]
    public string? Celular { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmPassword { get; set; }

    public int CarreraId { get; set; }
}

}