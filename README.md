# Microsoft Agent Framework, Aspire and DDD

[![.NET](https://github.com/jonathandbailey/the-infinite-loop-semantic-kernel/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jonathandbailey/the-infinite-loop-semantic-kernel/actions/workflows/dotnet.yml)


The architecture is inspired by the work I've done on DDD projects and Jason Taylors [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)

## Running the Application

### Azure Open AI 
You will need to create an  Azure Open AI resource and add the DeploymentName, Endpoint and Api Key to the Development Settings.

### Azurite Local Setup
The application uses .NET Aspire to run Azurite (Azure Storage Emulator) in a container in Docker Desktop. It's configured to persist data onto disk.

I would recommend downloading Microsoft Azure Storage Explorer to manage the local Azurite Instance : 

- Run the application (Aspire app host)
- Connect to the Azurite Instance with Microsoft Azure Storage Explorer
- Create 2 Blob containers : agent-templates & user-conversation-history
- Add the semantice kernel templates (in the assets folder) to the agent-templates blob

### SPA
.NET Aspire will run the SPA application when you run the App Host, but it's won't do the first time install. 

After cloning the repo, run npm install in the ui folder.

You can access the UI from the .NET Aspire dashboard and then use the application.




