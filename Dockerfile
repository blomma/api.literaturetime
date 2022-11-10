FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy-arm64v8 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR "/"
COPY "api.literaturetime.sln" "./"
COPY ["src/API.LiteratureTime.API/API.LiteratureTime.API.csproj", "./src/API.LiteratureTime.API/"]
COPY ["src/API.LiteratureTime.Core/API.LiteratureTime.Core.csproj", "./src/API.LiteratureTime.Core/"]
COPY ["src/API.LiteratureTime.Infrastructure/API.LiteratureTime.Infrastructure.csproj", "./src/API.LiteratureTime.Infrastructure/"]
COPY ["src/Irrbloss/Irrbloss.csproj", "./src/Irrbloss/"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/API.LiteratureTime.API/."
RUN dotnet publish "API.LiteratureTime.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.LiteratureTime.API.dll"]
