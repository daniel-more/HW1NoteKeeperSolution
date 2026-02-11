
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using HW1NoteKeeperSolution.Settings;
using System.Text.Json.Schema;
using System.Net.Mime;
using System.Text.Json;
using NJsonSchema;


namespace HW1NoteKeeperSolution

{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {

        public class KeyPhrasesResponse
        {
            public List<string> Phrases { get; set; } = [];
        }

        private readonly ILogger<ChatController> _logger;
        private readonly IChatClient _chatClient;
        private readonly AISettings _aISettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatClient">The chat client to interact with the AI model.</param>
        /// <param name="aISettings">The settings for the AI model.</param>
        /// <param name="logger">The logger instance for logging.</param>
        public ChatController(IChatClient chatClient,
                              AISettings aISettings,
                              ILogger<ChatController> logger)
        {
            _logger = logger;
            _chatClient = chatClient;
            _aISettings = aISettings;
        }


        /// <summary>
        /// Post a chat message to the AI model and get a response.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the AI model.</param>
        /// <returns>The response from the AI model as a string.</returns>
        [HttpPost(Name = "PostChat")]
        public async Task<string> Post([FromBody] string prompt)
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

            // Send the prompt to the AI model and get the response
            var responseCompletion = await _chatClient.GetResponseAsync(prompt, chatOptions);

            // Return the response text or a default message if the response is null            
            return responseCompletion?.Text ?? "<Unable to respond>";

        }

        /// <summary>
        /// Uses a system message to provide context to the AI model.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the AI model.</param>
        /// <param name="chatOptions">The chat options to configure the AI model.</param>
        /// <returns>The response from the AI model.</returns>
        private async Task<ChatResponse> Demo02SystemPromptToProvideContext(string prompt, ChatOptions chatOptions)
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            // messages.Add(new ChatMessage(ChatRole.System, "You will identify and return a JSON list of the most important 3 key words from the users input"));
            messages.Add(new ChatMessage(ChatRole.System, "Extract exactly 3 important keywords from the user's input. Return them only as a JSON array of strings, nothing else."));

            messages.Add(new ChatMessage(ChatRole.User, prompt));
            // Send the prompt to the AI model and get the response
            ChatResponse responseCompletion = await _chatClient.GetResponseAsync(messages, options: chatOptions);
            return responseCompletion;
        }
    }
}