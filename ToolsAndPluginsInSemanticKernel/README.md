# Using Semantic Kernel with Tools and Plugins

In this section, you will learn how to leverage the Semantic Kernel to integrate various tools and plugins seamlessly. This approach simplifies the orchestration of multiple services, allowing you to focus on implementing core functionalities without worrying about the underlying complexities of tool integration.

## 📚 Learning Objectives
 - Understand how to integrate the Azure OpenAI .NET SDK for Tool Calling
 - Compare direct Azure OpenAI calls with Semantic Kernel’s plugin-based approach
 - Learn different types of SK plugins
 - Implement code that uses different types of SK plugins
 - Learn how to leverage Semantic Kernel (SK) to orchestrate calls to multiple tools and plugins

## 📑 Task

### Open "Lab4" Project

Welcome to Lab 4! In this task, we are going to experiment with integrating the Azure OpenAI .NET SDK and Semantic Kernel plugins. Open the "Lab4" project located in the `Lab4` folder of your course materials. This project contains the initial setup required for this task, including the necessary project configuration and dependencies.

### Prepare configuration files:

Rename the `.env.example` file to `.env` and fill in your Azure OpenAI and Google Custom Search credentials:
- `OPENAIENDPOINT`: The endpoint URL for the Azure OpenAI service, which is used for generating text.
- `OPENAIAPIKEY`: Your API key for accessing the Azure OpenAI service.
- `OPENAIMODEL_DEPLOYMENT`: The deployment name of the OpenAI model you are using, for example, gpt-4o.
- `GOOGLE_SEARCH_API_KEY`: Your Google Custom Search API key.
- `GOOGLE_SEARCH_ENGINE_ID`: Your Google Custom Search Engine ID.

### Setup Google Custom Search API service:
Get Access to Google Custom Search API using the following steps:
- Create a new project at [Google Developer Dashboard](https://console.developers.google.com/apis/dashboard) 
- Create a new API key at [Credentials Tab](https://console.developers.google.com/apis/credentials)  
- Enable the [Custom Search API](https://console.developers.google.com/apis/library/customsearch.googleapis.com)
- Create a new [Custom Search Engine](https://cse.google.com/cse/all)
- Add your API Key and your Custom Search Engine ID to the configuration file

**Usage Limits**
Google gives you 100 free searches per day. You can increase this limit by creating a billing account.

### Call function with OpenAI .NET SDK:
Open file "Services/UserPromtFormatterService.cs" and implement the body of the `ProcessUserTextAsync` method.

This method should:
1. Prepare `ChatCompletionOptions` with tools, and a list of `ChatMessage` with system and user messages.
2. Call the LLM in a loop until the stop reason is not function calling.
3. When the LLM requires a function call, invoke the required function and return data to the LLM.

The service uses two tools:
1. `get_time`: Returns the current UTC time
2. `format_text`: Formats text with provided time (requires "text" and "time" parameters)

Make sure the tool definitions match the implementation in the `GetToolCallContent` method.

**Step-by-Step Guide:**
1. Initialize a list of `ChatMessage` with a system message and the user's text.
2. Create `ChatCompletionOptions` and set the tools.
3. Use a `while` loop to call the LLM until the stop reason is not function calling.
4. Handle tool calls by invoking the required function and adding the response to the messages list.
5. Return the final formatted text.

### Call function with Semantic Kernel plugins:

Review the plugin code in the SKPlugins folder.

Open `Services/DataSummaryService.cs` and in the constructor register the following plugins into the kernel:
1. `TimePlugin` from `Microsoft.SemanticKernel.Plugins.Core`.
2. `WebSearchEnginePlugin` from `Microsoft.SemanticKernel.Plugins.Web`.
3. `CustomPlugin` from `SKPlugins`.

In the same class, implement the body of the `SummarizeTopicAsync` method.

This method should:
1. Initialize `OpenAIPromptExecutionSettings` with `FunctionChoiceBehavior.Auto`.
2. Initialize `ChatHistory` with the system message (which guides the LLM to use plugins in order):
   - First use WebSearchEnginePlugin to find information
   - Then use TimePlugin to get current time
   - Then use CustomPlugin to format and translate
3. Add the user's topic as a chat message.
4. Call `_chatCompletionService.CompleteAsync` with settings and history.
5. Return the result, which will be automatically translated to the specified language.

**Step-by-Step Guide:**
1. Initialize `OpenAIPromptExecutionSettings` with `FunctionChoiceBehavior.Auto`.
2. Create a `ChatHistory` with a system message and the user's topic.
3. Call the completion method of `_chatCompletionService` with the execution settings and chat history.
4. Return the summarized and translated content.

### Comparing Approaches

After completing both parts, compare the two approaches:

* Plain OpenAI .NET SDK Approach:
  * You manually created tool schema JSON for each function.
  * You wrote logic for invoking tools and feeding back the results to the LLM.
  * The workflow was more explicit but required more boilerplate code.
* Semantic Kernel Approach:
  * Registering plugins is straightforward; SK functions can be annotated with attributes like [KernelFunction] and [Description] to auto-generate schema and tool definitions.
  * With FunctionChoiceBehavior.Auto, SK automatically determines when to call a plugin without requiring manual parsing or schema handling.
  * Tools and plugins are easier to implement and integrate, reducing boilerplate and simplifying orchestration.
  
Reflect on how SK’s built-in plugins and auto-invocation of functions free you from writing detailed tool call schemas and loops. Consider how this can scale as you add more tools and functionality in future applications.

> **Additional Resources**
> - [OpenAI .NET SDK Documentation](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-use-chat-completions-with-tools-and-function-calling)
> - [Semantic Kernel Plugin Documentation](https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/?tabs=Csharp)

### Free practice:
Review plugins code from official [Semantic Kernel repository](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/src/Plugins), implement your own plugin and test it with your application.
