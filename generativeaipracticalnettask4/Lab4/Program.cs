using Azure.AI.OpenAI;
using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using System.ClientModel;
using Lab4.Abstractions;
using Lab4.Services;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using Microsoft.SemanticKernel.Plugins.Web;
#pragma warning disable SKEXP0050

DotEnv.Load();

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("OPENAIENDPOINT")!;
var azureOpenAIKey = Environment.GetEnvironmentVariable("OPENAIAPIKEY")!;
var azureOpenAIDeployment = Environment.GetEnvironmentVariable("OPENAIMODEL_DEPLOYMENT")!;
var googleSearchApiKey = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY")!;
var googleSearchEngineId = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_ENGINE_ID")!;


// Build host and configure DI
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Register the Semantic Kernel and its Azure OpenAI integration
builder.Services.AddTransient<Kernel>(_ =>
{
    var kernelBuilder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(azureOpenAIDeployment, azureOpenAIEndpoint, azureOpenAIKey);
    var kernel = kernelBuilder.Build();
    return kernel;
});

// Register the AzureOpenAIClient for direct OpenAI SDK usage
builder.Services.AddSingleton<AzureOpenAIClient>(_ =>
{
    return new AzureOpenAIClient(
         new Uri(azureOpenAIEndpoint),
         new ApiKeyCredential(azureOpenAIKey));
});

// Register the GoogleConnector for direct Google search usage
builder.Services.AddTransient<IWebSearchEngineConnector>(_ =>
{
    return new GoogleConnector(googleSearchApiKey, googleSearchEngineId);
});

// Register our service that uses the OpenAI SDK directly (no SK), for simple prompt formatting
builder.Services.AddTransient<IUserPromtFormatterService>(sp =>
{
    var azureClient = sp.GetRequiredService<AzureOpenAIClient>();
    return new UserPromtFormatterService(azureClient, azureOpenAIDeployment);
});

// Register our service that uses SK and a more complex scenario (reading, summarizing, formatting, translating)
builder.Services.AddTransient<IDataSummaryService, DataSummaryService>();

using IHost host = builder.Build();

// -----------------------------------------------------------------------------
// First approach: Simple prompt formatting using the plain OpenAI .NET SDK.
// This uses IUserPromtFormatterService and demonstrates a direct function-calling flow.
// -----------------------------------------------------------------------------
var formatter = host.Services.GetRequiredService<IUserPromtFormatterService>();
string formattedResult = await formatter.ProcessUserTextAsync("Hello World!");
Console.WriteLine("=== Plain OpenAI SDK approach (IUserPromtFormatterService) ===");
Console.WriteLine(formattedResult);

// -----------------------------------------------------------------------------
// Second approach: Complex scenario using Semantic Kernel (IDataSummaryService).
// This demonstrates reading a conversation, summarizing, formatting with date/time,
// and translating into a target language (French).
// -----------------------------------------------------------------------------
var processor = host.Services.GetRequiredService<IDataSummaryService>();

string topic = "Uzbek plov";
string language = "French";
string processedResult = await processor.SummarizeTopicAsync(topic, language);

Console.WriteLine("=== Semantic Kernel approach (IConversationProcessorService) ===");
Console.WriteLine(processedResult);
