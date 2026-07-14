using FcgPagamentos.Application.Interfaces;
using FcgPagamentos.Application.Servicos;
using FcgPagamentos.Domain.Interfaces;
using FcgPagamentos.Domain.Servicos;
using FcgPagamentos.Infrastructure.Mensageria;
using FcgPagamentos.Infrastructure.Mensageria.Publicadores;
using FcgPagamentos.Infrastructure.Persistencia;
using FcgPagamentos.Infrastructure.Persistencia.Repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcgPagamentos.Infrastructure;

/// <summary>
/// Ponto único de registro de dependências da camada de Infrastructure,
/// mantendo o composition root (Api) simples e desacoplado dos detalhes internos.
/// </summary>
public static class RegistroServicosInfrastructure
{
    public static IServiceCollection AdicionarInfrastructure(this IServiceCollection servicos, IConfiguration configuracao)
    {
        servicos.Configure<ConfiguracaoMongoDb>(configuracao.GetSection("MongoDB"));
        servicos.AddSingleton<MongoDbContexto>();
        servicos.AddScoped<IPagamentoRepositorio, PagamentoRepositorio>();

        servicos.AddScoped<IPublicadorEventoPagamento, PagamentoEventoPublicador>();
        servicos.AddScoped<IPagamentoServico, PagamentoServico>();
        servicos.AddSingleton<ProcessadorPagamento>();

        servicos.AdicionarMassTransit(configuracao);

        return servicos;
    }
}
