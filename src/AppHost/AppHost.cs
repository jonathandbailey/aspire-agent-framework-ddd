using AppHost.Extensions;
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

const string apiName = "api";

var storage = builder.AddAzureStorageServices();

var serviceBus = builder.AddServiceBusServices();
var userTopic = "user-topic";

var topic = serviceBus.AddServiceBusTopic(userTopic);

var agentQueueName = "agent-queue";
var agentQueue = serviceBus.AddServiceBusQueue(agentQueueName);


var domainTopic = "conversation-domain-topic";
var conversationDomainTopic = serviceBus.AddServiceBusTopic(domainTopic);

topic.AddServiceBusSubscription("subscription")
    .WithProperties(subscription =>
    {
        subscription.MaxDeliveryCount = 10;
    });

var exchangeCompleteSubscription = "exchange-complete-subscription";
conversationDomainTopic.AddServiceBusSubscription(exchangeCompleteSubscription)
    .WithProperties(subscription =>
    {
        subscription.MaxDeliveryCount = 10;
        subscription.Rules.Add(
            new AzureServiceBusRule("ExchangeCompleteRule")
            {
                CorrelationFilter = new AzureServiceBusCorrelationFilter
                {
                    Subject = "ExchangeComplete"
                }
            });
    });

var titleUpdateSubscription = "title-update-subscription";
conversationDomainTopic.AddServiceBusSubscription(titleUpdateSubscription)
    .WithProperties(subscription =>
    {
        subscription.MaxDeliveryCount = 10;
        subscription.Rules.Add(
            new AzureServiceBusRule("TitleUpdateRule")
            {
                CorrelationFilter = new AzureServiceBusCorrelationFilter
                {
                    Subject = "TitleUpdate"
                }
            });
    });

var blobs = builder.AddAzureBlobsServices(storage);

var api = builder.AddProject<Projects.Api>(apiName).WithReference(blobs).WaitFor(storage)
    .WithReference(serviceBus)
    .WithEnvironment("Queues__Agent", agentQueueName)
    .WithEnvironment("Topics__Domain", domainTopic)
    .WithEnvironment("Topics__DomainSubscriptionExchange", exchangeCompleteSubscription)
    .WithEnvironment("Topics__DomainSubscriptionTitle", titleUpdateSubscription)
    .WaitFor(topic).WaitFor(conversationDomainTopic);

var hub = builder.AddProject<Projects.Api_Hub>("api-hub").WithReference(serviceBus).WaitFor(topic);

var ui = builder.AddUiServices(api, hub);

api.WithReference(ui);
hub.WithReference(ui);

var apiInfrastucture = builder.AddProject<Projects.Api_Infrastructure>("api-infrastructure")
    .WithReference(blobs).WaitFor(storage);


builder.AddProject<Projects.Agents_Conversation>("agents-conversation").WithReference(blobs).WaitFor(storage)
    .WithReference(serviceBus).WaitFor(agentQueue)
    .WithReference(apiInfrastucture).WaitFor(apiInfrastucture)
    .WithEnvironment("Queues__Agent", agentQueueName)
    .WithEnvironment("Topics__Domain", domainTopic)
    .WithEnvironment("Topics__User", userTopic);


var build = builder.Build();
build.Run();