# Telegram Crypto Safety Sentinel 🛡️
A production-style **Telegram bot + backend service** that helps protect crypto communities from modern scams:
**phishing links, fake airdrops, “connect wallet” drains, address poisoning, and suspicious token/contract links.**

Built with **.NET 8**, **Docker**, **EF Core migrations**, and security-minded engineering (rate limiting, audit logs, permissions, safe defaults).

---

## Why this exists (modern crypto safety problem)
Crypto Telegram groups are heavily targeted by:
- **Airdrop scams** and “claim now” drain sites
- **Fake support/admin impersonation** pushing links
- **Malicious “connect wallet” dApps** and sign-message traps
- **Address poisoning** (lookalike wallet addresses pasted in chat)
- **Fake token contracts** and misleading explorer links

Crypto Safety Sentinel reduces risk by automatically scanning messages and replying with a **risk report** and applying a configured moderation action.

---

## Features
### ✅ Core (v1)
#### 1) Scam link detection
- Detects common patterns:
  - **“connect wallet”** / **airdrop** / **claim** / **verify** / **support** bait
  - Short-link expansion (optional)
  - Punycode / lookalike domains / suspicious TLDs / IP-in-URL
- Optional reputation providers (pluggable):
  - VirusTotal URL checks (if enabled)
  - Custom domain blocklists/allowlists

#### 2) Wallet address safety checks (EVM + BTC)
- Detects wallet addresses posted in chat and flags:
  - **Non-checksummed Ethereum addresses** (EIP-55) as higher risk
  - **Lookalike addresses** (high similarity to recently-seen addresses in chat)
  - **Address poisoning patterns** (same prefix/suffix as “trusted” address)
- Maintains a short “recent trusted addresses” cache per chat for comparison.

#### 3) Contract/token link validation (EVM-focused)
- Recognises explorer links (Etherscan-like) and extracts:
  - contract address
  - chain (if detectable by domain / config)
- Optional verification checks via explorer APIs (pluggable):
  - contract verified source (yes/no)
  - contract age (very new contracts flagged)
  - token holders/tx count (low activity flagged)

> Note: The bot is designed to work *without* paid APIs. External checks are optional enhancements.

#### 4) Moderation modes (chat-level)
- `Off` – no action, only responds on `/scan`
- `Report` – reply with risk report
- `Quarantine` – delete + repost sanitized warning (requires permissions)
- `AutoDelete` – delete flagged messages (requires permissions)

#### 5) Admin controls + configuration
- Admin-only commands to set mode, thresholds, allowlists/blocklists, and provider toggles.

---

## Security-minded engineering (portfolio signals)
- **Rate limiting** per user/chat (prevents bot abuse & spam)
- **Idempotency** for Telegram updates (no double-processing)
- **Audit logging** with event trails (who/what/why/action)
- **Secrets-safe config** via environment variables (no keys in git)
- **EF Core migrations** (versioned schema + reproducible deployments)
- **Structured logging** (Serilog) with correlation IDs

---

## Tech Stack
- **.NET 8**
- **Telegram.Bot**
- **EF Core** (+ migrations)
- **PostgreSQL** (recommended) or SQLite (dev)
- **Serilog**
- **Docker + Docker Compose**

Optional providers:
- VirusTotal (URLs)
- Explorer APIs (Etherscan-like) for contract metadata
- Your own JSON feeds for known scam domains / addresses

---

## Quick Start (Docker)
### 1) Prereqs
- Docker + Docker Compose

### 2) Create a Telegram bot token
1. Message **@BotFather**
2. Create a bot and copy the token

### 3) Configure environment variables
Create a `.env` file in the repo root:

```env
TELEGRAM_BOT_TOKEN=YOUR_TOKEN_HERE

DB_HOST=db
DB_PORT=5432
DB_NAME=sentinel
DB_USER=sentinel
DB_PASSWORD=sentinel_password

# Optional providers (leave blank to run in heuristic-only mode)
VIRUSTOTAL_API_KEY=
ETHERSCAN_API_KEY=

# Risk scoring
RISK_HIGH_THRESHOLD=70
RISK_MED_THRESHOLD=40
