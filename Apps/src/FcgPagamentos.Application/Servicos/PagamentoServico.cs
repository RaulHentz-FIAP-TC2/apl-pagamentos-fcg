using FcgPagamentos.Application.Interfaces;
using FcgPagamentos.Domain.Entidades;
using FcgPagamentos.Domain.Enums;
using FcgPagamentos.Domain.Eventos;
using FcgPagamentos.Domain.Interfaces;
using FcgPagamentos.Domain.Servicos;
using Microsoft.Extensions.Logging;

namespace FcgPagamentos.Application.Servicos;

/// <summary>
/// Orquestra o caso de uso de processamento de pagamento: recebe o pedido realizado,
/// aplica a regra de simulação do domínio, persiste o resultado e publica o evento
/// de pagamento processado para os demais microsserviços.
/// </summary>
public class PagamentoServico : IPagamentoServico
{
    private readonly IPagamentoRepositorio _pagamentoRepositorio;
    private readonly IPublicadorEventoPagamento _publicadorEventoPagamento;
    private readonly ProcessadorPagamento _processadorPagamento;
    private readonly ILogger<PagamentoServico> _logger;

    private const string TipoNotificacaoConfirmacaoCompra = "ConfirmacaoCompra";

    public PagamentoServico(
        IPagamentoRepositorio pagamentoRepositorio,
        IPublicadorEventoPagamento publicadorEventoPagamento,
        ProcessadorPagamento processadorPagamento,
        ILogger<PagamentoServico> logger)
    {
        _pagamentoRepositorio = pagamentoRepositorio;
        _publicadorEventoPagamento = publicadorEventoPagamento;
        _processadorPagamento = processadorPagamento;
        _logger = logger;
    }

    public async Task ProcessarPagamentoAsync(PedidoRealizadoEvento pedidoRealizado, CancellationToken cancellationToken = default)
    {
        // Idempotência: se o pedido já foi processado anteriormente, ignora a mensagem.
        var pagamentoExistente = await _pagamentoRepositorio.ObterPorPedidoIdAsync(pedidoRealizado.PedidoId, cancellationToken);
        if (pagamentoExistente is not null)
        {
            _logger.LogInformation("Pedido {PedidoId} já foi processado anteriormente. Mensagem ignorada.", pedidoRealizado.PedidoId);
            return;
        }

        var status = _processadorPagamento.Processar(pedidoRealizado.PrecoPago);

        var pagamento = Pagamento.CriarProcessado(
            pedidoRealizado.PedidoId,
            pedidoRealizado.UsuarioId,
            pedidoRealizado.JogoId,
            pedidoRealizado.PrecoPago,
            status);

        await _pagamentoRepositorio.AdicionarAsync(pagamento, cancellationToken);

        var pagamentoProcessadoEvento = new PagamentoProcessadoEvento
        {
            EventoId = Guid.NewGuid(),
            OcorridoEm = DateTime.UtcNow,
            PedidoId = pedidoRealizado.PedidoId,
            UsuarioId = pedidoRealizado.UsuarioId,
            EmailUsuario = pedidoRealizado.EmailUsuario,
            NomeUsuario = pedidoRealizado.NomeUsuario,
            JogoId = pedidoRealizado.JogoId,
            NomeJogo = pedidoRealizado.NomeJogo,
            PrecoPago = pedidoRealizado.PrecoPago,
            Status = status == StatusPagamento.Aprovado ? "Approved" : "Rejected",
            TipoNotificacao = TipoNotificacaoConfirmacaoCompra
        };

        await _publicadorEventoPagamento.PublicarPagamentoProcessadoAsync(pagamentoProcessadoEvento, cancellationToken);

        _logger.LogInformation("Pagamento do pedido {PedidoId} processado com status {Status}.", pedidoRealizado.PedidoId, pagamentoProcessadoEvento.Status);
    }
}
