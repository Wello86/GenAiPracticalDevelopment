using Azure.AI.OpenAI;
using Lab1.Abstractions;
using OpenAI.Chat;
#pragma warning disable AOAI001

namespace Lab1.Services;

public class ChatService(AzureOpenAIClient azureOpenAIClient, string deploymentName) : IChatService
{
    private readonly ChatCompletionOptions _chatCompletionsOptions = new()
    {
        Temperature = 0.0f,      // or 0.1f — as deterministic as possible
        TopP = 1.0f,             // let temperature control it, don't compound
        FrequencyPenalty = 0.0f, // don't discourage repeating correct terms
        PresencePenalty = 0.0f,  // don't push toward introducing new terms
    };

    public async Task<string?> SendMessageAsync(string prompt)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage("You are answering factual Q&A."),
            new UserChatMessage(prompt)
        ];
        var chatClient = azureOpenAIClient.GetChatClient(deploymentName);
        var msg = await chatClient.CompleteChatAsync(messages, _chatCompletionsOptions);
        return msg?.Value?.Content?.FirstOrDefault()?.Text;
    }
}