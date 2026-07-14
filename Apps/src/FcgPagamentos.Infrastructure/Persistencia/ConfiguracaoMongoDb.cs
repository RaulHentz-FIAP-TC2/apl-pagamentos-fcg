namespace FcgPagamentos.Infrastructure.Persistencia;

/// <summary>
/// POCO com as configurações de conexão ao MongoDB, ligado à seção "MongoDB" do appsettings.
/// </summary>
public class ConfiguracaoMongoDb
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
