using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using HW1NoteKeeperSolution.Settings;
using System.Text.Json.Schema;
using System.Net.Mime;
using System.Text.Json;
using NJsonSchema;


namespace HW1NoteKeeperSolution.Controllers
{
    /// <summary>
    /// Handles chat-related API requests.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        /// <summary>
        /// Response model for key phrases returned by the AI.
        /// </summary>
        public class KeyPhrasesResponse
        {
            /// <summary>
            /// Gets or sets the list of key phrases extracted by the AI.
            /// </summary>
            public List<string> Phrases { get; set; } = [];
        }

        private readonly ILogger<ChatController> _logger;
        private readonly IChatClient _chatClient;
        private readonly AISettings _aISettings;
        private readonly IChatService _chatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatClient">The chat client to interact with the AI model.</param>
        /// <param name="aISettings">The settings for the AI model.</param>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="chatService">The chat service for processing messages.</param>
        public ChatController(IChatClient chatClient,
                              AISettings aISettings,
                              ILogger<ChatController> logger,
                              IChatService chatService)
        {
            _logger = logger;
            _chatClient = chatClient;
            _aISettings = aISettings;
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        }

        /// <summary>
        /// HTTP POST endpoint to send a chat prompt to the AI model and receive extracted keywords.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the AI model.</param>
        /// <returns>A list of key phrases extracted from the prompt by the AI model.</returns>
        [HttpPost(Name = "PostChat")]
        public async Task<List<string>> Post([FromBody] string prompt)
        {
            // Use the chat service to process the prompt and extract keywords
            var result = await _chatService.ProcessMessage(prompt);
            // Return the result or an empty list if no keywords were found
            return result ?? [];
        }
    }
}