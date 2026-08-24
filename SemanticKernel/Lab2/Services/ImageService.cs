using Lab2.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using OpenAI.Images;
#pragma warning disable KMEXP00, SKEXP0001, SKEXP0010, SKEXP0020

namespace Lab2.Services;

public class ImageService(Kernel kernel) : IImageService
{
    public async Task<string> GenerateImageAsync(string prompt)
    {
        // Use IMAGE_DEPLOYMENT=gpt-image-2 (DALL-E 3 is retired) to generate an image. OpenAI in this case
        // returns a URL (though you can ask to return a base64 image)

        var imageService = kernel.GetRequiredService<ITextToImageService>();

        var settings = new OpenAITextToImageExecutionSettings
        {
            Size = (1024, 1024)
        };

        // Note: gpt-image-2 model does not support returning URLs only base64
        var images = await imageService.GetImageContentsAsync(prompt, settings, kernel);

        var dataUri = images[0].DataUri
            ?? throw new InvalidOperationException("No image content returned.");

        return dataUri;
    }
}