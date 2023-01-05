FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS source
COPY . .

FROM source as restore
RUN dotnet restore

FROM restore as build
RUN dotnet build -c Release --no-restore

FROM build as publish
RUN dotnet publish AzureBlobUpload -c Release -o /app/publish --no-build

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzureBlobUpload.dll"]
