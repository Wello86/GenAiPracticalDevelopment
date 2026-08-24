using Lab3.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NSubstitute;
using NUnit.Framework;

namespace lab3.tests;

[TestFixture]
public class HuggingFaceServiceTest
{
    private IChatCompletionService _chatService = null!;
    private ILogger<HuggingFaceService> _logger = null!;
    private HuggingFaceService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _chatService = Substitute.For<IChatCompletionService>();

        var builder = Kernel.CreateBuilder();

        builder.Services.AddSingleton<IChatCompletionService>(_chatService);

        var kernel = builder.Build();

        _service = new HuggingFaceService(kernel);
    }

    [Test]
    public async Task GetHuggingFaceAnswer_ShouldReturnAnswer()
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
        var result = await _service.GetHuggingFaceAnswer("What is .NET?");

        // Assert
        Assert.That(result, Is.EqualTo("Test answer"));
    }

    [Test]
    public async Task GetHuggingFaceAnswer_ShouldReturnEmptyString_WhenResponseIsEmpty()
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
        var result = await _service.GetHuggingFaceAnswer("Hello");

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task GetHuggingFaceAnswer_ShouldPassQuestionToModel()
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
        await _service.GetHuggingFaceAnswer(question);

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
    public void GetHuggingFaceAnswer_ShouldThrow_WhenModelFails()
    {
        // Arrange
        _chatService
            .GetChatMessageContentsAsync(
                Arg.Any<ChatHistory>(),
                Arg.Any<PromptExecutionSettings?>(),
                Arg.Any<Kernel?>(),
                Arg.Any<CancellationToken>())
            .Returns<Task<IReadOnlyList<ChatMessageContent>>>(_ =>
                throw new InvalidOperationException("Hugging Face unavailable"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GetHuggingFaceAnswer("Hello"));
    }
}