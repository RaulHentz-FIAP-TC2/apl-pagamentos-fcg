# syntax=docker/dockerfile:1
#
# Build context esperado: Apps/ (raiz da solution), conforme configurado em
# fcg-infra/docker-compose.yml (context: ../apl-pagamentos-fcg/Apps).

# Etapa 1 - build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /origem

COPY FcgPagamentos.sln ./
COPY src/FcgPagamentos.Domain/*.csproj ./src/FcgPagamentos.Domain/
COPY src/FcgPagamentos.Application/*.csproj ./src/FcgPagamentos.Application/
COPY src/FcgPagamentos.Infrastructure/*.csproj ./src/FcgPagamentos.Infrastructure/
COPY src/FcgPagamentos.Api/*.csproj ./src/FcgPagamentos.Api/
COPY tests/FcgPagamentos.UnitTests/*.csproj ./tests/FcgPagamentos.UnitTests/

RUN dotnet restore ./FcgPagamentos.sln

COPY . .

RUN dotnet publish ./src/FcgPagamentos.Api/FcgPagamentos.Api.csproj \
    -c Release \
    -o /app/publicado \
    --no-restore

# Etapa 2 - imagem final de execução
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publicado ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "FcgPagamentos.Api.dll"]
