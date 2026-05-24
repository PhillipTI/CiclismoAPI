# Stage 1: Ambiente de compilação (+pesado)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

#Docker: Copia o .csproj ANTES do código fonte: o restore só roda novamente quando as dependências mudarem (não a cada mudança de código)
COPY ["CiclismoAPI.csproj", "."]
RUN dotnet restore

# Copia o restante do código e publica
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: O runtime é menor que o SDK (+ leve)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Segurança: Nunca rodar como root em produção
USER app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CiclismoAPI.dll"]