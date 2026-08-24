using dotenv.net;
using Lab2.Abstractions;
using Lab2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
#pragma warning disable KMEXP00, SKEXP0001, SKEXP0010, SKEXP0020

DotEnv.Load();

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("OPENAIENDPOINT")!;
var azureOpenAIKey = Environment.GetEnvironmentVariable("OPENAIAPIKEY")!;
var azureOpenAIDeployment = Environment.GetEnvironmentVariable("OPENAIMODEL_DEPLOYMENT")!;
var imageDeployment = Environment.GetEnvironmentVariable("IMAGE_DEPLOYMENT")!;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<Kernel>(_ => Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(azureOpenAIDeployment, azureOpenAIEndpoint, azureOpenAIKey)
    .AddAzureOpenAITextToImage(imageDeployment, azureOpenAIEndpoint, azureOpenAIKey)
    .Build());
    
builder.Services.AddTransient<IChatService, ChatService>(services =>
{
    var kernel = services.GetRequiredService<Kernel>();
    return new ChatService(kernel);
});

builder.Services.AddTransient<IImageService, ImageService>(services =>
{
    var kernel = services.GetRequiredService<Kernel>();
    return new ImageService(kernel);
});

using IHost host = builder.Build();

// Get Services
var chatService = host.Services.GetRequiredService<IChatService>();

// Question for the system
var testQuestion = "What do you know about Elon Musk?";

// Answer the question
var answer = await chatService.SendMessageAsync(testQuestion);

Console.WriteLine(answer);

// Prompt for image generation
var testPrompt = "Generate image where cats play with a books";

// Get Services
var imageService = host.Services.GetRequiredService<IImageService>();

// Ask to generate image and get the URI
var imageUri = await imageService.GenerateImageAsync(testPrompt);

Console.WriteLine(imageUri);

return 0;