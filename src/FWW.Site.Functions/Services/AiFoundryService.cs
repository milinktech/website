using FWW.Site.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FWW.Site.Functions.Services;

/// <summary>
/// Service for interacting with Azure AI Foundry Agent
/// </summary>
public class AiFoundryService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiFoundryService> _logger;

    // Configuration keys
    private const string AI_ENDPOINT_KEY = "AzureAiFoundry:Endpoint";
    private const string AI_API_KEY = "AzureAiFoundry:ApiKey";
    private const string AI_DEPLOYMENT_KEY = "AzureAiFoundry:DeploymentName";

    // System prompt for the logistics assistant
    private const string SystemPrompt = @"You are FOCUS Assistant, the helpful AI assistant for FOCUS Logistics, 
a global logistics and freight company. You help customers with:
- Tracking shipments (guide them to the Track page)
- Getting quotes (guide them to the Quote page)
- Information about services (Ocean Freight, Air Freight, Land Transport, Warehousing)
- General inquiries about shipping and logistics

Be professional, friendly, and concise. If you don't know something specific about their shipment, 
guide them to contact the team or use the relevant page on the website.

Keep responses brief and helpful - aim for 2-3 sentences when possible.";

    public AiFoundryService(HttpClient httpClient, IConfiguration configuration, ILogger<AiFoundryService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get a chat response from the AI agent
    /// </summary>
    public async Task<ChatResponse> GetChatResponseAsync(ChatRequest request)
    {
        try
        {
            var endpoint = _configuration[AI_ENDPOINT_KEY];
            var apiKey = _configuration[AI_API_KEY];
            var deploymentName = _configuration[AI_DEPLOYMENT_KEY] ?? "gpt-4o";

            // If no configuration, return fallback response
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Azure AI Foundry not configured, returning fallback response");
                return GetFallbackResponse(request.Message);
            }

            // Build messages array
            var messages = new List<object>
            {
                new { role = "system", content = SystemPrompt }
            };

            // Add history
            foreach (var msg in request.History.TakeLast(10)) // Limit history
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }

            // Add current message
            messages.Add(new { role = "user", content = request.Message });

            // Build request body
            var requestBody = new
            {
                model = deploymentName,
                messages = messages,
                max_tokens = 500,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Set up request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            var response = await _httpClient.PostAsync($"{endpoint}/openai/deployments/{deploymentName}/chat/completions?api-version=2024-02-15-preview", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("AI API error: {StatusCode} - {Error}", response.StatusCode, error);
                return GetFallbackResponse(request.Message);
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            
            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new ChatResponse
            {
                Message = messageContent ?? "I apologize, but I couldn't generate a response. Please try again.",
                Success = true,
                SessionId = request.SessionId ?? Guid.NewGuid().ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI response");
            return GetFallbackResponse(request.Message);
        }
    }

    /// <summary>
    /// Fallback response when AI is not configured or fails
    /// </summary>
    private ChatResponse GetFallbackResponse(string userMessage)
    {
        var lowerInput = userMessage.ToLowerInvariant();
        string response;

        if (lowerInput.Contains("track") || lowerInput.Contains("shipment"))
            response = "To track your shipment, please visit our Track page and enter your tracking number. Would you like me to guide you there?";
        else if (lowerInput.Contains("quote") || lowerInput.Contains("price") || lowerInput.Contains("cost"))
            response = "I'd be happy to help you get a quote! Please visit our Quote page or I can connect you with our sales team.";
        else if (lowerInput.Contains("service"))
            response = "We offer Ocean Freight, Air Freight, Land Transport, and Warehouse Solutions. Visit our Services page for more details!";
        else if (lowerInput.Contains("contact") || lowerInput.Contains("phone") || lowerInput.Contains("email"))
            response = "You can reach us at contact@focuslogistics.com or visit our Contact page.";
        else if (lowerInput.Contains("hello") || lowerInput.Contains("hi") || lowerInput.Contains("hey"))
            response = "Hello! Welcome to FOCUS Logistics. How can I assist you with your shipping needs today?";
        else
            response = "Thank you for your message! I'm here to help with shipping inquiries, quotes, and tracking. What would you like to know?";

        return new ChatResponse
        {
            Message = response,
            Success = true,
            SessionId = Guid.NewGuid().ToString()
        };
    }
}
