using Lab2.Abstractions;
using Microsoft.SemanticKernel;

namespace Lab2.Services;

public class ChatService(Kernel kernel) : IChatService
{
    private readonly PromptExecutionSettings _executionSettings = new()
    {
        ExtensionData = new Dictionary<string, object>
        {
            { "MaxTokens", 500 },
            { "Temperature", 0.7f }
        }
    };

    public async Task<string?> SendMessageAsync(string prompt)
    {
        var arguments = new KernelArguments(_executionSettings);

        var result = await kernel.InvokePromptAsync(prompt, arguments);

        return result.GetValue<string>();
    }
}