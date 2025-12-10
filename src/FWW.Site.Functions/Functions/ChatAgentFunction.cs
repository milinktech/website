using FWW.Site.Functions.Models;
using FWW.Site.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FWW.Site.Functions.Functions;

/// <summary>
/// Chat Agent endpoint for AI-powered customer support
/// </summary>
public class ChatAgentFunction
{
    private readonly ILogger<ChatAgentFunction> _logger;
    private readonly AiFoundryService _aiService;

    public ChatAgentFunction(ILogger<ChatAgentFunction> logger, AiFoundryService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    [Function("ChatAgent")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat")] HttpRequest req)
    {
        _logger.LogInformation("Chat agent request received");

        try
        {
            // Read request body
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return new BadRequestObjectResult(new ChatResponse
                {
                    Success = false,
                    Error = "Request body is required",
                    Message = ""
                });
            }

            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (chatRequest == null || string.IsNullOrWhiteSpace(chatRequest.Message))
            {
                return new BadRequestObjectResult(new ChatResponse
                {
                    Success = false,
                    Error = "Message is required",
                    Message = ""
                });
            }

            // Get AI response
            var response = await _aiService.GetChatResponseAsync(chatRequest);

            return new OkObjectResult(response);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in chat request");
            return new BadRequestObjectResult(new ChatResponse
            {
                Success = false,
                Error = "Invalid request format",
                Message = ""
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return new ObjectResult(new ChatResponse
            {
                Success = false,
                Error = "An error occurred processing your request",
                Message = ""
            })
            {
                StatusCode = 500
            };
        }
    }
}
