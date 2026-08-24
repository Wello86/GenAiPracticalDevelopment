namespace Lab3.Abstractions;

public interface IHuggingFaceService
{
    Task<string> GetHuggingFaceAnswer(string question);
}