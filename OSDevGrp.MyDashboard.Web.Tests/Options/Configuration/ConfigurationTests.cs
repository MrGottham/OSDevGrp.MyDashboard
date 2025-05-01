using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Web.Options;

namespace OSDevGrp.MyDashboard.Web.Tests.Options.Configuration;

[TestClass]
public class ConfigurationTests : ConfigurationTestBase
{
    [TestMethod]
    public void Configuration_WhenCalledWithRedditClientIdKey_ReturnsNotNull()
    {
        IConfiguration sut = CreateSut();

        string result = sut[ConfigurationKeys.RedditClientIdKey];

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Configuration_WhenCalledWithRedditClientIdKey_ReturnsNonEmptyValue()
    {
        IConfiguration sut = CreateSut();

        string result = sut[ConfigurationKeys.RedditClientIdKey];

        Assert.IsNotEmpty(result);
    }

    [TestMethod]
    public void Configuration_WhenCalledWithRedditClientSecretKey_ReturnsNotNull()
    {
        IConfiguration sut = CreateSut();

        string result = sut[ConfigurationKeys.RedditClientSecretKey];

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Configuration_WhenCalledWithRedditClientSecretKey_ReturnsNonEmptyValue()
    {
        IConfiguration sut = CreateSut();

        string result = sut[ConfigurationKeys.RedditClientSecretKey];

        Assert.IsNotEmpty(result);
    }

    private IConfiguration CreateSut()
    {
        return CreateTestConfiguration();
    }
}