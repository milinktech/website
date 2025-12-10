using FWW.Site.Functions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FWW.Site.Functions.Services;

/// <summary>
/// Service for interacting with Dataverse API to query tracking cases
/// </summary>
public class DataverseService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataverseService> _logger;

    // TODO: Add these fields when implementing actual Dataverse auth
    // private string? _accessToken;
    // private DateTime _tokenExpiry = DateTime.MinValue;

    public DataverseService(HttpClient httpClient, IConfiguration configuration, ILogger<DataverseService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get tracking info for public users (limited data)
    /// </summary>
    public async Task<PublicTrackingResponse?> GetPublicTrackingAsync(string trackingNumber)
    {
        var trackingCase = await GetCaseByTrackingNumberAsync(trackingNumber);
        
        if (trackingCase == null)
            return null;

        return new PublicTrackingResponse
        {
            TrackingNumber = trackingCase.TrackingNumber,
            Status = trackingCase.Status,
            StatusDescription = trackingCase.StatusDescription,
            Origin = trackingCase.Origin,
            Destination = trackingCase.Destination,
            EstimatedArrival = trackingCase.EstimatedArrival,
            Events = trackingCase.Events
        };
    }

    /// <summary>
    /// Get tracking info for staff (full data)
    /// </summary>
    public async Task<StaffTrackingResponse?> GetStaffTrackingAsync(string caseId)
    {
        var trackingCase = await GetCaseByCaseIdAsync(caseId);
        
        if (trackingCase == null)
            return null;

        return new StaffTrackingResponse
        {
            CaseId = trackingCase.CaseId,
            TrackingNumber = trackingCase.TrackingNumber,
            Status = trackingCase.Status,
            StatusDescription = trackingCase.StatusDescription,
            Origin = trackingCase.Origin,
            Destination = trackingCase.Destination,
            EstimatedArrival = trackingCase.EstimatedArrival,
            ActualArrival = trackingCase.ActualArrival,
            CreatedOn = trackingCase.CreatedOn,
            ModifiedOn = trackingCase.ModifiedOn,
            Events = trackingCase.Events,
            // Additional fields for staff would come from Dataverse
            InternalNotes = "",
            CustomerName = "",
            CustomerEmail = ""
        };
    }

    private async Task<TrackingCase?> GetCaseByTrackingNumberAsync(string trackingNumber)
    {
        // TODO: Replace with actual Dataverse API call
        // This is a mock implementation for development
        
        _logger.LogInformation("Querying Dataverse for tracking number: {TrackingNumber}", trackingNumber);

        // Mock data for testing - replace with actual Dataverse query
        if (trackingNumber.StartsWith("FWW"))
        {
            return CreateMockCase(trackingNumber);
        }

        return null;
    }

    private async Task<TrackingCase?> GetCaseByCaseIdAsync(string caseId)
    {
        // TODO: Replace with actual Dataverse API call
        
        _logger.LogInformation("Querying Dataverse for case ID: {CaseId}", caseId);

        // Mock data for testing
        return CreateMockCase($"FWW{caseId.Substring(0, Math.Min(6, caseId.Length))}");
    }

    private TrackingCase CreateMockCase(string trackingNumber)
    {
        var rand = new Random(trackingNumber.GetHashCode());
        var statuses = new[] { "Pending", "In Transit", "Customs Clearance", "Out for Delivery", "Delivered" };
        var statusIndex = rand.Next(statuses.Length);
        
        return new TrackingCase
        {
            CaseId = Guid.NewGuid().ToString("N").Substring(0, 8),
            TrackingNumber = trackingNumber,
            Status = statuses[statusIndex],
            StatusDescription = GetStatusDescription(statuses[statusIndex]),
            Origin = "Shanghai, China",
            Destination = "Los Angeles, USA",
            EstimatedArrival = DateTime.UtcNow.AddDays(rand.Next(1, 10)),
            ActualArrival = statusIndex == 4 ? DateTime.UtcNow.AddDays(-1) : null,
            CreatedOn = DateTime.UtcNow.AddDays(-rand.Next(5, 30)),
            ModifiedOn = DateTime.UtcNow.AddHours(-rand.Next(1, 48)),
            Events = GenerateMockEvents(statusIndex)
        };
    }

    private string GetStatusDescription(string status)
    {
        return status switch
        {
            "Pending" => "Shipment is being prepared for pickup",
            "In Transit" => "Shipment is on its way to the destination",
            "Customs Clearance" => "Shipment is being processed through customs",
            "Out for Delivery" => "Shipment is on the delivery vehicle",
            "Delivered" => "Shipment has been successfully delivered",
            _ => "Status unknown"
        };
    }

    private List<TrackingEvent> GenerateMockEvents(int currentStatusIndex)
    {
        var events = new List<TrackingEvent>();
        var baseDate = DateTime.UtcNow.AddDays(-10);
        
        var eventTemplates = new[]
        {
            ("Shanghai, China", "Pending", "Shipment created and awaiting pickup"),
            ("Shanghai, China", "Picked Up", "Shipment picked up from origin"),
            ("Shanghai Port", "In Transit", "Departed origin port"),
            ("Pacific Ocean", "In Transit", "In transit via ocean freight"),
            ("Los Angeles Port", "Arrived", "Arrived at destination port"),
            ("Los Angeles, USA", "Customs Clearance", "Customs clearance in progress"),
            ("Los Angeles, USA", "Cleared", "Customs cleared"),
            ("Distribution Center", "Out for Delivery", "Shipment out for delivery"),
            ("Los Angeles, USA", "Delivered", "Successfully delivered")
        };

        var eventsToShow = Math.Min(currentStatusIndex + 3, eventTemplates.Length);
        
        for (int i = 0; i < eventsToShow; i++)
        {
            events.Add(new TrackingEvent
            {
                Timestamp = baseDate.AddDays(i).AddHours(i * 3),
                Location = eventTemplates[i].Item1,
                Status = eventTemplates[i].Item2,
                Description = eventTemplates[i].Item3
            });
        }

        events.Reverse(); // Most recent first
        return events;
    }

    // TODO: Implement actual Dataverse authentication and API calls
    // private async Task<string> GetAccessTokenAsync() { ... }
    // private async Task<T?> QueryDataverseAsync<T>(string query) { ... }
}
