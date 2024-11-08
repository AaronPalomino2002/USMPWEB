namespace USMPWEB.Models;
public class PagoTarjetaViewModel
{
    public decimal Monto { get; set; }
    public string? NombreCliente { get; set; }
    public string? Email { get; set; }
    public string? NumeroTarjeta { get; set; }
    public string? FechaExpiracion { get; set; }
    public string? CVV { get; set; }
     public string? NumeroRecibo { get; set; }  // Agregar esta propiedad
}