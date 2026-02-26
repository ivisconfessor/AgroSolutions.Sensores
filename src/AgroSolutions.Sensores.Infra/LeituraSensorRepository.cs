using AgroSolutions.Sensores.Dominio;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AgroSolutions.Sensores.Infra;

public interface ILeituraSensorRepository
{
    Task<string> InserirAsync(LeituraSensor leitura, CancellationToken ct = default);
    Task<LeituraSensor?> ObterPorIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<LeituraSensor>> ListarPorTalhaoAsync(Guid idTalhao, DateTimeOffset? de = null, DateTimeOffset? ate = null, int limite = 100, CancellationToken ct = default);
}

public class LeituraSensorRepository : ILeituraSensorRepository
{
    private readonly IMongoCollection<LeituraSensorDocument> _collection;

    public LeituraSensorRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<LeituraSensorDocument>("leituras_sensores");
    }

    public async Task<string> InserirAsync(LeituraSensor leitura, CancellationToken ct = default)
    {
        var doc = ParaDocument(leitura);
        doc.Id = ObjectId.GenerateNewId().ToString();
        await _collection.InsertOneAsync(doc, cancellationToken: ct);
        return doc.Id;
    }

    public async Task<LeituraSensor?> ObterPorIdAsync(string id, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync(ct);
        return doc is null ? null : ParaDominio(doc);
    }

    public async Task<IReadOnlyList<LeituraSensor>> ListarPorTalhaoAsync(Guid idTalhao, DateTimeOffset? de = null, DateTimeOffset? ate = null, int limite = 100, CancellationToken ct = default)
    {
        var filter = Builders<LeituraSensorDocument>.Filter.Eq(d => d.IdTalhao, idTalhao);
        if (de.HasValue)
            filter &= Builders<LeituraSensorDocument>.Filter.Gte(d => d.DataLeitura, de.Value.UtcDateTime);
        if (ate.HasValue)
            filter &= Builders<LeituraSensorDocument>.Filter.Lte(d => d.DataLeitura, ate.Value.UtcDateTime);

        var docs = await _collection
            .Find(filter)
            .SortByDescending(d => d.DataLeitura)
            .Limit(limite)
            .ToListAsync(ct);
        return docs.Select(ParaDominio).ToList();
    }

    private static LeituraSensorDocument ParaDocument(LeituraSensor leitura)
    {
        return new LeituraSensorDocument
        {
            Id = leitura.Id,
            IdTalhao = leitura.IdTalhao,
            DataLeitura = leitura.DataLeitura.UtcDateTime,
            UmidadeSolo = leitura.UmidadeSolo,
            Temperatura = leitura.Temperatura,
            Precipitacao = leitura.Precipitacao
        };
    }

    private static LeituraSensor ParaDominio(LeituraSensorDocument doc)
    {
        return new LeituraSensor
        {
            Id = doc.Id,
            IdTalhao = doc.IdTalhao,
            DataLeitura = new DateTimeOffset(doc.DataLeitura, TimeSpan.Zero),
            UmidadeSolo = doc.UmidadeSolo,
            Temperatura = doc.Temperatura,
            Precipitacao = doc.Precipitacao
        };
    }
}
