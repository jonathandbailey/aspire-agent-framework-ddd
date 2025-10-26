# Microsoft Agent Framework, Aspire and DDD

[![.NET](https://github.com/jonathandbailey/the-infinite-loop-semantic-kernel/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jonathandbailey/the-infinite-loop-semantic-kernel/actions/workflows/dotnet.yml)


The architecture is inspired by the work I've done on DDD projects and Jason Taylors [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)




# Microsoft Agent Framework, Aspire & Domain-Driven Design Exploration

**Educational project exploring modern distributed application design using the Microsoft Aspire ecosystem, Domain-Driven Design (DDD), and Agentic patterns.**

> ⚠️ **Work in Progress / Educational Project**  
> This repository is a learning and experimentation space — not a production-ready framework.  
> APIs, structure, and agents may change frequently.

---

## 🚀 Overview

The **Aspire Agent Framework** demonstrates how to build modular, distributed, and message-driven systems using:

- **Microsoft Aspire Framework** — for orchestrating distributed microservices and cloud resources.  
- **Domain-Driven Design (DDD)** — to model clear domain boundaries, entities, and aggregates for managing user conversations.
- **Azure Service Bus** — to handle distributed messaging between agents and Signal R Hubs
- **SignalR Hubs** — for live, streaming communication with connected clients.  
- **React UI** — lightweight, throwaway demo frontend for visualizing agent interactions.

This project serves as a **sandbox** for exploring architectural principles and integration patterns in a modern .NET distributed environment.

## Running the Application

### Azure Open AI 
You will need to create an  Azure Open AI resource and add the DeploymentName, Endpoint and Api Key to the Development Settings.

### Azurite Local Setup
The application uses .NET Aspire to run Azurite (Azure Storage Emulator) in a container in Docker Desktop. It's configured to persist data onto disk.

Download the [Microsoft Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer) to manage the local Azurite Instance : 

- Run the application (Aspire app host)
- Connect to the Azurite Instance with Microsoft Azure Storage Explorer
- Create 2 Blob containers : agent-templates & user-conversation-history
- Add the semantice kernel templates (in the assets folder) to the agent-templates blob

### SPA
.NET Aspire will run the SPA application when you run the App Host, but it's won't do the first time install. 

After cloning the repo, run npm install in the ui folder.

You can access the UI from the .NET Aspire dashboard and then use the application.




