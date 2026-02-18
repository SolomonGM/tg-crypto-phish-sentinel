# Telegram Crypto Safety Sentinel
A starter Telegram bot backend for crypto scam detection and moderation, built with C#/.NET 8.

Current starter implementation includes:
- message scanning heuristics for phishing/lure phrases and risky URLs
- Ethereum checksum checks and wallet lookalike/poisoning detection signals
- moderation modes (`Off`, `Report`, `Quarantine`, `AutoDelete`)
- admin command to change mode (`/mode`)
- idempotent update handling, in-memory rate limiting, and audit logging
- EF Core database layer (Postgres or SQLite) and Docker Compose setup

## Project Layout
- `src/Sentinel.Bot/Program.cs` host startup and DI wiring
- `src/Sentinel.Bot/Services/` scanner, polling, moderation, rate limiter
- `src/Sentinel.Bot/Data/` EF Core DbContext and entities
- `docker-compose.yml` app + Postgres stack
- `.env.example` environment template

## First Steps (VS Code, Windows)
1. Install prerequisites:
   - .NET 8 SDK
   - VS Code extensions: `C# Dev Kit` and `C#`
   - Docker Desktop (optional, but recommended)
2. Open this folder in VS Code.
3. Copy env template and set your bot token:
   ```powershell
   Copy-Item .env.example .env
   ```
4. Edit `.env` and set `TELEGRAM_BOT_TOKEN`.

## Run Option A: Docker (Recommended)
```powershell
docker compose up --build
```

## Run Option B: Local in VS Code
1. Use SQLite for local dev (no Postgres needed):
   ```powershell
   $env:DB_PROVIDER="sqlite"
   $env:TELEGRAM_BOT_TOKEN="YOUR_TOKEN"
   dotnet restore src/Sentinel.Bot/Sentinel.Bot.csproj
   dotnet run --project src/Sentinel.Bot/Sentinel.Bot.csproj
   ```
2. Keep terminal running while you message the bot in Telegram.

## Bot Commands
- `/help` or `/start` show command help
- `/scan <text-or-url>` run manual risk scan
- `/mode <off|report|quarantine|autodelete>` change moderation mode (admin only in group chats)

## Notes for Development
- Default thresholds are controlled with:
  - `RISK_HIGH_THRESHOLD` (default `70`)
  - `RISK_MED_THRESHOLD` (default `40`)
- Rate limiting is controlled with:
  - `RATE_LIMIT_WINDOW_SECONDS`
  - `RATE_LIMIT_MAX_MESSAGES`
- Optional provider keys (`VIRUSTOTAL_API_KEY`, `ETHERSCAN_API_KEY`) are defined but not wired yet in this starter.

## Add EF Migrations (Next)
After .NET SDK is installed:
```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Sentinel.Bot/Sentinel.Bot.csproj
```
