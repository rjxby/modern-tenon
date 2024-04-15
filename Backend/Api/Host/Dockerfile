FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["./Directory.Packages.props", "./"]
COPY ["Api/Host/ModernTenon.Api.Host.csproj", "Host/"]
COPY ["Api/Services/Implimentation/ModernTenon.Api.Services.Implimentation.csproj", "Services/Implimentation/"]
COPY ["Api/Repositories/Implimentation/ModernTenon.Api.Repositories.Implimentation.csproj", "Repositories/Implimentation/"]
RUN dotnet restore "Host/ModernTenon.Api.Host.csproj"
COPY Api/ .
RUN dotnet publish "Host/ModernTenon.Api.Host.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ModernTenon.Api.Host.dll"]
