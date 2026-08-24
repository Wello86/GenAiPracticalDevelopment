using Lab3.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
#pragma warning disable SKEXP0070
namespace Lab3.Services;

public class HuggingFaceService : IHuggingFaceService
{
    private readonly Kernel _kernel;

    public HuggingFaceService([FromKeyedServices("KernelWithHuggingFace")] Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<string> GetHuggingFaceAnswer(string question)
    {
        var result = await _kernel.InvokePromptAsync(question);
        return result.GetValue<string>() ?? string.Empty;
    }
}
