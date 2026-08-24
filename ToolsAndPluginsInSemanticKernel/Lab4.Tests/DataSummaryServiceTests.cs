using Lab4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Web;
using Moq;
using NUnit.Framework;

#pragma warning disable SKEXP0050

namespace lab4.tests;

public class DataSummaryServiceTests
{
    private Mock<IChatCompletionService> _chatCompletionServiceMock = null!;
    private Mock<IWebSearchEngineConnector> _searchConnectorMock = null!;
    private Kernel _kernel = null!;

    [SetUp]
    public void SetUp()
    {
        _chatCompletionServiceMock = new Mock<IChatCompletionService>();
        _searchConnectorMock = new Mock<IWebSearchEngineConnector>();

        // The service resolves IChatCompletionService via _kernel.GetRequiredService,
        // so we register the mock into the Kernel's own DI container.
        var builder = Kernel.CreateBuilder();
        builder.Services.AddSingleton(_chatCompletionServiceMock.Object);
        _kernel = builder.Build();
    }

    [Test]
    public void Constructor_NullKernel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DataSummaryService(null!, _searchConnectorMock.Object));
    }

    [Test]
    public void Constructor_RegistersExpectedPlugins()
    {
        _ = new DataSummaryService(_kernel, _searchConnectorMock.Object);

        Assert.Multiple(() =>
        {
            Assert.That(_kernel.Plugins.Contains("time"), Is.True);
            Assert.That(_kernel.Plugins.Contains("webSearch"), Is.True);
            Assert.That(_kernel.Plugins.Contains("custom"), Is.True);
        });
    }
}
