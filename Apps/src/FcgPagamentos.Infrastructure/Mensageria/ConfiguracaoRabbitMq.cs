namespace FcgPagamentos.Infrastructure.Mensageria;

/// <summary>
/// POCO com as configurações de conexão ao RabbitMQ, ligado à seção "RabbitMQ" do appsettings.
/// </summary>
public class ConfiguracaoRabbitMq
{
    public string Host { get; set; } = string.Empty;
    public ushort Port { get; set; } = 5672;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
