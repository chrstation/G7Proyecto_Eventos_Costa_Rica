using System.Security.Cryptography;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Hash de contraseñas con PBKDF2 (SHA-256), sal aleatoria y comparación en tiempo constante.
/// El formato guardado es: iteraciones.sal.hash (sal y hash en Base64).
/// </summary>
public class HasheadorContrasena : IHasheadorContrasena
{
    private const int TamanoSal = 16;
    private const int TamanoHash = 32;
    private const int Iteraciones = 100_000;

    public string Hashear(string contrasena)
    {
        var sal = RandomNumberGenerator.GetBytes(TamanoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

        return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string contrasena, string hashGuardado)
    {
        var partes = hashGuardado.Split('.');

        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
        {
            return false;
        }

        var sal = Convert.FromBase64String(partes[1]);
        var hashEsperado = Convert.FromBase64String(partes[2]);
        var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }
}
