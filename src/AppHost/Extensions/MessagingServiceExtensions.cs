using Aspire.Hosting.Azure;

namespace AppHost.Extensions;

public static class MessagingServiceExtensions
{
    public static IResourceBuilder<AzureServiceBusResource> AddServiceBusServices(
        this IDistributedApplicationBuilder builder)
    {
        var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator(emu => emu.WithLifetime(ContainerLifetime.Persistent));

        return serviceBus;
    }
}