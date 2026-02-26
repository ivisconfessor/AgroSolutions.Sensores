using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgroSolutions.Sensores.Infra;

/// <summary>
/// Documento MongoDB para leituras de sensores (collection leituras_sensores).
/// </summary>
internal class LeituraSensorDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("id_talhao")]
    [BsonRepresentation(BsonType.String)]
    public Guid IdTalhao { get; set; }

    [BsonElement("data_leitura")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime DataLeitura { get; set; }

    [BsonElement("umidade_solo")]
    public double UmidadeSolo { get; set; }

    [BsonElement("temperatura")]
    public double Temperatura { get; set; }

    [BsonElement("precipitacao")]
    public double Precipitacao { get; set; }
}
