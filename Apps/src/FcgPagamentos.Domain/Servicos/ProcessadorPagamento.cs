using FcgPagamentos.Domain.Enums;

namespace FcgPagamentos.Domain.Servicos;

/// <summary>
/// Serviço de domínio responsável por simular a decisão de aprovação de um pagamento.
/// Não realiza nenhuma operação de I/O, é puro e determinístico — o que o torna
/// facilmente testável.
/// </summary>
public class ProcessadorPagamento
{
    /// <summary>
    /// Regra de simulação: pagamentos com valor maior que zero são aprovados,
    /// qualquer outro valor (zero ou negativo) é rejeitado.
    /// </summary>
    public StatusPagamento Processar(decimal valor)
        => valor > 0 ? StatusPagamento.Aprovado : StatusPagamento.Rejeitado;
}
