# =========================
# Runtime image
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# =========================
# Build image
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj files first (for caching)
COPY HelpDesk-API.sln ./
COPY HelpDesk.API/HelpDesk.API.csproj HelpDesk.API/
COPY HelpDesk.Repository/HelpDesk.Repository.csproj HelpDesk.Repository/

# Restore dependencies
RUN dotnet restore HelpDesk-API.sln

# Copy everything else
COPY . .

# Publish API project
RUN dotnet publish HelpDesk.API/HelpDesk.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =========================
# Final image
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelpDesk.API.dll"]
