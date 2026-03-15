# Étape 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Étape 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Installation des certificats et ajustement d'OpenSSL (Niveau 1)
RUN apt-get update && \
    apt-get install -y ca-certificates openssl && \
    sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    update-ca-certificates

# On copie les fichiers compilés depuis l'étape 1
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "TodoApi.dll"]