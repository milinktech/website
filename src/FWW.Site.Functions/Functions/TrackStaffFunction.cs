using FWW.Site.Shared.Models;
using FWW.Site.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FWW.Site.Functions.Functions;

/// <summary>
/// Staff tracking endpoint - requires authentication (token validated by SWA or manually)
/// Returns full case details including internal notes
/// </summary>
public class TrackStaffFunction
{
    private readonly ILogger<TrackStaffFunction> _logger;
    private readonly DataverseService _dataverseService;

    public TrackStaffFunction(ILogger<TrackStaffFunction> logger, DataverseService dataverseService)
    {
        _logger = logger;
        _dataverseService = dataverseService;
    }

    [Function("TrackStaff")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "track/staff/{caseId}")] HttpRequest req,
        string caseId)
    {
        _logger.LogInformation("Staff tracking request for case: {CaseId}", caseId);

        // TODO: In production, validate the Azure Entra ID token from the request header
        // For SWA Managed Functions, authentication is handled automatically via staticwebapp.config.json
        // For now, we check if user info header exists (set by SWA auth)
        var userPrincipal = req.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(userPrincipal))
        {
            _logger.LogWarning("Unauthorized access attempt to staff tracking");
            return new UnauthorizedObjectResult(new { error = "Authentication required" });
        }

        if (string.IsNullOrWhiteSpace(caseId))
        {
            return new BadRequestObjectResult(new { error = "Case ID is required" });
        }

        try
        {
            var result = await _dataverseService.GetStaffTrackingAsync(caseId);

            if (result == null)
            {
                return new NotFoundObjectResult(new { 
                    error = "Case not found",
                    caseId = caseId
                });
            }

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff tracking info for {CaseId}", caseId);
            return new StatusCodeResult(500);
        }
    }

    /// <summary>
    /// Search cases by customer or tracking number (staff only)
    /// </summary>
    [Function("TrackStaffSearch")]
    public async Task<IActionResult> Search(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "track/staff/search")] HttpRequest req)
    {
        var userPrincipal = req.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(userPrincipal))
        {
            return new UnauthorizedObjectResult(new { error = "Authentication required" });
        }

        var query = req.Query["q"].ToString();
        
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return new BadRequestObjectResult(new { error = "Search query must be at least 3 characters" });
        }

        // TODO: Implement search in Dataverse
        // For now, return mock results
        var mockResults = new List<StaffTrackingResponse>
        {
            new StaffTrackingResponse
            {
                CaseId = "12345678",
                TrackingNumber = $"FWW{query.ToUpper()}001",
                Status = "In Transit",
                StatusDescription = "Shipment is on its way",
                Origin = "Shanghai, China",
                Destination = "Los Angeles, USA"
            }
        };

        return new OkObjectResult(mockResults);
    }
}
