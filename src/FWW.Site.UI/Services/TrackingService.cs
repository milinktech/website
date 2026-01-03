using System.Net.Http.Json;
using FWW.Site.Shared.Models;

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
    public async Task<PublicTrackingResponse?> GetPublicTrackingAsync(string trackingNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/track/{trackingNumber}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PublicTrackingResponse>();
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
    public async Task<StaffTrackingResponse?> GetStaffTrackingAsync(string caseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/track/staff/{caseId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<StaffTrackingResponse>();
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
