# Introduction to Generative AI and Large Language Models
In this module, we explore the dynamic realm of Generative AI and Large Language Models, with a particular focus on utilizing Azure's OpenAI service. The aim is to provide practical experience in integrating and using Azure OpenAI within a .NET application. By the conclusion of this module, you will be adept at configuring Azure OpenAI, working with the OpenAI SDK for .NET, and creating a basic application that leverages the power of OpenAI's language models.

## 📚 Learning Objectives
- Gain hands-on experience in setting up and configuring an Azure OpenAI instance.
- Understand how to use the OpenAI SDK designed for .NET applications.
- Build a simple application that demonstrates the capabilities of large language models using the OpenAI SDK for .NET.

## **Creating an Azure OpenAI Instance**

### **1. Create a Resource Group**
1. Go to the [Azure Portal](https://portal.azure.com/) and sign in.
2. In the search bar, type **"Resource groups"** and select it.
3. Click **"Create"** and fill in the following details:
   - **Subscription**: Select **Visual Studio Professional**.
   - **Resource Group Name**: Enter **`practical-generative-ai-course-rg`**.
   - **Region**: Choose a region that supports Azure OpenAI (e.g., East US, West Europe).
4. Click **"Review + Create"**, then **"Create"**.

### **2. Create an Azure OpenAI Resource**
1. In the Azure Portal, click **"Create a resource"**.
2. Search for **"Azure OpenAI"** and select it.
3. Click **"Create"** and fill in the required fields:
   - **Subscription**: Select **Visual Studio Professional**.
   - **Resource Group**: Choose **`practical-generative-ai-course-rg`**.
   - **Region**: Select a supported region.
   - **Name**: Enter a unique name for the resource.
   - **Pricing Tier**: Select **Standard S0**.
4. Click **"Review + Create"**, then **"Create"**.

### **3. Get API Keys and Endpoint**
1. After deployment, go to the **Azure OpenAI** resource.
2. Navigate to **"Keys and Endpoint"**.
3. Copy the **API Key** and **Endpoint URL** for future use.

### **4. Deploy a Model via Azure AI Foundry**
1. Go to the [Azure AI Foundry Portal](https://oai.azure.com/) and navigate to your **Azure OpenAI** resource.
2. In the **Share Resources** section, select **"Deployments"**.
3. Click **"Deploy model"** and fill in the following fields:
   - **Model**: Choose an available model, such as `gpt-4o`.
   - **Deployment Name**: Enter a unique name for the deployment.
   - **Deployment Type**: Select **Standard**.
4. Click **"Create"** to deploy the model.

### **5. Add model for image generation**
You can do it the same way as for the chat model, but choose the image model instead.  


For a more detailed overview of the process of creating and configuring an Azure OpenAI instance, you can watch the following video:
[![Creating an Azure OpenAI Instance](https://img.youtube.com/vi/NUrrF3jnmLE/0.jpg)](https://www.youtube.com/watch?v=NUrrF3jnmLE)

## 📑 Coding Task

## Open the `lab1` Project and Configure Environment Variables

### 1. Open the `lab1` Project
- Navigate to the project directory where `lab1` is located.
- Open the project in your preferred code editor (e.g., **Visual Studio Code, JetBrains Rider, or Visual Studio**).
- This project contains the **initial setup** required for this task, including:
  - Necessary project configuration.
  - Pre-installed dependencies.
  - Structured project files to streamline development.

### 2. Add and Configure the `.env` File
- Locate the **`.env.example`** file inside the project directory.
- Create a new file in the same directory and name it **`.env`**.
- Copy the contents of `.env.example` into the newly created `.env` file.
- Fill in the required configuration values.

### 3. Verify the Configuration
- Ensure that all required values in the `.env` file are correctly set to avoid runtime issues.
- If using **Git**, add `.env` to your **`.gitignore`** file to prevent exposing sensitive credentials in the repository.

## Review the OpenAI .NET API library Documentation  
- Open repository [OpenAI .NET API library](https://github.com/openai/openai-dotnet).  
- Read Readme.md and Wiki to understand how to use the library.  
- Review the [examples](https://github.com/openai/openai-dotnet/tree/main/examples) to understand how to use the library.  
  
## Chatting with Azure OpenAI
- Open the `Program.cs' file and review how can be used the ChatService.  
- Open the `ChatService.cs` file in the `Services` folder and implement `SendMessageAsync` method.  
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
Challenge yourself further by extending the application to incorporate the other types of API calls available in the OpenAI .NET API library. Experiment with different parameters and options to explore the full capabilities of Azure OpenAI.