# Étape 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-jammy AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Étape 2 — Runtime Ubuntu 22.04 (OpenSSL 1.1.x)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y ca-certificates && \
    update-ca-certificates

COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TodoApi.dll"]