# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderApp.Main.Api.Domain/OrderApp.Main.Api.Domain.csproj", "OrderApp.Main.Api.Domain/"]
COPY ["OrderApp.Main.Api.Domain/packages.lock.json", "OrderApp.Main.Api.Domain/"]
COPY ["OrderApp.Main.Api.Application/OrderApp.Main.Api.Application.csproj", "OrderApp.Main.Api.Application/"]
COPY ["OrderApp.Main.Api.Application/packages.lock.json", "OrderApp.Main.Api.Application/"]
COPY ["OrderApp.Main.Api.Infrastructure/OrderApp.Main.Api.Infrastructure.csproj", "OrderApp.Main.Api.Infrastructure/"]
COPY ["OrderApp.Main.Api.Infrastructure/packages.lock.json", "OrderApp.Main.Api.Infrastructure/"]
COPY ["OrderApp.Main.Api.WebApi/OrderApp.Main.Api.WebApi.csproj", "OrderApp.Main.Api.WebApi/"]
COPY ["OrderApp.Main.Api.WebApi/packages.lock.json", "OrderApp.Main.Api.WebApi/"]
RUN dotnet restore --locked-mode "./OrderApp.Main.Api.WebApi/OrderApp.Main.Api.WebApi.csproj"
COPY . .
WORKDIR "/src/OrderApp.Main.Api.WebApi"
RUN dotnet build "./OrderApp.Main.Api.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderApp.Main.Api.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderApp.Main.Api.WebApi.dll"]
