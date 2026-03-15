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

# Créer une config OpenSSL permissive
RUN apt-get update && apt-get install -y ca-certificates openssl && \
    update-ca-certificates

RUN echo "[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT@SECLEVEL=0" \
    > /etc/ssl/openssl_mongo.cnf

ENV OPENSSL_CONF=/etc/ssl/openssl_mongo.cnf

COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TodoApi.dll"]