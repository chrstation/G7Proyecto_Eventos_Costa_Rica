using EventosCostaRica.Entidades;
using EventosCostaRica.Negocio;

namespace EventosCostaRica.Web.Infraestructura;

/// <summary>
/// Carga datos de ejemplo la primera vez que se ejecuta la aplicación.
/// </summary>
public static class InicializadorBaseDatos
{
    public static async Task SembrarAsync(IServiceProvider servicios)
    {
        var usuarioServicio = servicios.GetRequiredService<IUsuarioServicio>();
        var eventoServicio = servicios.GetRequiredService<IEventoServicio>();

        if ((await usuarioServicio.ListarAsync()).Count == 0)
        {
            await usuarioServicio.RegistrarAsync(
                new Usuario { Nombre = "Administrador", Correo = "admin@eventoscr.com", Rol = RolUsuario.Administrador },
                "Admin123!");

            await usuarioServicio.RegistrarAsync(
                new Usuario { Nombre = "Cliente de Prueba", Correo = "cliente@eventoscr.com", Rol = RolUsuario.Cliente },
                "Cliente123!");
        }

        if ((await eventoServicio.ListarAsync()).Count == 0)
        {
            var hoy = DateTime.Today;

            await eventoServicio.RegistrarAsync(new Evento
            {
                Nombre = "Noche de Jazz",
                Descripcion = "Una velada con los mejores músicos de jazz del país.",
                FechaEvento = hoy.AddDays(15).AddHours(19),
                Lugar = "Teatro Nacional, San José",
                BannerUrl = "https://picsum.photos/seed/jazz/900/450"
            });

            await eventoServicio.RegistrarAsync(new Evento
            {
                Nombre = "Festival Gastronómico",
                Descripcion = "Degustación de comida típica costarricense con chefs invitados.",
                FechaEvento = hoy.AddDays(30).AddHours(12),
                Lugar = "Parque Central, Liberia",
                BannerUrl = "https://picsum.photos/seed/comida/900/450"
            });

            await eventoServicio.RegistrarAsync(new Evento
            {
                Nombre = "Concierto en la Playa",
                Descripcion = "Música en vivo frente al mar con bandas nacionales.",
                FechaEvento = hoy.AddDays(45).AddHours(18),
                Lugar = "Playa Hermosa, Jacó",
                BannerUrl = "https://picsum.photos/seed/playa/900/450"
            });
        }
    }
}
