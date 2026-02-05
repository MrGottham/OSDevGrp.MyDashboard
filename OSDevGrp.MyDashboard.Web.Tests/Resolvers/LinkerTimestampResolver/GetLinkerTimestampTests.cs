using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Web.Tests.Resolvers;

[TestClass]
public class GetLinkerTimestampTests
{
    [TestMethod]
    public void GetLinkerTimestamp_WhenAssemblyIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => Web.Resolvers.LinkerTimestampResolver.GetLinkerTimestamp(null));

        Assert.AreEqual("assembly", result.ParamName);
    }

    [TestMethod]
    public void GetLinkerTimestamp_WhenCalledWithAssembly_ReturnsLinkerTimestampGreaterThanMinValue()
    {
        Assembly assembly = GetAssembly();
        DateTimeOffset result = Web.Resolvers.LinkerTimestampResolver.GetLinkerTimestamp(assembly);

        Assert.IsTrue(result > DateTimeOffset.MinValue);
    }

    [TestMethod]
    public void GetLinkerTimestamp_WhenCalledWithAssembly_ReturnsExpectedLinkerTimestamp()
    {
        Assembly assembly = GetAssembly();
        DateTimeOffset result = Web.Resolvers.LinkerTimestampResolver.GetLinkerTimestamp(assembly);

        Assert.AreEqual(GetLinkerTimestamp(), result);
    }

    [TestMethod]
    public void GetLinkerTimestamp_WhenCalledWithAssembly_ReturnsLinkerTimestampAsUtc()
    {
        Assembly assembly = GetAssembly();
        DateTimeOffset result = Web.Resolvers.LinkerTimestampResolver.GetLinkerTimestamp(assembly);

        Assert.AreEqual(TimeSpan.Zero, result.Offset);
    }

    private static Assembly GetAssembly()
    {
        return typeof(GetLinkerTimestampTests).Assembly;
    }

    private static DateTimeOffset GetLinkerTimestamp()
    {
        return new DateTimeOffset(File.GetCreationTimeUtc(GetAssembly().Location), TimeSpan.Zero);
    }
}