using dotenv.net;
using Lab3.Abstractions;
using Lab3.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0070

DotEnv.Load();

var huggingFaceModel = Environment.GetEnvironmentVariable("HUGGING_FACE_MODEL")!;
var huggingFaceKey = Environment.GetEnvironmentVariable("HUGGING_FACE_KEY")!;
var ollamaEndpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT")!;
var ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL")!;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddKeyedTransient<Kernel>("KernelWithHuggingFace", (sp, key) =>
{
    var kernelBuilder = Kernel.CreateBuilder()
         .AddOpenAIChatCompletion(
             modelId: huggingFaceModel,
             apiKey: huggingFaceKey,
             endpoint: new Uri("https://router.huggingface.co/v1"));
    return kernelBuilder.Build();
});

builder.Services.AddKeyedTransient<Kernel>("KernelWithOllama", (sp, key) =>
{
    var kernelBuilder = Kernel.CreateBuilder()
        .AddOllamaTextGeneration(ollamaModel, new Uri(ollamaEndpoint));
    return kernelBuilder.Build();
});

builder.Services.AddTransient<IHuggingFaceService, HuggingFaceService>();
builder.Services.AddTransient<IOllamaService, OllamaService>();
using IHost host = builder.Build();

// Prepare question
var question = "What is the best rock band of all time?";
// Get HuggingFace Services
var huggingFaceService = host.Services.GetRequiredService<IHuggingFaceService>();
var huggingFaceAnswer = await huggingFaceService.GetHuggingFaceAnswer(question);
Console.WriteLine("HuggingFace model answer: " + huggingFaceAnswer);

// Get Llama Services
var selfHostedModelService = host.Services.GetRequiredService<IOllamaService>();
var selfHostedModelAnswer = await selfHostedModelService.GetSelfHostedAnswer(question);
Console.WriteLine("SelfHosted Model Answer: " + selfHostedModelAnswer);





