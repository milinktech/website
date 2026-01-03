using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FWW.Site.Shared.Models;

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
    public async Task<ChatResponse> SendMessageAsync(string message, List<ChatHistoryItem>? history = null)
    {
        try
        {
            var request = new ChatRequest
            {
                Message = message,
                History = history?.Select(h => new ChatMessage
                {
                    Role = h.IsUser ? "user" : "assistant",
                    Content = h.Text
                }).ToList() ?? new List<ChatMessage>(),
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
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>(new JsonSerializerOptions
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
            return new ChatResponse
            {
                Message = "I'm having trouble connecting right now. Please try again in a moment.",
                Success = false
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chat error: {ex.Message}");
            return new ChatResponse
            {
                Message = "I'm sorry, something went wrong. Please try again.",
                Success = false
            };
        }
    }
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
