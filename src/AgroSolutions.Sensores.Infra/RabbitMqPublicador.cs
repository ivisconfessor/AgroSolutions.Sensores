using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AgroSolutions.Sensores.Infra;

public interface IRabbitMqPublicador
{
    void PublicarLeituraIngerida(object payload, CancellationToken ct = default);
}

/// <summary>
/// Publica mensagens no RabbitMQ para comunicação com outros microsserviços (ex.: Análise/Alertas).
/// </summary>
public class RabbitMqPublicador : IRabbitMqPublicador, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _filaLeituras;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public RabbitMqPublicador(string hostName, string? userName, string? password, string filaLeituras = "agrosolutions.sensores.leituras")
    {
        _filaLeituras = filaLeituras;
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = string.IsNullOrWhiteSpace(userName) ? "guest" : userName,
            Password = string.IsNullOrWhiteSpace(password) ? "guest" : password,
            Port = 5672
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _filaLeituras, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublicarLeituraIngerida(object payload, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var body = Encoding.UTF8.GetBytes(json);
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        props.ContentType = "application/json";
        _channel.BasicPublish(exchange: "", routingKey: _filaLeituras, basicProperties: props, body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
