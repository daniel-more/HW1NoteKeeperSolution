
using HW1NoteKeeperSolution.Settings;
using Microsoft.OpenApi;
using System.Reflection;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;



namespace HW1NoteKeeperSolution
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application that configures and runs the web host.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // for the logger, we need to create a LoggerFactory and then create a logger from it
            var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }
        );

            // Add services to the container.

            builder.Services.AddControllers();

            //Bind AISettings to the appsettings.json file
            AISettings? _aiSettings = builder.Configuration.GetSection("AISettings").Get<AISettings>()!;


            ILogger logger = loggerFactory.CreateLogger("Program");

            if (_aiSettings is null
                || string.IsNullOrWhiteSpace(_aiSettings.DeploymentUri)
                || string.IsNullOrWhiteSpace(_aiSettings.ApiKey))
            {

                if (_aiSettings == null)
                {
                    logger.LogCritical("AISettings is null. Please ensure the configuration is present.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_aiSettings.DeploymentUri))
                    {
                        logger.LogCritical("AISettings.DeploymentUri is null or empty.");
                    }
                    if (string.IsNullOrWhiteSpace(_aiSettings.ApiKey))
                    {
                        logger.LogCritical("AISettings.ApiKey is null or empty.");
                    }
                }
                throw new InvalidOperationException("AISettings validation failed. Check the logs for details.");
            }

            logger.LogInformation("AISettings successfully loaded from configuration.");

            builder.Services.AddSingleton(_aiSettings!);
            builder.Services.AddScoped<IChatService, ChatService>();  //AddScoped means a new instance of ChatService will be created for each HTTP request, and shared within that request. This is a good choice for services that are lightweight and don't maintain state across requests.

            // Initialize the OpenAIClient with the settings
            Uri openAIServiceEndpointUri;
            AzureKeyCredential openAIServiceCredential;
            openAIServiceEndpointUri = new Uri(_aiSettings.DeploymentUri);
            openAIServiceCredential = new AzureKeyCredential(_aiSettings.ApiKey);

            // RegisterOpenAIClient(openAIServiceEndpointUri, openAIServiceCredential, builder.Services);
            RegisterOpenAIClient(builder, openAIServiceEndpointUri, openAIServiceCredential, _aiSettings.DeploymentName);



            // Note: Don't forget to add this to the csproj file (removing /* and */)
            // Or use the Visual Studio UI settings do do this
            // See slide 48-56 in AzureAppService-01.pptx
            /*
                <!-- DEMO: Enable documentation file so API has user defined documentation via XML comments -->
                <PropertyGroup>
                    <GenerateDocumentationFile>true</GenerateDocumentationFile>

                <!-- Disable warning: Missing XML comment for publicly visible type or member 'Type_or_Member'-->
                <NoWarn>1701;1702;1591</NoWarn>
                </PropertyGroup>
            */

            // Add Swagger generation with XML comments
            builder.Services.AddSwaggerGen(c =>
            {
                // Add nice title
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Notes Testing", Version = "v1" });

                // Add documentation via C# XML Comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Only include XML comments if the file exists
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

            });



            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }

            // Code Note: Moved outside of env.IsDevelopment() so both 
            // Debug and Release are supported
            app.UseSwagger();

            // Customize the UseSwaggerUI() 
            app.UseSwaggerUI(c =>
            {
                // 1. Display a friendly title
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast Testing API V1");

                // Code Note: 
                // Launch the Swagger UI by default
                // Serving the Swagger UI at the app's root 
                // (http://localhost:<port>)
                c.RoutePrefix = string.Empty;
            });

            app.MapOpenApi();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


        /// <summary>
        /// Registers the OpenAI client with the specified parameters.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        /// <param name="openAIServiceEndpointUri">The OpenAI service endpoint URI.</param>
        /// <param name="apiKeyCredential">The API key credential.</param>
        /// <param name="deploymentName">The deployment name.</param>
        private static void RegisterOpenAIClient(WebApplicationBuilder builder,
                                             Uri openAIServiceEndpointUri,
                                             AzureKeyCredential apiKeyCredential,
                                             string deploymentName)
        {
            // Register the OpenAI client as a singleton service
            // A singleton service is created once and shared throughout the application's lifetime
            builder.Services.AddSingleton<IChatClient>(services =>
            {
                var azureOpenAiClient = new AzureOpenAIClient(openAIServiceEndpointUri, apiKeyCredential);
                var chatClient = azureOpenAiClient.GetChatClient(deploymentName);
                return chatClient.AsIChatClient();
            });
        }

    }

}

