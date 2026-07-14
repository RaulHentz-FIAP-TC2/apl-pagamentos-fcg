using FcgPagamentos.Domain.Entidades;
using FcgPagamentos.Domain.Interfaces;
using MongoDB.Driver;

namespace FcgPagamentos.Infrastructure.Persistencia.Repositorios;

/// <summary>
/// Implementação da persistência de <see cref="Pagamento"/> utilizando MongoDB.
/// Collection "pagamentos" no database "fcg-pagamentos-db".
/// </summary>
public class PagamentoRepositorio : IPagamentoRepositorio
{
    private const string NomeCollection = "pagamentos";

    private readonly IMongoCollection<Pagamento> _pagamentos;

    public PagamentoRepositorio(MongoDbContexto contexto)
    {
        _pagamentos = contexto.ObterCollection<Pagamento>(NomeCollection);
    }

    public Task AdicionarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
        => _pagamentos.InsertOneAsync(pagamento, options: null, cancellationToken);

    public async Task<Pagamento?> ObterPorPedidoIdAsync(Guid pedidoId, CancellationToken cancellationToken = default)
        => await _pagamentos.Find(p => p.PedidoId == pedidoId)
            .FirstOrDefaultAsync(cancellationToken);
}
