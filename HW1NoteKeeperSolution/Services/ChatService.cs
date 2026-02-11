using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using HW1NoteKeeperSolution.Settings;
using System.Text.Json.Schema;
using System.Net.Mime;
using System.Text.Json;
using NJsonSchema;

public interface IChatService
{
    Task<List<string>> ProcessMessage(string message);

}
public class ChatService : IChatService


{
    public class KeyPhrasesResponse
    {
        public List<string> Phrases { get; set; } = [];
    }
    // You can inject other dependencies here if needed
    private readonly ILogger<ChatService> _logger;
    private readonly IChatClient _chatClient;
    private readonly AISettings _aISettings;



    public ChatService(ILogger<ChatService> logger, IChatClient chatClient, AISettings aISettings)
    {
        _logger = logger;
        _chatClient = chatClient;
        _aISettings = aISettings;
    }


    public async Task<List<string>> ProcessMessage(string message)
    {
        // Add a Json schema for the output prompt
        JsonSchema schema = JsonSchema.FromType<KeyPhrasesResponse>();
        string jsonSchemaString = schema.ToJson();

        JsonElement jsonSchemaElement = JsonDocument.Parse(jsonSchemaString).RootElement;

        ChatResponseFormatJson chatResponseFormatJson = ChatResponseFormat.ForJsonSchema(jsonSchemaElement, "ChatResponse", "Chat response schema");

        // Create chat options using settings from AISettings
        ChatOptions chatOptions = new ChatOptions()
        {
            Temperature = _aISettings.Temperature, // Controls the randomness of the response
            TopP = _aISettings.TopP, // Controls the diversity of the response
            MaxOutputTokens = _aISettings.MaxOutputTokens, // Maximum number of tokens in the response
            ResponseFormat = chatResponseFormatJson // Set the response format to the defined JSON schema
        };

        // Use system message to provide context to the AI model
        List<ChatMessage> messages = new List<ChatMessage>();
        messages.Add(new ChatMessage(ChatRole.System, "Extract exactly 3 important keywords from the user's input. Return them only as a JSON array of strings, nothing else."));
        messages.Add(new ChatMessage(ChatRole.User, message));

        // Send the prompt to the AI model and get the response
        var responseCompletion = await _chatClient.GetResponseAsync(messages, chatOptions);

        // Parse the JSON response and extract the phrases
        try
        {
            var jsonDoc = JsonDocument.Parse(responseCompletion?.Text ?? "{}");
            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("Phrases", out var phrasesElement) && phrasesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                return phrasesElement.EnumerateArray().Select(p => p.GetString()).ToList();
            }
        }
        catch
        {
            _logger.LogWarning("Failed to parse JSON response from AI model");
        }

        return [];
    }
}