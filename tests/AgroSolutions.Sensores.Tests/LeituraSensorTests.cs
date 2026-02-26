using Xunit;

namespace AgroSolutions.Sensores.Tests;

public class LeituraSensorTests
{
    [Fact]
    public void Dominio_LeituraSensor_Deve_Existir()
    {
        var leitura = new Dominio.LeituraSensor
        {
            Id = "1",
            IdTalhao = Guid.NewGuid(),
            DataLeitura = DateTimeOffset.UtcNow,
            UmidadeSolo = 65.5,
            Temperatura = 24.2,
            Precipitacao = 0
        };
        Assert.Equal(65.5, leitura.UmidadeSolo);
        Assert.Equal(24.2, leitura.Temperatura);
    }
}
