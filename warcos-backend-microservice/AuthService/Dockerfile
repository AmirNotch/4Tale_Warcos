FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001/tcp

ENV DOTNET_URLS=http://0.0.0.0:5001/
ENV ASPNETCORE_URLS "http://0.0.0.0:5001/"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY IdentityServer/AuthService.csproj IdentityServer/
COPY Libraries/IdentityServer4.EntityFramework.Storage/IdentityServer4.EntityFramework.Storage.csproj Libraries/IdentityServer4.EntityFramework.Storage/
COPY Libraries/IdentityServer4.EntityFramework/IdentityServer4.EntityFramework.csproj Libraries/IdentityServer4.EntityFramework/
COPY . .
WORKDIR "/src/IdentityServer"
RUN dotnet restore "./AuthService.csproj"
RUN dotnet build "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# COPY run_auth_service.sh .
# COPY env.sh .
ENTRYPOINT ["dotnet", "AuthService.dll"]
