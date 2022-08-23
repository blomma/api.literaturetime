FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR "/"
COPY "api.literature.sln" "./"
COPY ["src/API.Literature.API/API.Literature.API.csproj", "./src/API.Literature.API/"]
COPY ["src/API.Literature.Core/API.Literature.Core.csproj", "./src/API.Literature.Core/"]
COPY ["src/API.Literature.Infrastructure/API.Literature.Infrastructure.csproj", "./src/API.Literature.Infrastructure/"]
COPY ["src/Irrbloss/Irrbloss.csproj", "./src/Irrbloss/"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/API.Literature.API/."
RUN dotnet publish "API.Literature.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.Literature.API.dll"]
