FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine-arm64v8 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
WORKDIR "/"
COPY "api.literaturetime.sln" "./"
COPY ["src/API.LiteratureTime.API/API.LiteratureTime.API.csproj", "./src/API.LiteratureTime.API/"]
COPY ["src/API.LiteratureTime.Core/API.LiteratureTime.Core.csproj", "./src/API.LiteratureTime.Core/"]
COPY ["src/API.LiteratureTime.Infrastructure/API.LiteratureTime.Infrastructure.csproj", "./src/API.LiteratureTime.Infrastructure/"]
COPY ["src/Irrbloss/Irrbloss.csproj", "./src/Irrbloss/"]
COPY . .

FROM build AS publish
WORKDIR "/src/API.LiteratureTime.API/."
RUN dotnet publish "API.LiteratureTime.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.LiteratureTime.API.dll"]
