
using AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

const string apiName = "api";

var storage = builder.AddAzureStorageServices();

var serviceBus = builder.AddServiceBusServices();
var topic = serviceBus.AddServiceBusTopic("topic");
var queue = serviceBus.AddServiceBusQueue("queue");

var conversationDomainQueue = serviceBus.AddServiceBusQueue("conversation-domain-queue");

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


builder.AddProject<Projects.Agents_Conversation>("agents-conversation").
WithReference(blobs).WaitFor(storage)
    .WithReference(serviceBus).WaitFor(queue);


var build = builder.Build();
build.Run();