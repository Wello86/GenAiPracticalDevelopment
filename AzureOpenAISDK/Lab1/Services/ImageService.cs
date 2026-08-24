using Azure.AI.OpenAI;
using Lab1.Abstractions;
using OpenAI.Images;

namespace Lab1.Services;

public class ImageService(AzureOpenAIClient azureOpenAIClient, string deploymentName) : IImageService
{
    public async Task<string?> GenerateImageAsync(string prompt)
    {
        var imageGenerationOptions = new ImageGenerationOptions()
        {
            Quality = "high"
        };
        var imageClient = azureOpenAIClient.GetImageClient(deploymentName);
        var response = await imageClient.GenerateImagesAsync(prompt, 1, imageGenerationOptions);
        BinaryData bytes = response?.Value?.FirstOrDefault()?.ImageBytes;
        var fileName = $"{Guid.NewGuid()}.png";
        await File.WriteAllBytesAsync(fileName, bytes.ToArray());
        return fileName;
    }
}