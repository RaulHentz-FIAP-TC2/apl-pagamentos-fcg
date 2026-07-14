using FcgPagamentos.Domain.Eventos;
using FcgPagamentos.Infrastructure.Mensageria.Consumidores;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FcgPagamentos.Infrastructure.Mensageria;

/// <summary>
/// Extensão responsável por registrar o MassTransit com transporte RabbitMQ,
/// configurando o consumidor de pedidos realizados e a política de retry.
/// </summary>
public static class ConfiguracaoMassTransit
{
    private const string NomeFilaPedidoRealizado = "fcg-pagamentos-pedido-realizado";

    // Nomes canônicos de exchange, compartilhados entre todos os microsserviços que
    // publicam/consomem estes eventos. Sem isso, o MassTransit usaria o namespace CLR
    // completo (ex.: "FcgPagamentos.Domain.Eventos:PedidoRealizadoEvento"), que difere
    // entre produtor e consumidor e impede o roteamento das mensagens.
    private const string EntidadePedidoRealizado = "order-placed-event";
    private const string EntidadePagamentoProcessado = "payment-processed-event";

    public static IServiceCollection AdicionarMassTransit(this IServiceCollection servicos, IConfiguration configuracao)
    {
        var rabbitMq = configuracao.GetSection("RabbitMQ").Get<ConfiguracaoRabbitMq>() ?? new ConfiguracaoRabbitMq();

        servicos.AddMassTransit(configuradorBus =>
        {
            configuradorBus.AddConsumer<PedidoRealizadoConsumidor>();

            configuradorBus.UsingRabbitMq((contexto, configuradorRabbit) =>
            {
                configuradorRabbit.Message<PedidoRealizadoEvento>(x => x.SetEntityName(EntidadePedidoRealizado));
                configuradorRabbit.Message<PagamentoProcessadoEvento>(x => x.SetEntityName(EntidadePagamentoProcessado));

                configuradorRabbit.Host(rabbitMq.Host, rabbitMq.Port, "/", h =>
                {
                    h.Username(rabbitMq.Username);
                    h.Password(rabbitMq.Password);
                });

                configuradorRabbit.ReceiveEndpoint(NomeFilaPedidoRealizado, endpoint =>
                {
                    endpoint.ConfigureConsumer<PedidoRealizadoConsumidor>(contexto);
                    endpoint.UseMessageRetry(retry => retry.Intervals(500, 1000, 2000, 5000));
                });
            });
        });

        return servicos;
    }
}
