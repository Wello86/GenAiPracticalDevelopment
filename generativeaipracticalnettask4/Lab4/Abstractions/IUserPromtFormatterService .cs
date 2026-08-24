namespace Lab4.Abstractions;

public interface IUserPromtFormatterService
{
    Task<string> ProcessUserTextAsync(string userText);
}
