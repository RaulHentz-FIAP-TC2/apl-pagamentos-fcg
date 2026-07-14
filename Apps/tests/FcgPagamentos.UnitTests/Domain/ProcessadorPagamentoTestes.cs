using FcgPagamentos.Domain.Enums;
using FcgPagamentos.Domain.Servicos;
using FluentAssertions;
using Xunit;

namespace FcgPagamentos.UnitTests.Domain;

public class ProcessadorPagamentoTestes
{
    private readonly ProcessadorPagamento _processadorPagamento = new();

    [Theory]
    [InlineData(0.01)]
    [InlineData(59.90)]
    [InlineData(999.99)]
    public void Processar_DeveAprovar_QuandoValorForMaiorQueZero(decimal valor)
    {
        var status = _processadorPagamento.Processar(valor);

        status.Should().Be(StatusPagamento.Aprovado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Processar_DeveRejeitar_QuandoValorForZeroOuNegativo(decimal valor)
    {
        var status = _processadorPagamento.Processar(valor);

        status.Should().Be(StatusPagamento.Rejeitado);
    }
}
