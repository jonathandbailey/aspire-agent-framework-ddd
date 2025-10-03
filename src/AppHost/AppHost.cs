
using AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

const string apiName = "api";

var storage = builder.AddAzureStorageServices();

var serviceBus = builder.AddServiceBusServices();
var userTopic = "user-topic";

var topic = serviceBus.AddServiceBusTopic(userTopic);

var conversationQueue = "agent-conversation-queue";
var agentConversationQueue = serviceBus.AddServiceBusQueue(conversationQueue);

var summarizerQueue = "agent-summarizer-queue";
var agentSummarizerQueue = serviceBus.AddServiceBusQueue(summarizerQueue);

var domainQueue = "conversation-domain-queue";
var conversationDomainQueue = serviceBus.AddServiceBusQueue(domainQueue);

topic.AddServiceBusSubscription("subscription")
    .WithProperties(subscription =>
    {
        subscription.MaxDeliveryCount = 10;
    });

var blobs = builder.AddAzureBlobsServices(storage);

var api = builder.AddProject<Projects.Api>(apiName).WithReference(blobs).WaitFor(storage)
    .WithReference(serviceBus).WaitFor(topic).WaitFor(conversationDomainQueue);

var hub = builder.AddProject<Projects.Api_Hub>("api-hub").WithReference(serviceBus).WaitFor(topic);

var ui = builder.AddUiServices(api, hub);

api.WithReference(ui);
hub.WithReference(ui);


builder.AddProject<Projects.Agents_Conversation>("agents-conversation").WithReference(blobs).WaitFor(storage)
    .WithReference(serviceBus).WaitFor(agentConversationQueue)
    .WithEnvironment("Queues__Agent", conversationQueue)
    .WithEnvironment("Queues__Domain", domainQueue)
    .WithEnvironment("Topics__User", userTopic);


builder.AddProject<Projects.Agents_Summarizer>("agents-summarizer")
    .WithReference(serviceBus).WaitFor(agentSummarizerQueue)
    .WithEnvironment("Queues__Agent", summarizerQueue)
    .WithEnvironment("Topics__User", userTopic);


var build = builder.Build();
build.Run();