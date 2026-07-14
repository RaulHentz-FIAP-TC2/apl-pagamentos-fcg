using FcgPagamentos.Domain.Entidades;

namespace FcgPagamentos.Domain.Interfaces;

/// <summary>
/// Contrato de persistência para a entidade <see cref="Pagamento"/>.
/// </summary>
public interface IPagamentoRepositorio
{
    Task AdicionarAsync(Pagamento pagamento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um pagamento já processado pelo Id do pedido de origem.
    /// Utilizado para garantir idempotência no processamento.
    /// </summary>
    Task<Pagamento?> ObterPorPedidoIdAsync(Guid pedidoId, CancellationToken cancellationToken = default);
}
