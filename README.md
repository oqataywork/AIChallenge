## AIChallenge

AIChallenge is a modular .NET solution that demonstrates an AI‑powered application with:

- A clean separation of **Domain**, **Infrastructure**, **Integrations**, and **Presentation**
- Support for multiple **LLM providers** (OpenAI, DeepSeek, Ollama)
- **RAG-style context management** (saving and querying AI context)
- Optional **MCP-based tools** (file research, forecasting, reminders)

The solution is designed as a reference architecture for building production‑ready AI backends in .NET.

---

## Solution structure

- **`Domain`**: Core domain models and enums (`AiContext`, `AiResponse`, `Chunk`, `ModelType`, `ResponseSource`).
- **`DomainService`**: Application services and business logic:
  - `Services/MessageSender`, `SendMessageHandler`, `SendMessageConverter`
  - `Services/PromptBuilder` and `RagContextService`
  - DI extensions in `ServiceCollectionExtensions`.
- **`Infrastructure`**: Data access and database helpers:
  - `Context/AiContextDb`, `ContextRepository`
  - SQL scripts in `Context/Queries` (`AddContext.sql`, `GetAllContext.sql`)
  - Shared utilities in `Common` (e.g., `SqlScriptProvider`, `SqlRegex`).
- **`Repository`**: Abstraction over context persistence (`IContextRepository`).
- **`Integrations`**: Contracts for external services:
  - **LLMs**: `OpenAI`, `DeepSeek`, `Ollama` via `Integrations.OpenAI`, `Integrations.DeepSeek`, `Integrations.Ollama`
  - **MCP**: `FileResearcher`, `Forecast`, `Reminder` and general MCP server client in `Integrations/Mcp`.
  - Common DTOs in `Integrations/Ai/Contracts`.
- **`Integrations.OpenAI` / `Integrations.DeepSeek` / `Integrations.Ollama`**:
  - Concrete HTTP clients and options
  - DI registration via `ServiceCollectionExtensions`.
- **`Integrations.OgMcpClient`**:
  - Concrete MCP client implementations (`FileResearcherMcpClient`, `ForecastMcpClient`, `SchedulerMcpClient`, `McpServerClient`)
  - Tool registration via `SupportedToolsRegistry`.
- **`Presentation`**:
  - ASP.NET Core Web API entry point (`Program.cs`, `Startup.cs`)
  - Controllers for LLM interaction (e.g., `Controllers/LlmController`)
  - API contracts in `Controllers/Contracts`
  - Configuration via `appsettings.json` and `appsettings.Development.json`.
- **`Runner`**:
  - Console application (`Runner/Program.cs`) useful for local testing, scripting, or background runs.
- **`docker-compose.yaml`**:
  - Orchestrates the API and its backing services (e.g., database and/or other dependencies, depending on your local setup).

---

## Prerequisites

- **.NET SDK**: .NET 9.0 (as indicated by project output in `bin/Debug/net9.0`)
- **Docker** (optional): To run using `docker-compose`.
- Access credentials for any external AI providers you intend to use (e.g. **OpenAI**, **DeepSeek**, **Ollama**, MCP servers), typically provided as environment variables or configuration in `appsettings.json`.

---

## Getting started (local development)

### 1. Restore and build

From the solution root:

```bash
dotnet restore
dotnet build
```

### 2. Configure settings

Review and update `Presentation/appsettings.Development.json` (and/or `appsettings.json`) to match your environment:

- **Connection strings** used by `Infrastructure/Context/AiContextDb`
- **API keys / endpoints** for:
  - OpenAI (`Integrations.OpenAI/OpenAiOptions`)
  - DeepSeek (`Integrations.DeepSeek/DeepSeekOptions`)
  - Ollama (`Integrations.Ollama/OllamaClient`)
  - MCP server base URLs used by `Integrations.OgMcpClient`

You can also use environment variables to override configuration values in typical ASP.NET Core fashion.

### 3. Run the Web API

```bash
dotnet run --project Presentation
```

By default the API will start on the port configured in `Properties/launchSettings.json`. Common patterns:

- `https://localhost:{port}`
- `http://localhost:{port}`

Once running, you can:

- Call the LLM endpoints in `LlmController` (for sending messages and retrieving responses).
- Use your preferred HTTP client (curl, Postman, VS Code/JetBrains HTTP client) to test the endpoints.

### 4. Run the console Runner

```bash
dotnet run --project Runner
```

`Runner` is a thin console app that can be wired to invoke the same domain services as the web API for scripted or background usage.

---

## Running with Docker

If you prefer to run the stack via Docker, from the solution root:

```bash
docker-compose up --build
```

This will:

- Build the necessary images
- Start the web API and any configured backing services

Check `docker-compose.yaml` for the exposed ports and service names, and then call the API in the same way as in local development (using the mapped host ports).

---

## Key concepts

- **Message sending pipeline**  
  The `DomainService` layer orchestrates:
  - Building prompts (`PromptBuilder`)
  - Fetching relevant context (`RagContextService`, `ContextRepository`, `AiContextDb`)
  - Invoking the appropriate LLM client (`OpenAiClient`, `DeepSeekAiClient`, `OllamaClient`)
  - Converting requests and responses between the API, domain, and integration layers.

- **RAG / Context storage**  
  `AiContext`, `Chunk`, and related SQL scripts in `Infrastructure/Context/Queries` implement a simple pattern for:
  - Storing conversation or knowledge chunks
  - Retrieving relevant pieces for augmentation of prompts

- **MCP integrations**  
  `Integrations.OgMcpClient` and `Integrations/Mcp` define clients and contracts for MCP servers that can be used as tools by the AI models (e.g., file research, forecasting, reminders).

---

## Typical workflows

- **Send a message to an LLM via HTTP**:
  - Start the API (`dotnet run --project Presentation`)
  - Call the LLM endpoint defined in `Controllers/LlmController` with the appropriate request contract from `Controllers/Contracts`.

- **Extend with a new LLM provider**:
  - Add a new integration project (or expand `Integrations`).
  - Implement a client (similar to `OpenAiClient` or `DeepSeekAiClient`).
  - Register it via a DI extension (`ServiceCollectionExtensions`).
  - Wire it into `DomainService` (e.g., `MessageSender` / `SendMessageHandler`).

- **Add a new MCP tool**:
  - Define contracts under `Integrations/Mcp/Contracts`.
  - Implement the corresponding client in `Integrations.OgMcpClient`.
  - Register the tool in `SupportedToolsRegistry` and connect it to your LLM configuration.

---

## Testing & quality

There are no explicit test projects listed in the current tree; if you add tests, a common layout is:

- `tests/Domain.Tests`
- `tests/DomainService.Tests`
- `tests/Integrations.Tests`

You can then run:

```bash
dotnet test
```

---

## Contributing / customizing

- **Configuration**: Prefer environment variables and `appsettings.*.json` files to keep secrets out of source control.
- **Logging & observability**: Extend logging in `Presentation/Program.cs` and `Startup.cs` and ensure downstream services log key events (request IDs, provider names, model types).
- **Error handling**: Centralize exception handling at the API boundary (middleware/filters) and in integration clients (retry policies, timeouts, fallbacks).

Feel free to adapt this structure to your specific AI use case (chat bots, agents, batch processing, etc.).


