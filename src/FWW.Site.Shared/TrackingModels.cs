namespace FWW.Site.Shared.Models;

/// <summary>
/// Represents a tracking case from Dataverse
/// </summary>
public class TrackingCase
{
    public string CaseId { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusDescription { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime? EstimatedArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public List<TrackingEvent> Events { get; set; } = new();
}

/// <summary>
/// Represents a tracking event/milestone
/// </summary>
public class TrackingEvent
{
    public DateTime Timestamp { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Public tracking response (limited data)
/// </summary>
public class PublicTrackingResponse
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
/// Staff tracking response (full data)
/// </summary>
public class StaffTrackingResponse : PublicTrackingResponse
{
    public string CaseId { get; set; } = string.Empty;
    public DateTime? ActualArrival { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string InternalNotes { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
}
