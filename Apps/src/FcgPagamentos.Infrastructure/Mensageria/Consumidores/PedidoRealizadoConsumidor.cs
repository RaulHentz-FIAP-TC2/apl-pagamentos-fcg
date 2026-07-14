using FcgPagamentos.Application.Interfaces;
using FcgPagamentos.Domain.Eventos;
using MassTransit;

namespace FcgPagamentos.Infrastructure.Mensageria.Consumidores;

/// <summary>
/// Consumidor responsável por receber o <see cref="PedidoRealizadoEvento"/> publicado
/// pelo CatalogAPI e disparar o processamento de pagamento.
/// </summary>
public class PedidoRealizadoConsumidor : IConsumer<PedidoRealizadoEvento>
{
    private readonly IPagamentoServico _pagamentoServico;

    public PedidoRealizadoConsumidor(IPagamentoServico pagamentoServico)
    {
        _pagamentoServico = pagamentoServico;
    }

    public Task Consume(ConsumeContext<PedidoRealizadoEvento> context)
        => _pagamentoServico.ProcessarPagamentoAsync(context.Message, context.CancellationToken);
}
