using System.ClientModel;
using System.ClientModel.Primitives;
using Azure.AI.OpenAI;
using Lab4.Services;
using Moq;
using NUnit.Framework;
using OpenAI.Chat;

namespace lab4.tests;

[TestFixture]
public class UserPromtFormatterServiceTests
{
    [Test]
    public void TestAlwaysPasses()
    {
        Assert.Pass();
    }

    [Test]
    public async Task ProcessUserTextAsync_NoToolCalls_ReturnsFinalContentDirectly()
    {
        // Arrange
        var finalCompletion = OpenAIChatModelFactory.ChatCompletion(
            id: "chatcmpl-final",
            finishReason: ChatFinishReason.Stop,
            content: new ChatMessageContent("Hello, formatted text!"),
            role: ChatMessageRole.Assistant,
            model: "gpt-4");

        var mockChatClient = new Mock<ChatClient>();
        mockChatClient
            .Setup(c => c.CompleteChatAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatCompletionOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(finalCompletion, new MockPipelineResponse(200)));

        var mockAzureClient = new Mock<AzureOpenAIClient>();
        mockAzureClient
            .Setup(c => c.GetChatClient(It.IsAny<string>()))
            .Returns(mockChatClient.Object);

        var service = new UserPromtFormatterService(mockAzureClient.Object, "test-deployment");

        // Act
        var result = await service.ProcessUserTextAsync("some user text");

        // Assert
        Assert.That(result, Is.EqualTo("Hello, formatted text!"));
        mockChatClient.Verify(c => c.CompleteChatAsync(
            It.IsAny<IEnumerable<ChatMessage>>(),
            It.IsAny<ChatCompletionOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ProcessUserTextAsync_WithToolCalls_InvokesToolsAndReturnsFinalContent()
    {
        // Arrange: first response asks for both tools, second response is the final answer
        var toolCalls = new List<ChatToolCall>
        {
            ChatToolCall.CreateFunctionToolCall(
                id: "call_get_time",
                functionName: "get_time",
                functionArguments: BinaryData.FromString("{}")),
            ChatToolCall.CreateFunctionToolCall(
                id: "call_format_text",
                functionName: "format_text",
                functionArguments: BinaryData.FromString(
                    "{\"text\":\"hello\",\"time\":\"2024-01-01T00:00:00Z\"}"))
        };

        var toolCallCompletion = OpenAIChatModelFactory.ChatCompletion(
            id: "chatcmpl-tools",
            finishReason: ChatFinishReason.ToolCalls,
            toolCalls: toolCalls,
            role: ChatMessageRole.Assistant,
            model: "gpt-4");

        var finalCompletion = OpenAIChatModelFactory.ChatCompletion(
            id: "chatcmpl-final",
            finishReason: ChatFinishReason.Stop,
            content: new ChatMessageContent("=== Formatted Text ==="),
            role: ChatMessageRole.Assistant,
            model: "gpt-4");

        var mockChatClient = new Mock<ChatClient>();
        mockChatClient
            .SetupSequence(c => c.CompleteChatAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatCompletionOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(toolCallCompletion, new MockPipelineResponse(200)))
            .ReturnsAsync(ClientResult.FromValue(finalCompletion, new MockPipelineResponse(200)));

        var mockAzureClient = new Mock<AzureOpenAIClient>();
        mockAzureClient
            .Setup(c => c.GetChatClient(It.IsAny<string>()))
            .Returns(mockChatClient.Object);

        var service = new UserPromtFormatterService(mockAzureClient.Object, "test-deployment");

        // Act
        var result = await service.ProcessUserTextAsync("hello");

        // Assert
        Assert.That(result, Is.EqualTo("=== Formatted Text ==="));
        mockChatClient.Verify(c => c.CompleteChatAsync(
            It.IsAny<IEnumerable<ChatMessage>>(),
            It.IsAny<ChatCompletionOptions>(),
            It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}

public class MockPipelineResponse(int status) : PipelineResponse
{
    public override int Status { get; } = status;

    public override string ReasonPhrase => throw new NotImplementedException();

    public override Stream? ContentStream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override BinaryData Content => throw new NotImplementedException();

    protected override PipelineResponseHeaders HeadersCore => throw new NotImplementedException();

    public override BinaryData BufferContent(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}