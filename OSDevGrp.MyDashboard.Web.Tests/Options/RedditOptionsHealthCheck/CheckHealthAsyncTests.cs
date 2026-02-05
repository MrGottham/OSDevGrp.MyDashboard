using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Options;
using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Tests.Options.RedditOptionsHealthCheck;

[TestClass]
public class CheckHealthAsyncTests
{
    #region Private variables

    private Mock<IOptions<RedditOptions>> _redditOptionsMock;

    #endregion

    [TestInitialize]
    public void TestInitialize()
    {
        _redditOptionsMock = new Mock<IOptions<RedditOptions>>();
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenHealthCheckContextIsNull_ThrowsArgumentNullException()
    {
        IHealthCheck sut = CreateSut();

        ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CheckHealthAsync(null));

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenHealthCheckContextIsNull_ThrowsArgumentNullExceptionWhereParamNameIsEqualToContext()
    {
        IHealthCheck sut = CreateSut();

        ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CheckHealthAsync(null));

        Assert.AreEqual("context", result.ParamName);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenHealthCheckContextIsNull_ThrowsArgumentNullExceptionWhereInnerExceptionIsNull()
    {
        IHealthCheck sut = CreateSut();

        ArgumentNullException result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.CheckHealthAsync(null));

        Assert.IsNull(result.InnerException);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenCalled_AssertValueWasCalledOnRedditOptions()
    {
        IHealthCheck sut = CreateSut();

        HealthCheckContext context = CreateHealthCheckContext();
        await sut.CheckHealthAsync(context);

        _redditOptionsMock.Verify(m => m.Value, Times.Once);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenRedditOptionsIsNotSet_ReturnsNotNull()
    {
        IHealthCheck sut = CreateSut(hasRedditOptions: false);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenRedditOptionsIsNotSet_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        IHealthCheck sut = CreateSut(hasRedditOptions: false);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context)!;

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenRedditOptionsIsNotSet_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        IHealthCheck sut = CreateSut(hasRedditOptions: false);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsNull_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsNull_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsNull_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientId)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsEmpty_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsEmpty_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsEmpty_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientId)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsWhiteSpace_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsWhiteSpace_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientIdInRedditOptionsIsWhiteSpace_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientId: true, clientId: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientId)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsNull_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsNull_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsNull_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: false);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientSecret)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsEmpty_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsEmpty_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsEmpty_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: string.Empty);
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientSecret)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsWhiteSpace_ReturnsNotNull()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsWhiteSpace_ReturnsHealthCheckResultWhereStatusIsEqualToUnhealthy()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenClientSecretInRedditOptionsIsWhiteSpace_ReturnsHealthCheckResultWhereDescriptionIsEqualToSpecificText()
    {
        RedditOptions redditOptions = CreateRedditOptions(hasClientSecret: true, clientSecret: " ");
        IHealthCheck sut = CreateSut(hasRedditOptions: true, redditOptions: redditOptions);

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual($"{nameof(redditOptions.ClientSecret)} in {nameof(RedditOptions)} are misconfigured.", result.Description);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenRedditOptionsHasValidConfiguratoion_ReturnsNotNull()
    {
        IHealthCheck sut = CreateSut();

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult? result = await sut.CheckHealthAsync(context);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CheckHealthAsync_WhenRedditOptionsHasValidConfiguratoion_ReturnsHealthCheckResultWhereStatusIsEqualToHealthy()
    {
        IHealthCheck sut = CreateSut();

        HealthCheckContext context = CreateHealthCheckContext();
        HealthCheckResult result = await sut.CheckHealthAsync(context);

        Assert.AreEqual(HealthStatus.Healthy, result.Status);
    }

    private IHealthCheck CreateSut(bool hasRedditOptions = true, RedditOptions redditOptions = null)
    {
        _redditOptionsMock.Setup(m => m.Value)
            .Returns(hasRedditOptions ? redditOptions ?? CreateRedditOptions(hasRedditOptions) : null);

        return new Web.Options.RedditOptionsHealthCheck(_redditOptionsMock.Object);
    }

    private static RedditOptions CreateRedditOptions(bool hasClientId = true, string clientId = null, bool hasClientSecret = true, string clientSecret = null)
    {
        return new RedditOptions
        {
            ClientId = hasClientId ? clientId ?? Guid.NewGuid().ToString("N") : null,
            ClientSecret = hasClientSecret ? clientSecret ?? Guid.NewGuid().ToString("N") : null
        };
    }

    private HealthCheckContext CreateHealthCheckContext()
    {
        return new HealthCheckContext();
    }
}