namespace AgroSolutions.Sensores.Dominio;

/// <summary>
/// Representa uma leitura de sensores de campo para um talhão (umidade, temperatura, precipitação).
/// </summary>
public class LeituraSensor
{
    public string Id { get; set; } = null!;
    public Guid IdTalhao { get; set; }
    public DateTimeOffset DataLeitura { get; set; }
    /// <summary>Umidade do solo em porcentagem (0-100).</summary>
    public double UmidadeSolo { get; set; }
    /// <summary>Temperatura em graus Celsius.</summary>
    public double Temperatura { get; set; }
    /// <summary>Nível de precipitação em mm.</summary>
    public double Precipitacao { get; set; }
}
