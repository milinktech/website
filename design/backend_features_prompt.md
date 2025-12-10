# FOCUS Logistics - Backend Features Regeneration Prompt

Use this prompt **in addition to** `project_regeneration_prompt.md` to add backend features.

---

## 📋 Overview

This prompt adds the following to the base Blazor WASM project:
1. **Azure Entra ID Authentication** (MSAL)
2. **Azure Functions Backend** (managed by Static Web Apps)
3. **Track & Trace** with Dataverse integration
4. **AI Chat Agent** with Azure AI Foundry
5. **Unit Converter** tool

---

## 📁 Additional Project Structure

```
src/
├── FWW.Site.sln                     # Solution file
├── FWW.Site.UI/                     # Blazor WASM (from original prompt)
│   ├── Pages/
│   │   ├── Authentication.razor     # [NEW] Auth callback page
│   │   ├── Track.razor              # [NEW] Public tracking page
│   │   └── Tools.razor              # [NEW] Unit converter page
│   ├── Components/
│   │   └── UnitConverter.razor      # [NEW] Unit converter component
│   ├── Shared/
│   │   ├── LoginDisplay.razor       # [NEW] Login/logout buttons
│   │   ├── RedirectToLogin.razor    # [NEW] Auth redirect helper
│   │   └── ChatPopup.razor          # [MODIFIED] Uses ChatService
│   ├── Services/
│   │   ├── TrackingService.cs       # [NEW] Tracking API client
│   │   └── ChatService.cs           # [NEW] Chat API client
│   ├── Program.cs                   # [MODIFIED] MSAL + services
│   └── wwwroot/
│       └── appsettings.json         # [NEW] Azure AD config
│
├── FWW.Site.Functions/              # [NEW] Azure Functions project
│   ├── Functions/
│   │   ├── TrackPublicFunction.cs   # GET /api/track/{number}
│   │   ├── TrackStaffFunction.cs    # GET /api/track/staff/{id}
│   │   └── ChatAgentFunction.cs     # POST /api/chat
│   ├── Services/
│   │   ├── DataverseService.cs      # Dataverse API client
│   │   └── AiFoundryService.cs      # Azure AI Foundry client
│   ├── Models/
│   │   ├── TrackingModels.cs        # Tracking data models
│   │   └── ChatModels.cs            # Chat request/response
│   ├── Program.cs                   # DI configuration
│   ├── host.json
│   └── local.settings.json
│
└── staticwebapp.config.json         # [NEW] SWA routing config
```

---

## 🔐 Authentication Setup (Phase 0)

### Blazor WASM Configuration

**Program.cs** additions:
```csharp
using FWW.Site.UI.Services;

// Register services
builder.Services.AddScoped<TrackingService>();
builder.Services.AddScoped<ChatService>();

// Configure MSAL Authentication
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
    options.ProviderOptions.LoginMode = "redirect";
});
```

**wwwroot/appsettings.json**:
```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{TENANT_ID}",
    "ClientId": "{CLIENT_ID}",
    "ValidateAuthority": true
  }
}
```

**_Imports.razor** additions:
```razor
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

**App.razor** - Wrap with CascadingAuthenticationState:
```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    @if (context.User.Identity?.IsAuthenticated != true)
                    { <RedirectToLogin /> }
                    else
                    { <p>Access Denied</p> }
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
    </Router>
</CascadingAuthenticationState>
```

**index.html** - Add before blazor.webassembly.js:
```html
<script src="_content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js"></script>
```

### Authentication Components

**LoginDisplay.razor**:
```razor
@inject NavigationManager Navigation

<AuthorizeView>
    <Authorized>
        <div class="user-menu">
            <span class="user-name">Hello, @context.User.Identity?.Name</span>
            <button class="btn btn-light btn-rounded" @onclick="BeginLogout">Logout</button>
        </div>
    </Authorized>
    <NotAuthorized>
        <a href="authentication/login" class="btn btn-light btn-rounded">Staff Login</a>
    </NotAuthorized>
</AuthorizeView>

