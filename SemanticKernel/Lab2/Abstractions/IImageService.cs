namespace Lab2.Abstractions;

public interface IImageService
{
    Task<string> GenerateImageAsync(string prompt);
}