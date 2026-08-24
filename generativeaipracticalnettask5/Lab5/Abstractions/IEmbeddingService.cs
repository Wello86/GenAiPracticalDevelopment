namespace Lab5.Abstractions;

public interface IEmbeddingService
{
    Task<string> GenerateEmbeddingsAsync(string fileName);
}