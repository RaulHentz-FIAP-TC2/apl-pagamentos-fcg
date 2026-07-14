using FcgPagamentos.Domain.Eventos;

namespace FcgPagamentos.Application.Interfaces;

/// <summary>
/// Abstrai a publicação do resultado do processamento de pagamento na mensageria,
/// permitindo que a Application não conheça detalhes de infraestrutura (RabbitMQ/MassTransit).
/// </summary>
public interface IPublicadorEventoPagamento
{
    Task PublicarPagamentoProcessadoAsync(PagamentoProcessadoEvento evento, CancellationToken cancellationToken = default);
}
