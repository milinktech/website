# Alignment Plan: Shared Library for milinktech

This plan introduces a shared class library to `milinktech` to resolve model duplication and bring its architecture in line with the `ssca-bc` project.

## Proposed Changes

### [milinktech/website]

#### [NEW] [FWW.Site.Shared](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.Shared/FWW.Site.Shared.csproj)
- Create a new .NET 8.0 Class Library project.
- Move `TrackingModels.cs` and `ChatModels.cs` from the Functions project to this shared library.
- Rename DTOs to be consistent (e.g., `TrackingResult`, `TrackingEvent`).

#### [MODIFY] [FWW.Site.Functions](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.Functions/FWW.Site.Functions.csproj)
- Add a reference to `FWW.Site.Shared`.
- Remove local model definitions in `Functions/Models`.

#### [MODIFY] [FWW.Site.UI](file:///c:/Users/hwang5/Projects/milinktech/website/src/FWW.Site.UI/FWW.Site.UI.csproj)
- Add a reference to `FWW.Site.Shared`.
- Remove duplicated model definitions in `Services/TrackingService.cs` and `Services/ChatService.cs`.

## Verification Plan

### Automated Tests
- Run `dotnet build` to ensure the solution compiles correctly with the new project references.

### Manual Verification
- Verify that the UI correctly serializes/deserializes data using the shared models during local debugging.
