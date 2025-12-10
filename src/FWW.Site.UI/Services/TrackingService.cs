using System.Net.Http.Json;

namespace FWW.Site.UI.Services;

/// <summary>
/// Client service for tracking API calls
/// </summary>
public class TrackingService
{
    private readonly HttpClient _httpClient;

    public TrackingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get tracking info by tracking number (public)
    /// </summary>
    public async Task<TrackingResult?> GetPublicTrackingAsync(string trackingNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/track/{trackingNumber}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TrackingResult>();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tracking error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get tracking info by case ID (staff only)
    /// </summary>
    public async Task<StaffTrackingResult?> GetStaffTrackingAsync(string caseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/track/staff/{caseId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StaffTrackingResult>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Staff tracking error: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// Tracking result model for public users
/// </summary>
public class TrackingResult
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime? EstimatedArrival { get; set; }
    public List<TrackingEvent> Events { get; set; } = new();
}

/// <summary>
/// Tracking result model for staff
/// </summary>
public class StaffTrackingResult : TrackingResult
{
    public string CaseId { get; set; } = string.Empty;
    public DateTime? ActualArrival { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string InternalNotes { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}

/// <summary>
/// Tracking event/milestone
/// </summary>
public class TrackingEvent
{
    public DateTime Timestamp { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
