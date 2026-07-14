# apl-pagamentos-fcg (PaymentsAPI)

Microsserviço responsável por processar (simular) o pagamento de uma compra de jogo
dentro da plataforma FIAP Cloud Games (FCG).

## Responsabilidade

Este serviço é, primariamente, um **consumidor de mensageria**. Ele não expõe
endpoints de negócio obrigatórios — sua função é:

1. Consumir o evento `PedidoRealizadoEvento`, publicado pelo `apl-catalogo-jogos-fcg`
   quando um usuário inicia o fluxo de compra de um jogo.
2. Simular o processamento do pagamento (regra: valor > 0 = Aprovado; caso
   contrário, Rejeitado).
3. Persistir o resultado na collection `pagamentos` (MongoDB, database
   `fcg-pagamentos-db`).
4. Publicar o evento `PagamentoProcessadoEvento`, consumido pelo
   `apl-catalogo-jogos-fcg` (para liberar o jogo na biblioteca) e pelo
   `apl-notificacoes-fcg` (para notificar o usuário).

Um endpoint opcional `GET /api/pagamentos/{pedidoId}` está disponível para
consulta pontual do resultado de um pagamento já processado.

## Arquitetura

Segue Clean Architecture / DDD tático, organizado em 4 camadas:

```
Apps/
├── FcgPagamentos.sln
├── src/
│   ├── FcgPagamentos.Api/             # Host HTTP, health checks, composition root
│   ├── FcgPagamentos.Application/     # Casos de uso (orquestração)
│   ├── FcgPagamentos.Domain/          # Entidades, enums e regras de negócio puras
│   └── FcgPagamentos.Infrastructure/  # MongoDB, MassTransit/RabbitMQ
└── tests/
    └── FcgPagamentos.UnitTests/       # Testes unitários de Domain e Application
```

- **Domain**: não possui nenhuma dependência de pacotes externos.
- **Application**: referencia apenas Domain.
- **Infrastructure**: referencia Domain e Application; implementa persistência
  (MongoDB.Driver) e mensageria (MassTransit + RabbitMQ).
- **Api**: referencia todas as camadas (composition root).

## Idempotência

O `PagamentoServico` verifica, antes de processar, se já existe um pagamento
registrado para o `PedidoId` recebido. Se existir, a mensagem é ignorada — isso
protege contra reprocessamento em caso de reentrega pelo RabbitMQ.

## Contratos de evento

### Consome: `PedidoRealizadoEvento`

Publicado pelo `apl-catalogo-jogos-fcg` no endpoint `fcg-pagamentos-pedido-realizado`.

### Publica: `PagamentoProcessadoEvento`

Publicado via `IPublishEndpoint` (fanout), consumido pelo CatalogAPI e pelo
NotificationsAPI.

## Como rodar localmente

Pré-requisitos: MongoDB e RabbitMQ acessíveis (ver `appsettings.Development.json`).

```bash
cd Apps
dotnet build FcgPagamentos.sln
dotnet run --project src/FcgPagamentos.Api
```

## Testes

```bash
cd Apps
dotnet test tests/FcgPagamentos.UnitTests/FcgPagamentos.UnitTests.csproj
```

## Docker

```bash
docker build -t fcg-payments-api:v1 -f Dockerfile .
```

## Kubernetes

Manifestos em `k8s/`: `deployment.yml`, `service.yml` (ClusterIP, service name
`payments-api`), `configmap.yml` e `secret.yml`.
