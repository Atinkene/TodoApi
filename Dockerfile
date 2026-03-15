# Étape 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Installation des certificats et ajustement d'OpenSSL
RUN apt-get update && \
    apt-get install -y ca-certificates openssl && \
    # On remplace SECLEVEL=2 par SECLEVEL=1 pour compatibilité avec Atlas
    sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    update-ca-certificates

# SUPPRIMEZ CETTE LIGNE : Elle cause le plantage Status 139
# ENV OPENSSL_CONF=/dev/null 

COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TodoApi.dll"]