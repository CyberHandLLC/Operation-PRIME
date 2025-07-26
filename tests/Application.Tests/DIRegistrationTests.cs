using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OperationPrime.Application.Interfaces;
using OperationPrime.Infrastructure;
using Xunit;

namespace Application.Tests;

public class DIRegistrationTests
{
    [Fact]
    public void AddInfrastructure_RegistersRequiredServices()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IIncidentService>());
        Assert.NotNull(provider.GetService<IPriorityService>());
        Assert.NotNull(provider.GetService<INOIService>());
        Assert.NotNull(provider.GetService<IPreIncidentWorkflowService>());
        Assert.NotNull(provider.GetService<IMajorIncidentWorkflowService>());
    }
}
