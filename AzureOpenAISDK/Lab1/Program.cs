using System.ClientModel;
using Azure.AI.OpenAI;
using dotenv.net;
using Lab1.Abstractions;
using Lab1.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

DotEnv.Load();

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("OPENAIENDPOINT")!;
var azureOpenAIKey = Environment.GetEnvironmentVariable("OPENAIAPIKEY")!;
var azureOpenAIDeployment = Environment.GetEnvironmentVariable("OPENAIMODEL_DEPLOYMENT")!;
var imageDeployment = Environment.GetEnvironmentVariable("IMAGE_DEPLOYMENT")!;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    
builder.Services.AddTransient<IChatService, ChatService>(_ => new ChatService(
    new AzureOpenAIClient(new Uri(azureOpenAIEndpoint), new ApiKeyCredential(azureOpenAIKey)),
    azureOpenAIDeployment));

builder.Services.AddTransient<IImageService, ImageService>(_ => new ImageService(
    new AzureOpenAIClient(new Uri(azureOpenAIEndpoint), new ApiKeyCredential(azureOpenAIKey)),
    imageDeployment));

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