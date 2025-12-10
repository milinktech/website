using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FWW.Site.UI.Services;

/// <summary>
/// Client service for Chat API calls
/// </summary>
public class ChatService
{
    private readonly HttpClient _httpClient;
    private string? _sessionId;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Send a message to the AI chat agent
    /// </summary>
    public async Task<ChatApiResponse> SendMessageAsync(string message, List<ChatHistoryItem>? history = null)
    {
        try
        {
            var request = new ChatApiRequest
            {
                Message = message,
                History = history?.Select(h => new ChatApiMessage
                {
                    Role = h.IsUser ? "user" : "assistant",
                    Content = h.Text
                }).ToList() ?? new List<ChatApiMessage>(),
                SessionId = _sessionId
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null)
                {
                    _sessionId = result.SessionId;
                    return result;
                }
            }

            // Fallback on error
            return new ChatApiResponse
            {
                Message = "I'm having trouble connecting right now. Please try again in a moment.",
                Success = false
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chat error: {ex.Message}");
            return new ChatApiResponse
            {
                Message = "I'm sorry, something went wrong. Please try again.",
                Success = false
            };
        }
    }
}

/// <summary>
/// Request to chat API
/// </summary>
public class ChatApiRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatApiMessage> History { get; set; } = new();
    public string? SessionId { get; set; }
}

/// <summary>
/// Message in chat history for API
/// </summary>
public class ChatApiMessage
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Response from chat API
/// </summary>
public class ChatApiResponse
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}

/// <summary>
/// Chat history item for frontend display
/// </summary>
public class ChatHistoryItem
{
    public string Text { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Time { get; set; }
}
