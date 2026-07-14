using MassTransit;

namespace FcgPagamentos.Domain.Eventos;

[MessageUrn("fcg:PedidoRealizadoEvento")]
public record PedidoRealizadoEvento
{
    public Guid EventoId { get; init; }
    public DateTime OcorridoEm { get; init; }
    public Guid PedidoId { get; init; }
    public string UsuarioId { get; init; } = string.Empty;
    public string EmailUsuario { get; init; } = string.Empty;
    public string NomeUsuario { get; init; } = string.Empty;
    public string JogoId { get; init; } = string.Empty;
    public string NomeJogo { get; init; } = string.Empty;
    public decimal PrecoPago { get; init; }
}
