using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Globalization;
#pragma warning disable SKEXP0001

namespace Lab4.SKPlugins;

public class CustomPlugin(Kernel kernel)
{
    private readonly Kernel _kernel = kernel;

    [KernelFunction("Format")]
    [Description("Formats retrieved information with the current date and time.")]
    public string Format(
            [Description("The text to format.")] string text,
            [Description("The current date and time to include.")] DateTime dateTime)
    {
        string date = dateTime.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
        string time = dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
        return $"""
        === Topic Summary ===
        Generated on {date} at {time}
        -----------------------------------
        {text}
        ===================================
        """;
    }

    [KernelFunction("Translate")]
    [Description("Translates the given text into the specified language.")]
    public async Task<string> TranslateAsync(
       [Description("The text to translate.")] string text,
       [Description("The target language code (e.g., 'fr' for French).")] string language)
    {
        var prompt = $"You are a skilled translator. Please translate the following text into {language}:\n\n{text}";

        var skFunction = _kernel.CreateFunctionFromPrompt(
            promptTemplate: prompt,
            functionName: nameof(TranslateAsync),
            description: "Translate the text.");

        var result = await skFunction.InvokeAsync(_kernel);
        return result?.GetValue<string>() ?? string.Empty;
    }
}