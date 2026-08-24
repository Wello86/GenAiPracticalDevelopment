using dotenv.net;
using Lab5.Abstractions;
using Lab5.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory.DataFormats;
using Microsoft.KernelMemory.DataFormats.Pdf;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Memory;
#pragma warning disable KMEXP00, SKEXP0001, SKEXP0010, SKEXP0020

DotEnv.Load();

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("OPENAIENDPOINT")!;
var azureOpenAIKey = Environment.GetEnvironmentVariable("OPENAIAPIKEY")!;
var azureOpenAIDeployment = Environment.GetEnvironmentVariable("OPENAIMODEL_DEPLOYMENT")!;
var chromaEndpoint = Environment.GetEnvironmentVariable("CHROMA_ENDPOINT")!;
var embeddingsDeployment = Environment.GetEnvironmentVariable("EMBEDDINGS_DEPLOYMENT")!;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<Kernel>(_ =>
{
    var kernelBuilder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(azureOpenAIDeployment, azureOpenAIEndpoint, azureOpenAIKey);
    return kernelBuilder.Build();
});
builder.Services.AddTransient<ISemanticTextMemory>(_ =>
{
    var memoryBuilder = new MemoryBuilder();
    memoryBuilder.WithTextEmbeddingGeneration(new AzureOpenAITextEmbeddingGenerationService(embeddingsDeployment, azureOpenAIEndpoint, azureOpenAIKey));
    var chromaMemoryStore = new ChromaMemoryStore(chromaEndpoint!);
    memoryBuilder.WithMemoryStore(chromaMemoryStore);
    return memoryBuilder.Build();
});
builder.Services.AddTransient<IEmbeddingService, EmbeddingService>();
builder.Services.AddTransient<IAnswerService, AnswerService>();
builder.Services.AddTransient<IContentDecoder, PdfDecoder>();
using IHost host = builder.Build();


// File name for embedding generation 
string defaultFile = "ExampleTestDocument.pdf";

// Question for the system
string testAnswer = "What do you know about Elon Musk?";

// Get Services
var embeddingService = host.Services.GetRequiredService<IEmbeddingService>();
var answerService = host.Services.GetRequiredService<IAnswerService>();

// Create Chroma collection for storing embeddings
var client = new ChromaClient(chromaEndpoint!);
await client.CreateCollectionAsync("content");

// Generate embeddings
await embeddingService.GenerateEmbeddingsAsync(defaultFile);

// Answer the question based on the embeddings
var answer = await answerService.AnswerToQuestionAsync(testAnswer);

Console.WriteLine(answer);

return 0;
