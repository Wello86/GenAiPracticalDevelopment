namespace Lab5.Abstractions;

public interface IAnswerService
{
    Task<string> AnswerToQuestionAsync(string question);
}