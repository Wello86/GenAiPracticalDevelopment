using Lab5.Services;
using Microsoft.KernelMemory.DataFormats;
using Microsoft.SemanticKernel.Memory;
using Moq;
using NUnit.Framework;
#pragma warning disable KMEXP00
#pragma warning disable SKEXP0001

namespace lab5.tests;

[TestFixture]
public class EmbeddingServiceTests
{
    private Mock<ISemanticTextMemory> _mockMemory;
    private Mock<IContentDecoder> _mockPdfDecoder;
    private EmbeddingService _embeddingService;

    [SetUp]
    public void Setup()
    {
        _mockMemory = new Mock<ISemanticTextMemory>();
        _mockPdfDecoder = new Mock<IContentDecoder>();
        _embeddingService = new EmbeddingService(_mockMemory.Object, _mockPdfDecoder.Object);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_WithValidPdfFile_ReturnsDocumentId()
    {
        // Arrange
        var fileName = "test.pdf";
        var pdfContent = new FileContent("pdf");

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(fileName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        var result = await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
        Assert.That(Guid.TryParse(result, out _), Is.True);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_CallsPdfDecoder_WithCorrectFileName()
    {
        // Arrange
        var fileName = "document.pdf";
        var pdfContent = new FileContent("pdf");

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(fileName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert
        _mockPdfDecoder.Verify(
            d => d.DecodeAsync(fileName, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_SavesContentToMemory_WithConcatenatedSections()
    {
        // Arrange
        var fileName = "test.pdf";
        var expectedContent = "Section 1 contentSection 2 content";
        var pdfContent = new FileContent("pdf")
        {
            Sections = new List<FileSection>
            {
                new FileSection(0, "Section 1 content",true),
                new FileSection(1, "Section 2 content",true)
            }
        };
        var documentId = Guid.NewGuid().ToString();

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync(documentId);

        // Act
        await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert
        _mockMemory.Verify(
            m => m.SaveInformationAsync(
                "content",
                expectedContent,
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_WithMultipleSections_ConcatenatesAllContent()
    {
        // Arrange
        var fileName = "multipage.pdf";
        var expectedContent = "Section 1 contentSection 2 content";
        var pdfContent = new FileContent("pdf")
        {
            Sections = new List<FileSection>
            {
                new FileSection(0, "Section 1 content",true),
                new FileSection(1, "Section 2 content",true)
            }
        };

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert
        _mockMemory.Verify(
            m => m.SaveInformationAsync(
                "content",
                expectedContent,
                It.IsAny<string>(),
                null,
                null,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_WithEmptyPdf_ReturnsDocumentId()
    {
        // Arrange
        var fileName = "empty.pdf";
        var pdfContent = new FileContent("pdf");
        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        var result = await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(Guid.TryParse(result, out _), Is.True);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_SavesWithContentCollection()
    {
        // Arrange
        var fileName = "test.pdf";
        var pdfContent = new FileContent("pdf");

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert - Verify collection name is "content"
        _mockMemory.Verify(
            m => m.SaveInformationAsync(
                "content",
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GenerateEmbeddingsAsync_SavesCalled_WithAllRequiredParameters()
    {
        // Arrange
        var fileName = "parameter-test.pdf";
        var pdfContent = new FileContent("pdf");

        _mockPdfDecoder
            .Setup(d => d.DecodeAsync(fileName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfContent);

        _mockMemory
            .Setup(m => m.SaveInformationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                null,
                CancellationToken.None))
            .ReturnsAsync("test-document-id");

        // Act
        await _embeddingService.GenerateEmbeddingsAsync(fileName);

        // Assert - Verify SaveInformationAsync was called exactly once with correct collection
        _mockMemory.Verify(
            m => m.SaveInformationAsync(
                "content",
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null,
                null,
                CancellationToken.None),
            Times.Once);
    }
}