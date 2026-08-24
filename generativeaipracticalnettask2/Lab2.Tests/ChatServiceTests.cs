using FluentAssertions;
using Lab2.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using NUnit.Framework;

namespace lab2.tests
{
    [TestFixture]
    public class ChatServiceTests
    {
        private Mock<IChatCompletionService> _chatCompletionMock;
        private Kernel _kernel;
        private ChatService _chatService;

        [SetUp]
        public void SetUp()
        {
            _chatCompletionMock = new Mock<IChatCompletionService>();

            // Build a real Kernel instance injected with our mocked IChatCompletionService
            var builder = Kernel.CreateBuilder();
            builder.Services.AddKeyedSingleton("testService", _chatCompletionMock.Object);
            _kernel = builder.Build();

            // Instantiate your class passing the configured kernel
            _chatService = new ChatService(_kernel);
        }

        [Test]
        public async Task SendMessageAsync_WhenPromptIsValid_ReturnsExpectedResponse()
        {
            // Arrange
            string prompt = "Hello AI";
            string expectedResponse = "Hello Human!";

            _chatCompletionMock
                .Setup(x => x.GetChatMessageContentsAsync(
                    It.IsAny<ChatHistory>(),
                    It.IsAny<PromptExecutionSettings>(),
                    It.IsAny<Kernel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new ChatMessageContent(AuthorRole.Assistant, expectedResponse) });

            // Act
            string? result = await _chatService.SendMessageAsync(prompt);

            // Assert
            result.Should().Be(expectedResponse);
        }

        [Test]
        public void SendMessageAsync_WhenKernelThrowsException_ThrowsException()
        {
            // Arrange
            string prompt = "Error prompt";

            _chatCompletionMock
                .Setup(x => x.GetChatMessageContentsAsync(
                    It.IsAny<ChatHistory>(),
                    It.IsAny<PromptExecutionSettings>(),
                    It.IsAny<Kernel>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("AI Service Unavailable"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _chatService.SendMessageAsync(prompt));
        }
    }
}