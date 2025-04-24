using System;
using System.IO;
using System.Reflection;

namespace OSDevGrp.MyDashboard.Web.Resolvers;

internal static class LinkerTimestampResolver
{
    #region Methods

    internal static DateTimeOffset GetLinkerTimestamp(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        return GetLinkerTimestamp(assembly.Location);
    }

    private static DateTimeOffset GetLinkerTimestamp(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        return new DateTimeOffset(File.GetCreationTimeUtc(filePath), TimeSpan.Zero);
    }

    #endregion
}