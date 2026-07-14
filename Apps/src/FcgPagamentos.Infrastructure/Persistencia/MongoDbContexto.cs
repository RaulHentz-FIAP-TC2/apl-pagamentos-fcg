using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FcgPagamentos.Infrastructure.Persistencia;

/// <summary>
/// Ponto único de acesso ao banco MongoDB do microsserviço de pagamentos.
/// Encapsula a criação do client e a obtenção das collections utilizadas.
/// </summary>
public class MongoDbContexto
{
    private readonly IMongoDatabase _database;

    public MongoDbContexto(IOptions<ConfiguracaoMongoDb> configuracao)
    {
        var settings = configuracao.Value;
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<TDocumento> ObterCollection<TDocumento>(string nome)
        => _database.GetCollection<TDocumento>(nome);
}
