namespace HW1NoteKeeperSolution.Settings
{
    /// <summary>
    /// Contains settings for AI service configuration.
    /// </summary>
    public class AISettings
    {
        /// <summary>
        /// Gets or sets the deployment URI for the Azure OpenAI service.
        /// </summary>
        /// <remarks>
        /// This is the endpoint URL where the AI model is deployed.
        /// Example: https://your-resource.openai.azure.com/
        /// </remarks>
        public required string DeploymentUri { get; set; }

        /// <summary>
        /// Gets or sets the API key for authenticating with the Azure OpenAI service.
        /// </summary>
        /// <remarks>
        /// This key should be kept secure and not exposed in version control.
        /// It is typically stored in environment variables or secure configuration.
        /// </remarks>
        public required string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the deployed model.
        /// </summary>
        /// <remarks>
        /// This is the deployment name in Azure OpenAI, not the model family.
        /// Default value is "gpt-5-mini".
        /// </remarks>
        public string DeploymentModelName { get; set; } = "gpt-5-mini";

        /// <summary>
        /// Gets or sets the temperature parameter for controlling response randomness.
        /// </summary>
        /// <remarks>
        /// Valid range is 0 to 2. Lower values (closer to 0) make responses more deterministic,
        /// while higher values make responses more random and creative.
        /// Default value is 1.0f (neutral randomness).
        /// </remarks>
        public float Temperature { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the Top P (nucleus sampling) parameter for controlling response diversity.
        /// </summary>
        /// <remarks>
        /// Valid range is 0 to 1. Lower values make responses more focused on likely outputs,
        /// while higher values increase diversity by considering less likely options.
        /// Default value is 1.0f (include all tokens).
        /// </remarks>
        public float TopP { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the maximum number of tokens in the AI response.
        /// </summary>
        /// <remarks>
        /// This limits the length of the generated response. Each token is approximately 4 characters.
        /// Default value is 5500 tokens.
        /// </remarks>
        public int MaxOutputTokens { get; set; } = 5500;
    }
}