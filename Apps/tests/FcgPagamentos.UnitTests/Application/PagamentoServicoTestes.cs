using FcgPagamentos.Application.Interfaces;
using FcgPagamentos.Application.Servicos;
using FcgPagamentos.Domain.Entidades;
using FcgPagamentos.Domain.Enums;
using FcgPagamentos.Domain.Eventos;
using FcgPagamentos.Domain.Interfaces;
using FcgPagamentos.Domain.Servicos;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FcgPagamentos.UnitTests.Application;

public class PagamentoServicoTestes
{
    private readonly Mock<IPagamentoRepositorio> _pagamentoRepositorioMock = new();
    private readonly Mock<IPublicadorEventoPagamento> _publicadorEventoPagamentoMock = new();
    private readonly PagamentoServico _pagamentoServico;

    public PagamentoServicoTestes()
    {
        _pagamentoServico = new PagamentoServico(
            _pagamentoRepositorioMock.Object,
            _publicadorEventoPagamentoMock.Object,
            new ProcessadorPagamento(),
            new LoggerFactory().CreateLogger<PagamentoServico>());
    }

    private static PedidoRealizadoEvento CriarPedidoRealizado(decimal precoPago) => new()
    {
        EventoId = Guid.NewGuid(),
        OcorridoEm = DateTime.UtcNow,
        PedidoId = Guid.NewGuid(),
        UsuarioId = "usuario-1",
        EmailUsuario = "usuario@fcg.com",
        NomeUsuario = "Usuario Teste",
        JogoId = "jogo-1",
        NomeJogo = "Jogo Teste",
        PrecoPago = precoPago
    };

    [Fact]
    public async Task ProcessarPagamentoAsync_DevePersistirEPublicar_QuandoPedidoNaoFoiProcessado()
    {
        var pedidoRealizado = CriarPedidoRealizado(59.90m);

        _pagamentoRepositorioMock
            .Setup(r => r.ObterPorPedidoIdAsync(pedidoRealizado.PedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pagamento?)null);

        await _pagamentoServico.ProcessarPagamentoAsync(pedidoRealizado);

        _pagamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()), Times.Once);

        _publicadorEventoPagamentoMock.Verify(p => p.PublicarPagamentoProcessadoAsync(
            It.Is<PagamentoProcessadoEvento>(e =>
                e.PedidoId == pedidoRealizado.PedidoId &&
                e.Status == "Approved" &&
                e.TipoNotificacao == "ConfirmacaoCompra"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessarPagamentoAsync_DevePublicarRejeitado_QuandoPrecoForZeroOuMenor()
    {
        var pedidoRealizado = CriarPedidoRealizado(0m);

        _pagamentoRepositorioMock
            .Setup(r => r.ObterPorPedidoIdAsync(pedidoRealizado.PedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pagamento?)null);

        await _pagamentoServico.ProcessarPagamentoAsync(pedidoRealizado);

        _publicadorEventoPagamentoMock.Verify(p => p.PublicarPagamentoProcessadoAsync(
            It.Is<PagamentoProcessadoEvento>(e => e.Status == "Rejected"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessarPagamentoAsync_DeveIgnorarMensagem_QuandoPedidoJaFoiProcessado()
    {
        var pedidoRealizado = CriarPedidoRealizado(59.90m);
        var pagamentoJaProcessado = Pagamento.CriarProcessado(
            pedidoRealizado.PedidoId, pedidoRealizado.UsuarioId, pedidoRealizado.JogoId, pedidoRealizado.PrecoPago, StatusPagamento.Aprovado);

        _pagamentoRepositorioMock
            .Setup(r => r.ObterPorPedidoIdAsync(pedidoRealizado.PedidoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentoJaProcessado);

        await _pagamentoServico.ProcessarPagamentoAsync(pedidoRealizado);

        _pagamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()), Times.Never);
        _publicadorEventoPagamentoMock.Verify(p => p.PublicarPagamentoProcessadoAsync(It.IsAny<PagamentoProcessadoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
