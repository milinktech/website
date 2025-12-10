# FOCUS Logistics - Deployment Guide

## Overview

Deploy the Blazor WASM frontend + Azure Functions backend to **Azure Static Web Apps (SWA)**.

---

## Prerequisites

| Item | Details |
|------|---------|
| Azure Subscription | Active subscription |
| GitHub Repository | Code pushed to GitHub |
| Azure CLI | [Install](https://docs.microsoft.com/cli/azure/install-azure-cli) |
| Azure Entra ID | App registration created |

---

## Step 1: Create Azure Static Web App

### Via Azure Portal

1. Go to **Azure Portal** → **Create a resource** → **Static Web App**
2. Configure:
   - **Name**: `focus-logistics-site`
   - **Region**: Select closest to users
   - **Source**: GitHub
   - **Organization**: Your org
   - **Repository**: `website`
   - **Branch**: `main`
   - **Build Presets**: Blazor
   - **App location**: `/src/FWW.Site.UI`
   - **API location**: `/src/FWW.Site.Functions`
   - **Output location**: `wwwroot`

3. Click **Create**

### Via Azure CLI

```bash
az staticwebapp create \
  --name focus-logistics-site \
  --resource-group your-rg \
  --source https://github.com/your-org/website \
  --location "East US 2" \
  --branch main \
  --app-location "/src/FWW.Site.UI" \
  --api-location "/src/FWW.Site.Functions" \
  --output-location "wwwroot" \
  --login-with-github
```

---

## Step 2: GitHub Actions Workflow

SWA auto-creates a workflow file. If needed, customize:

```yaml
# .github/workflows/azure-static-web-apps.yml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build_and_deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Build And Deploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "/src/FWW.Site.UI"
          api_location: "/src/FWW.Site.Functions"
          output_location: "wwwroot"
```

---

## Step 3: Configure Application Settings

### Azure Entra ID (Authentication)

1. Go to **Azure Portal** → **Static Web App** → **Settings** → **Configuration**
2. Add application settings:

| Name | Value |
|------|-------|
| `AzureAd__Authority` | `https://login.microsoftonline.com/YOUR_TENANT_ID` |
| `AzureAd__ClientId` | `YOUR_CLIENT_ID` |

3. Update **App Registration** redirect URIs:
   - `https://your-app.azurestaticapps.net/authentication/login-callback`

### AI Chat (Optional)

| Name | Value |
|------|-------|
| `AzureAiFoundry__Endpoint` | `https://your-resource.openai.azure.com` |
| `AzureAiFoundry__ApiKey` | `your-api-key` |
| `AzureAiFoundry__DeploymentName` | `gpt-4o` |

### Dataverse (Future)

| Name | Value |
|------|-------|
| `Dataverse__Environment` | `https://your-org.crm.dynamics.com` |
| `Dataverse__ClientId` | `service-principal-id` |
| `Dataverse__ClientSecret` | `secret` |

---

## Step 4: Custom Domain (Optional)

1. Go to **Static Web App** → **Custom domains**
2. Click **Add** → Enter your domain
3. Configure DNS:
   - **CNAME**: `www` → `your-app.azurestaticapps.net`
   - **A Record** (apex): Use Azure's IP

4. Add domain to App Registration redirect URIs

---

## Step 5: Verify Deployment

| Test | URL |
|------|-----|
| Homepage | `https://your-app.azurestaticapps.net/` |
| Tracking | `https://your-app.azurestaticapps.net/track` |
| Tools | `https://your-app.azurestaticapps.net/tools` |
| API (Public Track) | `https://your-app.azurestaticapps.net/api/track/FWW123` |
| API (Chat) | `POST https://your-app.azurestaticapps.net/api/chat` |

---

## Local Development

### Run Both Projects

**Terminal 1 - Blazor:**
```bash
cd src/FWW.Site.UI
dotnet run
```

**Terminal 2 - Functions:**
```bash
cd src/FWW.Site.Functions
func start --port 7071
```

### Using SWA CLI (Production-like)

```bash
npm install -g @azure/static-web-apps-cli
cd src
swa start http://localhost:60637 --api-location FWW.Site.Functions
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| 404 on page refresh | Ensure `staticwebapp.config.json` has navigation fallback |
| API returns 401 | Check SWA auth config and token scopes |
| Chat returns fallback | Configure AI Foundry settings in app config |
| CORS errors | SWA handles CORS automatically for managed functions |

---

## Files Reference

| File | Purpose |
|------|---------|
| `staticwebapp.config.json` | SWA routing & fallback config |
| `src/FWW.Site.UI/wwwroot/appsettings.json` | Frontend auth config |
| `src/FWW.Site.Functions/local.settings.json` | Local dev secrets |
