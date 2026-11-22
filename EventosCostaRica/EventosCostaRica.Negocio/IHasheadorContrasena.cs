namespace EventosCostaRica.Negocio;

/// <summary>
/// Contrato para convertir contraseñas en hash y verificarlas.
/// </summary>
public interface IHasheadorContrasena
{
    string Hashear(string contrasena);

    bool Verificar(string contrasena, string hashGuardado);
}
