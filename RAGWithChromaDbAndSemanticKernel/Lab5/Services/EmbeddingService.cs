using Lab5.Abstractions;
using Microsoft.KernelMemory.DataFormats;
using Microsoft.SemanticKernel.Memory;
#pragma warning disable KMEXP00
#pragma warning disable SKEXP0001

namespace Lab5.Services;

/// <summary>
/// Hint: this works only with Python 3.11, and requires the following packages: numpy 1.26.4
/// </summary>
/// <param name="memory"></param>
/// <param name="pdfDecoder"></param>
public class EmbeddingService(ISemanticTextMemory memory, IContentDecoder pdfDecoder) : IEmbeddingService
{
    public async Task<string> GenerateEmbeddingsAsync(string fileName)
    {
        // Decode the PDF file using the PDF decoder
        var decodedContent = await pdfDecoder.DecodeAsync(fileName, CancellationToken.None);

        // Concatenate all content sections into one string
        var concatenatedContent = string.Concat(decodedContent.Sections.Select(section => section.Content));

        // Save to ChromaDb and return the document id
        var documentId = Guid.NewGuid().ToString();
        await memory.SaveInformationAsync(
            collection: "content",
            id: documentId,
            text: concatenatedContent);

        return documentId;
    }
}