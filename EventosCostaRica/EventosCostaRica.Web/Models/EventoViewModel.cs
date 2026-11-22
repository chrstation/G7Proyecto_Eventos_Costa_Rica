using System.ComponentModel.DataAnnotations;
using EventosCostaRica.Entidades;
using EventosCostaRica.Web.Infraestructura;

namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos de un evento para las vistas.
/// </summary>
public class EventoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Fecha y hora")]
    [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
    public DateTime? FechaEvento { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(150, ErrorMessage = "El lugar no puede superar los 150 caracteres.")]
    public string Lugar { get; set; } = string.Empty;

    [Display(Name = "Banner (dirección de la imagen)")]
    [Url(ErrorMessage = "Escriba una dirección web válida, por ejemplo https://...")]
    [StringLength(500, ErrorMessage = "La dirección no puede superar los 500 caracteres.")]
    public string? BannerUrl { get; set; }

    public int EntradasVendidas { get; set; }

    public int AsientosDisponibles { get; set; }

    public bool Agotado { get; set; }

    public bool Finalizado { get; set; }

    public bool PuedeComprar => !Agotado && !Finalizado;

    public string FechaLarga => FechaEvento.HasValue ? FormatoFecha.Largo(FechaEvento.Value) : string.Empty;

    public string FechaCorta => FechaEvento.HasValue ? FormatoFecha.Corto(FechaEvento.Value) : string.Empty;

    public static EventoViewModel DesdeEntidad(Evento evento)
    {
        return new EventoViewModel
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Descripcion = evento.Descripcion,
            FechaEvento = evento.FechaEvento,
            Lugar = evento.Lugar,
            BannerUrl = evento.BannerUrl,
            EntradasVendidas = evento.EntradasVendidas,
            AsientosDisponibles = evento.AsientosDisponibles,
            Agotado = evento.Agotado,
            Finalizado = evento.Finalizado
        };
    }

    public Evento ComoEntidad()
    {
        return new Evento
        {
            Id = Id,
            Nombre = Nombre.Trim(),
            Descripcion = Descripcion?.Trim() ?? string.Empty,
            FechaEvento = FechaEvento.GetValueOrDefault(),
            Lugar = Lugar.Trim(),
            BannerUrl = BannerUrl?.Trim() ?? string.Empty
        };
    }
}
