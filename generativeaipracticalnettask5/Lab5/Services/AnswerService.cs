using Lab5.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
#pragma warning disable SKEXP0001

namespace Lab5.Services;

public class AnswerService(Kernel kernel, ISemanticTextMemory memory) : IAnswerService
{
    public async Task<string> AnswerToQuestionAsync(string question)
    {
        // Search semantic memory for relevant content using embeddings
        var searchResults = await memory.SearchAsync(
            "content",
            question,
            limit: 3,
            minRelevanceScore: 0.4
        ).ToListAsync();

        // Deduplicate results by text content
        var uniqueResults = searchResults
                            .DistinctBy(r => r.Metadata.Text)
                            .ToList();

        // Build context from search results
        var context = string.Join("\n", uniqueResults.Select(r => r.Metadata.Text));

        // Use Kernel to generate answer based on retrieved context
        var prompt = $@"Based on the following context, answer the question.
                        Context:
                        {context}

                        Question: {question}

                        Answer:";

        var answer = await kernel.InvokePromptAsync(prompt);
        return answer.ToString();
    }
}