using FcgPagamentos.Api.Extensoes;
using FcgPagamentos.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AdicionarInfrastructure(builder.Configuration);
builder.Services.AdicionarHealthChecks(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Esta API é um consumer puro de mensageria (fila fcg-pagamentos-pedido-realizado) — não expõe
// endpoints de negócio, apenas health checks para orquestração no Kubernetes. O endpoint de
// consulta pontual de pagamento foi removido por não ter autenticação/autorização; se essa
// consulta voltar a ser necessária, deve ser exposta via um serviço de Application dedicado
// (DTO de resposta) e protegida com RequireAuthorization(), nunca acessando o repositório
// de Domain diretamente a partir da Api.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();

// Necessário para que os testes de integração possam referenciar a classe Program (WebApplicationFactory).
public partial class Program { }