@code {
    private void BeginLogout() => Navigation.NavigateToLogout("authentication/logout");
}
```

**Authentication.razor**:
```razor
@page "/authentication/{action}"
<RemoteAuthenticatorView Action="@Action" />
@code {
    [Parameter] public string? Action { get; set; }
}
```

---

## 🔧 Unit Converter (Phase 1)

### Tools.razor
- Route: `/tools`
- Hero section with gradient background
- Embedded `<UnitConverter />` component
- "Coming Soon" cards for future tools

### UnitConverter.razor
4 converter cards using `@bind:after` for real-time updates:
1. **Weight**: kg ↔ lb (factor: 2.20462)
2. **Length**: cm ↔ inch (factor: 0.393701)
3. **Volume**: m³ ↔ ft³ (factor: 35.3147)
4. **CBM Calculator**: L×W×H (cm) × Qty → m³/ft³

---

## 📦 Track & Trace (Phase 2)

### Backend API Endpoints

| Endpoint | Auth | Response |
|----------|------|----------|
| `GET /api/track/{trackingNumber}` | None | Public status only |
| `GET /api/track/staff/{caseId}` | Entra ID | Full case details |

### TrackingModels.cs
```csharp
public class TrackingCase { ... }
public class TrackingEvent { Timestamp, Location, Status, Description }
public class PublicTrackingResponse { TrackingNumber, Status, Origin, Destination, ETA, Events }
public class StaffTrackingResponse : PublicTrackingResponse { CaseId, CustomerInfo, InternalNotes }
```

### DataverseService.cs
- Mock implementation for development
- Returns realistic tracking data for numbers starting with "FWW"
- TODO markers for actual Dataverse API integration

### Track.razor (Frontend)
- Search form with tracking number input
- Status header with icon (based on status)
- Route visualization (Origin → Destination)
- Event timeline with locations and timestamps

---

## 🤖 AI Chat Agent (Phase 3)

### Backend

**ChatAgentFunction.cs**: `POST /api/chat`
- Accepts: `{ message, history, sessionId }`
- Returns: `{ message, sessionId, success }`

**AiFoundryService.cs**:
- Integrates with Azure OpenAI/AI Foundry
- System prompt defines FOCUS Assistant persona
- Fallback responses when AI not configured
- Configuration keys: `AzureAiFoundry:Endpoint`, `ApiKey`, `DeploymentName`

### Frontend

**ChatService.cs**:
- Maintains session ID across messages
- Sends conversation history for context

**ChatPopup.razor** (Modified):
- Injects `ChatService`
- Replaces placeholder with `await ChatService.SendMessageAsync()`
- Handles errors gracefully

---

## 📄 Static Web Apps Configuration

**staticwebapp.config.json** (at repo root):
```json
{
  "platform": { "apiRuntime": "dotnet-isolated:8.0" },
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["/api/*", "/_framework/*", "/css/*"]
  },
  "routes": [
    { "route": "/api/*", "allowedRoles": ["anonymous"] }
  ]
}
```

---

## 🎨 Additional CSS

Add to `app.css`:

```css
/* Unit Converter */
.converter-inputs { display: flex; align-items: flex-end; gap: 1rem; }
.converter-arrow { color: var(--color-primary); }
.cbm-result { display: flex; gap: 2rem; border-top: 1px solid var(--color-border); }
.result-value { font-size: 1.5rem; color: var(--color-primary); }

/* Tracking */
.tracking-status { display: flex; align-items: center; gap: 1rem; }
.status-icon.status-delivered { color: #10b981; }
.tracking-route { display: flex; align-items: center; }
.route-line { flex: 1; height: 2px; background: linear-gradient(...); }
.tracking-timeline { position: relative; padding-left: 1.5rem; }
.timeline-dot { background: var(--color-primary); border-radius: 50%; }

/* User Menu */
.user-menu { display: flex; align-items: center; gap: 0.75rem; }
```

---

## 🔄 NavMenu Updates

Add to navigation links:
```razor
<NavLink class="nav-link" href="/track">Track</NavLink>
<NavLink class="nav-link" href="/tools">Tools</NavLink>
```

Add to right side:
```razor
<LoginDisplay />
```

---

## 📦 NuGet Packages

**FWW.Site.UI.csproj** additions:
```xml
<PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="8.0.0" />
```

**FWW.Site.Functions.csproj**:
```xml
<PackageReference Include="Microsoft.Azure.Functions.Worker" Version="1.20.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore" Version="1.2.0" />
<PackageReference Include="Microsoft.Azure.Functions.Worker.Sdk" Version="1.16.2" />
<PackageReference Include="Microsoft.ApplicationInsights.WorkerService" Version="2.21.0" />
```

---

## 🚀 Local Development

```bash
# Terminal 1: Run Blazor
cd src/FWW.Site.UI && dotnet run

# Terminal 2: Run Functions
cd src/FWW.Site.Functions && func start --port 7071

# Or use SWA CLI
swa start http://localhost:60637 --api-location src/FWW.Site.Functions
```

---

*Use this prompt with the original `project_regeneration_prompt.md` to recreate the full FOCUS Logistics application with backend features.*
