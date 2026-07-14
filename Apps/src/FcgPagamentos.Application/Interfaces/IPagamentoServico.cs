using FcgPagamentos.Domain.Eventos;

namespace FcgPagamentos.Application.Interfaces;

/// <summary>
/// Caso de uso responsável por orquestrar o processamento de um pagamento
/// a partir de um pedido de compra recebido via mensageria.
/// </summary>
public interface IPagamentoServico
{
    Task ProcessarPagamentoAsync(PedidoRealizadoEvento pedidoRealizado, CancellationToken cancellationToken = default);
}
