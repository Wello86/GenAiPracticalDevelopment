# Working with Different Models
Welcome to Lab 3! In this exercise, we'll explore the configuration of Semantic Kernel with Hugging Face, a leading provider of Natural Language Processing resources.  
You'll learn to set up the Semantic Kernel, and send requests to different models. In this case we will send the requests to the HuggingFace using DIAL as API Proxy. Let's get started!

## 📚 Learning Objectives
 - Review the compatibility of the Hugging Face
 - Review the ability to run Self Hosted Models
 - Write an application that uses the Hugging Face model with Semantic Kernel
 - Write implementation that call Self Hosted Model with Semantic Kernel

## 📑 Task


### Open "Lab3" Project

Open the "Lab3" Solution folder of your course materials. This project contains the initial setup required for this task, including the necessary project configuration and dependencies.
Add .env file to the project and fill it with configuration values from .net.example file.


### Register on HuggingFace and Get an API Key

Visit the Hugging Face website: [https://huggingface.co/](https://huggingface.co/)  
Register or Log in to the portal.  
Click on your username and select "Settings".  
In the "Access Tokens" tab, click the "New Token" button.  
Enter a descriptive name for your token.  
Select the appropriate permissions for your token.  
Click the "Generate token" button.  
Copy the generated API token and save it to .env file.  

### Find Chat Model on Hugging Face  

Navigate to the Hugging Face Models Hub: [https://huggingface.co/models](https://huggingface.co/models)  
Scroll down to the "Natural Language Processing" section and filter the models by appropriate tags.
Choose a model that you would like to use and copy the model name into .env file.

### Call Hugging Face model:

Now implement the code that will call the Hugging Face model using the keyed semantic kernel instance.
Implement body of the `CallHuggingFaceModel` method in the `Services/HuggingFaceService.cs` file.
For calling the model use method InvokePromptAsync with question as a parameter. 
Call application to get the response from the Hugging Face model.


### Setup and Run Self Hosted Models
Go to [Ollama application portal](https://ollama.com/) and download ollama.  
Install application.  
Review supported [model library](https://github.com/ollama/ollama?tab=readme-ov-file#model-library) and choose the model that you would like to use.
Start ollama aplication with the selected model using command line 
```
ollama run <model_name>
```

### Configure connection to the Self Hosted Model
Add to the project nuget package `Microsoft.SematicKernel.Connectors.Ollama`.  
Add name of the selected model to the .env file.  
Congigure Keyed Semantic Kernel to use Ollama model.
Implement body of the `GetSelfHostedAnswer` method in the `Services/OllamaService.cs` file.  
For calling the model use method InvokePromptAsync with question as a parameter.   


### Closing

Create pull requests to the original repository and check that pipeline has passed.

Create a screenshot of the workflow showing the checks have passed and attach this screenshot to your tasks on learn.epam.com.

### Free practice:

Run different models using the ollama and compare the inference time and quality of the answers.
