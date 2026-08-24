# Retrieval-Augmented Generation (RAG)

## 📚 Learning Objectives
 - Understand what is Retrieval-Augmented Generation
 - Learn how to use embeddings with RAG
 - Implement RAG application that uses PDF document as a knowledge source

## 📑 Task

### Open "Lab5" Project

Open the "Lab5" solution in the root folder of this repository. The project contains the initial setup required for this task, including the necessary project configuration and dependencies.

### Prepare configuration files
Rename file `.env.example` to `.env` and fill it with configuration values.

- `CHROMA_ENDPOINT`: The endpoint URL for the Chroma service, which is used for document storage and retrieval. Default value is `http://localhost:8000`.
- `OPENAIENDPOINT`: The endpoint URL for the Azure OpenAI service, which is used for generating text.
- `OPENAIAPIKEY`: Your API key for accessing the Azure OpenAI service.
- `OPENAIMODEL_DEPLOYMENT`: The deployment name of the OpenAI model you are using, for example, `gpt-4o`.
- `EMBEDDINGS_DEPLOYMENT`: The deployment name of the embeddings model you are using, for example, `text-embedding-ada-002`.

### Setup and run ChromaDb service
Let's start by reviewing the ChromaDb capabilities from the [ChromaDb](https://docs.trychroma.com/) documentation.

- Install the ChromaDb by running following command:
```bash
pip install chromadb 
```

- Start the ChromaDb service by running the following command:
```bash
chroma run --host localhost --port 8000
```
Check that the service url is the same as in configuration file.

### Implement embeddings
Open file "Services/EmbeddingsService.cs" and implement body of the GenerateEmbeddingsAsync method.   

This method should: 
1. Decode file using PDF decorer. 
2. Concatenate content of file content sections into one string. 
3. Save this string to the ChromaDb and return the id of the saved document.

### Implement RAG

Open file "Services/AnswerService.cs" and implement body of the AnswerToQuestionAsync method.

This method should:
1. Search through the ChromaDb for the text similar to question passed as parameter to the method.
2. Generate the LLM request that contains the data from embeddings and the question.
3. Call the Azure OpenAI service with the generated request and return the response.

### Run the application
Run the application and check that it works as expected.