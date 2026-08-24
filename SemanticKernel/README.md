# Semantic Kernel Fundamentals  

Semantic Kernel (SK) is a lightweight, open-source SDK that enables developers to integrate AI capabilities, such as large language models (LLMs), into their applications. One of its key features is prompt templating, which allows dynamic prompt construction using variables, functions, and contextual data to enhance AI interactions.

In this assignment, you will explore basic Semantic Kernel functionality and gain hands-on experience with SK Prompt Templating. The goal is to understand how to craft effective prompts and leverage SK’s capabilities to generate relevant and coherent AI responses.


## 📚 Learning Objectives
In this section, participants will dive into **Semantic Kernel (SK)** and its role in orchestrating **LLM calls**, comparing SK’s API with the **OpenAI SDK**, and refining their **prompt engineering** skills.  

- **Gaining experience in Semantic Kernel orchestration**  
  Understand how SK facilitates interactions with LLMs, enabling dynamic AI-driven applications.  

- **Comparing SK API with the OpenAI SDK**  
  Explore differences in usage, flexibility, and integration approaches between SK and direct OpenAI API calls.  

- **Enhancing prompt engineering skills**  
  Learn to structure and optimize prompts using SK’s templating features to guide AI responses effectively.  

This hands-on experience will provide insights into **AI-driven workflows** and help participants build more efficient AI-powered solutions.


## 📑 Task
![image](https://github.com/epam-net-cc/GenAIFoundationsForNetDevelopers/assets/4239376/c2ce11a0-00d4-49aa-953d-3a631ab717c1)

Welcome to Lab 2! In this task, we are going to experiment with Semantic Kernel, based on Lab 1, its capabilities.

### Open "Lab2" Project

Open `Lab2.sln` in the root folder. This solution contains the initial setup required for this task, including the necessary project configuration and dependencies. Add a .env file in the `Lab2` project to the project and fill it with configuration values from the .env.example file.

If needed, you can check the configuration from previous labs.

### Review the Semantic Kernel Documentation  

- Open the repository: [Semantic Kernel](https://github.com/microsoft/semantic-kernel).  
- Read the `README.md` and Wiki to understand how to use the library.  
- Review the [examples](https://github.com/microsoft/semantic-kernel/tree/main/dotnet/samples) to see practical implementations of the library.  

### Initialize the Semantic Kernel using Kernel Builder:

Review the existing code and ensure that the Semantic Kernel is set up correctly. Use the Kernel Builder to create a new instance of the Semantic Kernel with the desired configuration.
You can find configuration in `Program.cs` file.

## Chatting with Azure OpenAI
- Open the `Program.cs' file and review how can be used the ChatService.  
- Open the `ChatService.cs` file in the `Services` folder and implement `SendMessageAsync` method. You can use the InvokePromptAsync method from the Semantic Kernel to send a prompt to the OpenAI API.  
- Run the application and test that the Azure OpenAI returns a response.  
- Play a bit with `ChatCompletionOptions` to see how it affects the response.  

### Generate an Image Using OpenAI

- Open the `Program.cs' file and review how can be used the ImageService.  
- Open the `ImageService.cs` file in the `Services` folder and implement `GenerateImageAsync` method.  
- Run the application and test that the Azure OpenAI returns an url to the generate image.  

### Final Steps
 - Commit and push the changes to your repository.
 - Review evaluation results and change implementation if needed.

### Engage in Free Practice
Review [Semantic Kernel prompt template syntax](https://learn.microsoft.com/en-us/semantic-kernel/concepts/prompts/prompt-template-syntax) and explore how to use it in your application.
Challenge yourself further by extending the application to incorporate the other types of API calls available in the OpenAI .NET API library. Experiment with different parameters and options to explore the full capabilities of Azure OpenAI.