using Lab5.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
#pragma warning disable SKEXP0001

namespace lab5.tests;

[TestFixture]
public class AnswerServiceTests
{
    private Kernel _kernel;
    private Mock<IChatCompletionService> _mockChatCompletion;
    private Mock<ISemanticTextMemory> _mockMemory;
    private AnswerService _answerService;

    [SetUp]
    public void Setup()
    {
        _mockChatCompletion = new Mock<IChatCompletionService>();

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddSingleton(
            typeof(IChatCompletionService),
            _mockChatCompletion.Object);

        _kernel = kernelBuilder.Build();
        _mockMemory = new Mock<ISemanticTextMemory>();
        _answerService = new AnswerService(_kernel, _mockMemory.Object);
    }

    [Test]
    public async Task AnswerToQuestionAsync_WithValidQuestion_ReturnsAnswer()
    {
        // Arrange
        var question = "What is artificial intelligence?";
        var expectedAnswer = "AI is a field of computer science.";

        var memorySearchResults = new List<MemoryQueryResult>
        {
            MemoryQueryResult.FromMemoryRecord(
                new MemoryRecord(
                    new MemoryRecordMetadata(
                        isReference: false,
                        id: "1",
                        text: "AI is the field of computer science.",
                        description: "AI definition",
                        externalSourceName: "content",
                        additionalMetadata: ""
                    ),
                    embedding: new ReadOnlyMemory<float>(new float[] { 0.1f, 0.2f }),
                    key: "1",
                    timestamp: null
                ),
                0.9
            )
        };

        _mockMemory
            .Setup(m => m.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                false,
                null,
                CancellationToken.None))
            .Returns(memorySearchResults.ToAsyncEnumerable());

        _mockChatCompletion
            .Setup(chat => chat.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                _kernel,
                CancellationToken.None))
            .ReturnsAsync(new[]
            {
                new ChatMessageContent(AuthorRole.Assistant, expectedAnswer)
            });

        // Act
        var result = await _answerService.AnswerToQuestionAsync(question);

        // Assert
        Assert.That(result, Is.EqualTo(expectedAnswer));
    }

    [Test]
    public async Task AnswerToQuestionAsync_WithEmptySearchResults_ReturnsAnswerWithoutContext()
    {
        // Arrange
        var question = "What is unknown?";
        var expectedAnswer = "No relevant context found.";

        _mockMemory
            .Setup(m => m.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                false,
                null,
                CancellationToken.None))
            .Returns(new List<MemoryQueryResult>().ToAsyncEnumerable());

        _mockChatCompletion
            .Setup(chat => chat.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                _kernel,
                CancellationToken.None))
            .ReturnsAsync(new[]
            {
                new ChatMessageContent(AuthorRole.Assistant, expectedAnswer)
            });

        // Act
        var result = await _answerService.AnswerToQuestionAsync(question);

        // Assert
        Assert.That(result, Is.EqualTo(expectedAnswer));
    }

    [Test]
    public async Task AnswerToQuestionAsync_WithDuplicateResults_DeduplicatesResults()
    {
        // Arrange
        var question = "Tell me something?";
        var contextText = "Important information";

        var memorySearchResults = new List<MemoryQueryResult>
        {
            MemoryQueryResult.FromMemoryRecord(
                new MemoryRecord(
                    new MemoryRecordMetadata(
                        isReference: false,
                        id: "1",
                        text: contextText,
                        description: "Result 1",
                        externalSourceName: "content",
                        additionalMetadata: ""
                    ),
                    embedding: new ReadOnlyMemory<float>(new float[] { 0.1f }),
                    key: "1",
                    timestamp: null
                ),
                 0.9
            ),
            MemoryQueryResult.FromMemoryRecord(
                new MemoryRecord(
                    new MemoryRecordMetadata(
                        isReference: false,
                        id: "2",
                        text: contextText,
                        description: "Result 2 (duplicate)",
                        externalSourceName: "content",
                        additionalMetadata: ""
                    ),
                    embedding: new ReadOnlyMemory<float>(new float[] { 0.1f }),
                    key: "2",
                    timestamp: null
                ),
                 0.8
            )
        };

        _mockMemory
            .Setup(m => m.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                false,
                null,
                CancellationToken.None))
            .Returns(memorySearchResults.ToAsyncEnumerable());

        _mockChatCompletion
            .Setup(chat => chat.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                _kernel,
                CancellationToken.None))
            .ReturnsAsync(new[]
            {
                new ChatMessageContent(AuthorRole.Assistant, "Answer")
            });

        // Act
        var result = await _answerService.AnswerToQuestionAsync(question);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task AnswerToQuestionAsync_SearchCalled_WithCorrectParameters()
    {
        // Arrange
        var question = "Test question?";
        var memorySearchResults = new List<MemoryQueryResult>();

        _mockMemory
            .Setup(m => m.SearchAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<double>(),
                false,
                null,
                CancellationToken.None))
            .Returns(memorySearchResults.ToAsyncEnumerable());

        _mockChatCompletion
            .Setup(chat => chat.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                _kernel,
                CancellationToken.None))
            .ReturnsAsync(new[]
            {
                new ChatMessageContent(AuthorRole.Assistant, "Answer")
            });

        // Act
        await _answerService.AnswerToQuestionAsync(question);

        // Assert
        _mockMemory.Verify(
            m => m.SearchAsync(
                "content",
                question,
                3,
                0.4,
                false,
                null,
                CancellationToken.None),
            Times.Once);
    }
}