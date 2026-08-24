namespace Lab4.Abstractions;

public interface IDataSummaryService
{
    Task<string> SummarizeTopicAsync(string filePath, string language);
}
