using Lab3.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NSubstitute;
using NUnit.Framework;

namespace lab3.tests;

[TestFixture]
public class OllamaServiceTest
{
    private IChatCompletionService _chatService = null!;
    private OllamaService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _chatService = Substitute.For<IChatCompletionService>();

        var builder = Kernel.CreateBuilder();

        builder.Services.AddSingleton(_chatService);

        var kernel = builder.Build();

        _service = new OllamaService(kernel);
    }

    [Test]
    public async Task GetSelfHostedAnswer_ShouldReturnAnswer()
    {
        // Arrange
        _chatService
            .GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                Arg.Any<PromptExecutionSettings?>(),
                Arg.Any<Kernel?>(),
                Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new ChatMessageContent(
                    AuthorRole.Assistant,
                    "Test answer")
            });

        // Act
        var result = await _service.GetSelfHostedAnswer("What is .NET?");

        // Assert
        Assert.That(result, Is.EqualTo("Test answer"));
    }

    [Test]
    public async Task GetSelfHostedAnswer_ShouldReturnEmptyString_WhenResponseIsEmpty()
    {
        // Arrange
        _chatService
            .GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                Arg.Any<PromptExecutionSettings?>(),
                Arg.Any<Kernel?>(),
                Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new ChatMessageContent(
                    AuthorRole.Assistant,
                    string.Empty)
            });

        // Act
        var result = await _service.GetSelfHostedAnswer("Hello");

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task GetSelfHostedAnswer_ShouldPassQuestionToModel()
    {
        // Arrange
        _chatService
            .GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                Arg.Any<PromptExecutionSettings?>(),
                Arg.Any<Kernel?>(),
                Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new ChatMessageContent(
                    AuthorRole.Assistant,
                    "Test answer")
            });

        var question = "What is dependency injection?";

        // Act
        await _service.GetSelfHostedAnswer(question);

        // Assert
        await _chatService.Received(1).GetChatMessageContentsAsync(
            Arg.Is<ChatHistory>(history =>
                history.Any(message =>
                    message.Role == AuthorRole.User &&
                    message.Content == question)),
            Arg.Any<PromptExecutionSettings?>(),
            Arg.Any<Kernel?>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public void GetSelfHostedAnswer_ShouldThrow_WhenModelFails()
    {
        // Arrange

        _chatService
            .GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                Arg.Any<PromptExecutionSettings?>(),
                Arg.Any<Kernel?>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<IReadOnlyList<ChatMessageContent>>>(_ =>
                throw new InvalidOperationException("Ollama unavailable"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GetSelfHostedAnswer("Hello"));
    }
}