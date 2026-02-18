FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Sentinel.Bot/Sentinel.Bot.csproj src/Sentinel.Bot/
RUN dotnet restore src/Sentinel.Bot/Sentinel.Bot.csproj

COPY . .
RUN dotnet publish src/Sentinel.Bot/Sentinel.Bot.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Sentinel.Bot.dll"]
