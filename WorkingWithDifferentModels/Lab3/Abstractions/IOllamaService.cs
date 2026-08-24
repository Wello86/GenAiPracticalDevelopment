namespace Lab3.Abstractions;

public interface IOllamaService
{
    Task<string> GetSelfHostedAnswer(string question);
}