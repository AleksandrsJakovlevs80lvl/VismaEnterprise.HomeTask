FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /
COPY . .
RUN dotnet publish \
    VismaEnterprise.HomeTask.Web/VismaEnterprise.HomeTask.Web.csproj \
    --configuration Release \
    --output /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /var/task
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "VismaEnterprise.HomeTask.Web.dll"]