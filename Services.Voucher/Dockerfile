# Build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ENV DOTNET_EnableDiagnostics=0
WORKDIR /app
COPY . .
RUN dotnet publish -f net5.0 -c Release --nologo -o out ./Services.Voucher/Services.Voucher.csproj

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
VOLUME /root/.aspnet/DataProtection-Keys
ENV DOTNET_EnableDiagnostics=0
ENV ASPNETCORE_ENVIRONMENT=Development
ENV PORT=80
EXPOSE 80
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Services.Voucher.dll"]
HEALTHCHECK CMD curl --fail http://localhost:$PORT/healthz/ready || exit
