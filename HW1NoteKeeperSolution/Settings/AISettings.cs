namespace HW1NoteKeeperSolution.Settings
{
    /// <summary>
    /// Contains settings for AI service configuration.
    /// </summary>
    public class AISettings
    {
        public string DeploymentUri { get; set; }
        public string ApiKey { get; set; }
        public string DeploymentName { get; set; } = "gpt-5-mini";
        public float Temperature { get; set; } = 1.0f;
        public float TopP { get; set; } = 1.0f;
        public int MaxOutputTokens { get; set; } = 5500;
    }
}