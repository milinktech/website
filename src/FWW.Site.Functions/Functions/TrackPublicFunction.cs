using FWW.Site.Functions.Models;
using FWW.Site.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FWW.Site.Functions.Functions;

/// <summary>
/// Public tracking endpoint - no authentication required
/// </summary>
public class TrackPublicFunction
{
    private readonly ILogger<TrackPublicFunction> _logger;
    private readonly DataverseService _dataverseService;

    public TrackPublicFunction(ILogger<TrackPublicFunction> logger, DataverseService dataverseService)
    {
        _logger = logger;
        _dataverseService = dataverseService;
    }

    [Function("TrackPublic")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "track/{trackingNumber}")] HttpRequest req,
        string trackingNumber)
    {
        _logger.LogInformation("Public tracking request for: {TrackingNumber}", trackingNumber);

        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            return new BadRequestObjectResult(new { error = "Tracking number is required" });
        }

        // Sanitize input
        trackingNumber = trackingNumber.Trim().ToUpperInvariant();

        try
        {
            var result = await _dataverseService.GetPublicTrackingAsync(trackingNumber);

            if (result == null)
            {
                return new NotFoundObjectResult(new { 
                    error = "Tracking number not found",
                    trackingNumber = trackingNumber
                });
            }

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tracking info for {TrackingNumber}", trackingNumber);
            return new StatusCodeResult(500);
        }
    }
}
