using FcgPagamentos.Application.Interfaces;
using FcgPagamentos.Domain.Eventos;
using MassTransit;

namespace FcgPagamentos.Infrastructure.Mensageria.Publicadores;

/// <summary>
/// Publica o <see cref="PagamentoProcessadoEvento"/> no barramento (RabbitMQ) via MassTransit,
/// utilizando publish (fanout) para que múltiplos consumidores (CatalogAPI e NotificationsAPI)
/// possam reagir ao mesmo evento de forma independente.
/// </summary>
public class PagamentoEventoPublicador : IPublicadorEventoPagamento
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PagamentoEventoPublicador(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublicarPagamentoProcessadoAsync(PagamentoProcessadoEvento evento, CancellationToken cancellationToken = default)
        => _publishEndpoint.Publish(evento, cancellationToken);
}
