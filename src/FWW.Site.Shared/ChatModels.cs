namespace FWW.Site.Shared.Models;

/// <summary>
/// Chat request from frontend
/// </summary>
public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessage> History { get; set; } = new();
    public string? SessionId { get; set; }
}

/// <summary>
/// Chat message in history
/// </summary>
public class ChatMessage
{
    public string Role { get; set; } = "user"; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Chat response from AI agent
/// </summary>
public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}
