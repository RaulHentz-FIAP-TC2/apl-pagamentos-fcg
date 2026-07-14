using FcgPagamentos.Infrastructure.Mensageria;
using FcgPagamentos.Infrastructure.Persistencia;

namespace FcgPagamentos.Api.Extensoes;

/// <summary>
/// Registra os health checks de dependências externas (MongoDB e RabbitMQ),
/// usados pelos endpoints /health/ready e /health/live e pelas probes do Kubernetes.
/// </summary>
public static class RegistroServicosExtensao
{
    public static IServiceCollection AdicionarHealthChecks(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var mongoConfig = configuracao.GetSection("MongoDB").Get<ConfiguracaoMongoDb>() ?? new ConfiguracaoMongoDb();
        var rabbitConfig = configuracao.GetSection("RabbitMQ").Get<ConfiguracaoRabbitMq>() ?? new ConfiguracaoRabbitMq();

        var rabbitConnectionUri = $"amqp://{rabbitConfig.Username}:{rabbitConfig.Password}@{rabbitConfig.Host}:{rabbitConfig.Port}";

        servicos.AddHealthChecks()
            .AddMongoDb(mongoConfig.ConnectionString, name: "mongodb")
            .AddRabbitMQ(rabbitConnectionUri, name: "rabbitmq");

        return servicos;
    }
}
