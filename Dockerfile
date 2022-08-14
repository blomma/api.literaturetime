FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal-arm64v8 AS base
WORKDIR /app
EXPOSE 433

ENV ASPNETCORE_URLS=https://+:433

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY "src/API.Literature.sln" "./"
COPY ["src/API.Literature.API/API.Literature.API.csproj", "./API.Literature.API/"]
COPY ["src/API.Literature.Core/API.Literature.Core.csproj", "./API.Literature.Core/"]
COPY ["src/API.Literature.Infrastructure/API.Literature.Infrastructure.csproj", "./API.Literature.Infrastructure/"]
COPY ["src/Irrbloss/Irrbloss.csproj", "./Irrbloss/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/API.Literature.API/."
RUN dotnet build "API.Literature.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.Literature.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.Literature.API.dll"]
