namespace Lab2.Abstractions;

public interface IChatService
{
    Task<string?> SendMessageAsync(string message);
}