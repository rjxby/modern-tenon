FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
RUN apk add --no-cache icu-libs
RUN mkdir -p /data
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ConnectionStrings__DefaultConnection="Data Source=/data/modern-tenon.db"
VOLUME ["/data"]

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
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
