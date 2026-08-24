using Lab4.Abstractions;
using Lab4.SKPlugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.Web;
#pragma warning disable SKEXP0050

namespace Lab4.Services;

public class DataSummaryService : IDataSummaryService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    private const string SystemPrompt = """
        You are an assistant tasked with finding and summorizing data.

        **Instructions**:
        - First, for given a topic, find brief, relevant information from the web.
        - Summarize the key points into a short paragraph.
        - Get the current date and time.
        - Insert the retrieved date and time into the summary in a formatted manner.
        - Translate this final, formatted summary into the specified language.
        
        **Constraints**:
        - Do not provide any implementation details.
        - Return only the final translated summary as your answer.
        - Do not include any additional commentary or explanation.
    """;

    public DataSummaryService(Kernel kernel, IWebSearchEngineConnector searchConnector)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));

        // Please register TimePlugin, WebSearchEnginePlugin and CustomPlugin

        _kernel.ImportPluginFromObject(new TimePlugin(), "time");
        _kernel.ImportPluginFromObject(new WebSearchEnginePlugin(searchConnector), "webSearch");
        _kernel.ImportPluginFromObject(new CustomPlugin(_kernel), "custom");

        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> SummarizeTopicAsync(string topic, string language)
    {
        var chatHistory = new ChatHistory(SystemPrompt);

        chatHistory.AddUserMessage(
            $"Topic: {topic}\nTarget language: {language}");

        var settings = new OpenAIPromptExecutionSettings
        {
            // Lets the model decide which registered plugin functions to invoke,
            // and in what order/how many times, to satisfy the system prompt.
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        ChatMessageContent result = await _chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: settings,
            kernel: _kernel);

        return result.Content ?? string.Empty;
    }
}

