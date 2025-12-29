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

# Copy solution
COPY HelpDesk-API.sln .

# Copy all csproj files (IMPORTANT)
COPY HelpDesk.API/HelpDesk.API.csproj HelpDesk.API/
COPY HelpDesk.Common/HelpDesk.Common.csproj HelpDesk.Common/
COPY HelpDesk.Repositories/HelpDesk.Repositories.csproj HelpDesk.Repositories/
COPY HelpDesk.Services/HelpDesk.Services.csproj HelpDesk.Services/

# Restore dependencies
RUN dotnet restore HelpDesk.API/HelpDesk.API.csproj

# Copy the rest of the source code
COPY . .

# Build & publish
RUN dotnet publish HelpDesk.API/HelpDesk.API.csproj -c Release -o /app/publish

# =========================
# Final image
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelpDesk.API.dll"]
