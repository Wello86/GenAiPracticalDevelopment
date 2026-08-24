namespace Lab1.Abstractions;

public interface IImageService
{
    Task<string?> GenerateImageAsync(string prompt);
}