using FcgPagamentos.Domain.Enums;

namespace FcgPagamentos.Domain.Entidades;

/// <summary>
/// Agregado raiz que representa o resultado do processamento (simulado) de um pagamento
/// referente a um pedido de compra de jogo.
/// </summary>
public class Pagamento
{
    public string? Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public string UsuarioId { get; private set; } = string.Empty;
    public string JogoId { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public StatusPagamento Status { get; private set; }
    public DateTime ProcessadoEm { get; private set; }

    // Construtor privado para uso do driver de persistência (deserialização).
    private Pagamento() { }

    private Pagamento(Guid pedidoId, string usuarioId, string jogoId, decimal valor, StatusPagamento status)
    {
        PedidoId = pedidoId;
        UsuarioId = usuarioId;
        JogoId = jogoId;
        Valor = valor;
        Status = status;
        ProcessadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria um novo registro de pagamento já processado, refletindo o resultado
    /// decidido pelo <see cref="Servicos.ProcessadorPagamento"/>.
    /// </summary>
    public static Pagamento CriarProcessado(Guid pedidoId, string usuarioId, string jogoId, decimal valor, StatusPagamento status)
        => new(pedidoId, usuarioId, jogoId, valor, status);
}
