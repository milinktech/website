# Alignment Plan: SWA-Native Authentication for milinktech

This plan aligns the `milinktech` project with the SWA-native authentication approach used in `ssca-bc`. This shift moves authentication logic from the client-side (MSAL) to the Azure Static Web Apps infrastructure.

## Proposed Changes

### [milinktech/website]

#### [MODIFY] [staticwebapp.config.json](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.UI/wwwroot/staticwebapp.config.json) [NEW]
- Move the file from the root to `src/FWW.Site.UI/wwwroot/` to ensure it's served as a static asset.
- Add `auth` configuration for Azure Active Directory (Entra ID).
- Define route rules for protected areas (e.g., tracking or staff functions).
- Add security headers (CSP, HSTS, etc.) to match `ssca-bc`'s robust configuration.

#### [MODIFY] [Program.cs](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.UI/Program.cs)
- Remove `builder.Services.AddMsalAuthentication`.
- Simplify `HttpClient` registration if token management is no longer needed client-side (SWA handles this via cookies).

#### [MODIFY] [index.html](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.UI/wwwroot/index.html)
- Remove the MSAL authentication script: `<script src="_content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js"></script>`.

#### [DELETE] [appsettings.json](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.UI/wwwroot/appsettings.json)
- Remove AzureAd configuration as it will be managed via SWA infrastructure settings.

## Verification Plan

### Manual Verification
1. **Local Development**: Verify that the app still loads without the MSAL script. Note that SWA EasyAuth requires the SWA CLI (`swa start`) for local emulation of the auth flow.
2. **Deployment**: After deploying to Azure SWA, verify:
    - Navigation to protected routes triggers the Entra ID login flow.
    - User information is accessible via the `/.auth/me` endpoint.
    - API calls to Azure Functions are correctly authenticated using the SWA session cookie.
