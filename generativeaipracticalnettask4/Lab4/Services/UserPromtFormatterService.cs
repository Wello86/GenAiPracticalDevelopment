using Azure.AI.OpenAI;
using Lab4.Abstractions;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text.Json;
using OpenAIChatFinishReason = OpenAI.Chat.ChatFinishReason;
using OpenAIChatMessage = OpenAI.Chat.ChatMessage;

namespace Lab4.Services;

public class UserPromtFormatterService : IUserPromtFormatterService   
{
    private readonly AzureOpenAIClient _azureClient;
    private readonly ChatClient _chatClient;
    private readonly string _azureOpenAIDeployment;

    public UserPromtFormatterService(AzureOpenAIClient azureClient, string azureOpenAIDeployment)
    {
        _azureClient = azureClient ?? throw new ArgumentNullException(nameof(azureClient));
        _azureOpenAIDeployment = azureOpenAIDeployment ?? throw new ArgumentNullException(nameof(azureOpenAIDeployment));
        _chatClient = _azureClient.GetChatClient(_azureOpenAIDeployment);
    }

    private const string SystemPrompt = """
        You are an assistant that must strictly follow these steps:
        - When the user provides some text, first determine the current time.
        - Use that time to produce a formatted version of the user’s text.
        - Return only the final formatted text, with no explanations or additional commentary.
    
        Do not explain what you are doing. Do not return the internal steps. Return only the final result.
    """;

    private List<ChatTool> Tools =>
    [
        ChatTool.CreateFunctionTool(
            functionName: "get_time",
            functionDescription: "Get the current UTC time.",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {},
                "required": []
            }
            """)
        ),
        ChatTool.CreateFunctionTool(
            functionName: "format_text",
            functionDescription: "Format the given text with the provided time.",
            functionParameters: BinaryData.FromString("""
            {
                "type": "object",
                "properties": {
                    "text": { "type": "string" },
                    "time": { "type": "string" }
                },
                "required": ["text", "time"]
            }
            """)
        )
    ];

    /// <summary>
    /// This method should:
    /// Prepare ChatCompletionOptions with tools, and a list of ChatMessage with system and user messages.
    /// Call the LLM in a loop until the stop reason is not function calling.
    /// When the LLM requires a function call, invoke the required function and return data to the LLM.
    /// </summary>
    /// <param name="userText"></param>
    /// <returns></returns>
    public async Task<string> ProcessUserTextAsync(string userText)
    {
        // Please implement here the code to process the user text using the chat client.
        // Use the SystemPrompt and Tools to guide the chat completion.
        // Ensure to handle tool calls and format the final response with the current time.
        var messages = new List<OpenAIChatMessage>
        {
            OpenAIChatMessage.CreateSystemMessage(SystemPrompt),
            OpenAIChatMessage.CreateUserMessage(userText)
        };

        var chatOptions = new ChatCompletionOptions
        {
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            Tools = { Tools[0], Tools[1] }
        };

        var result = await _chatClient.CompleteChatAsync(messages, chatOptions);

        var completion = result.Value;

        while (completion.FinishReason == OpenAIChatFinishReason.ToolCalls)
        {
            messages.Add(new AssistantChatMessage(completion));

            foreach (ChatToolCall toolCall in completion.ToolCalls)
            {
                var content = GetToolCallContent(toolCall);

                messages.Add(new ToolChatMessage(toolCall.Id, content));
            }

            // Call again so the model can use the tool results to form its final answer
            result = await _chatClient.CompleteChatAsync(messages, chatOptions);

            completion = result.Value;
        }

        var textPart = completion.Content?.FirstOrDefault(c => !string.IsNullOrEmpty(c.Text));
        return textPart?.Text ?? "[No text content returned by the model]";
    }

    private string GetToolCallContent(ChatToolCall toolCall)
    {
        switch (toolCall.FunctionName)
        {
            case "get_time":
                return GetTime();
            case "format_text":
                {
                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                    bool hasText = argumentsJson.RootElement.TryGetProperty("text", out JsonElement text);
                    bool hasTime = argumentsJson.RootElement.TryGetProperty("time", out JsonElement time);

                    if (!hasText || !hasTime)
                    {
                        throw new InvalidOperationException("Missing required arguments.");
                    }

                    return GetFormattedText(text.GetString()!, time.GetString()!);
                }
            default:
                throw new NotImplementedException($"Function '{toolCall.FunctionName}' is not implemented.");
        }
    }

    private string GetTime()
    {
        return DateTime.UtcNow.ToString("u");
    }

    private string GetFormattedText(string text, string time)
    {
        return $"""
        === Formatted Text ===
        Time: {time}
        Text: {text}
        =======================
        """;
    }
}
