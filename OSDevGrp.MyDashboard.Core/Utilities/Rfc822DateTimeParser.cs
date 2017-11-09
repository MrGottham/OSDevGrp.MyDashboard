//* ------------------------------------------------------------------------
//*    Rfc822DateTimeParser - Parse Date-Time In RFC-822 Format
//*    Copyright (C) 2009  Tom Dong
//*
//*    This library is free software; you can redistribute it and/or
//*    modify it under the terms of the GNU Lesser General Public
//*    License as published by the Free Software Foundation; either
//*    version 2.1 of the License, or (at your option) any later version.
//* ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace OSDevGrp.MyDashboard.Core.Utilities
{
   /// <summary>
   /// Provides methods for parsing date-time information in RFC-822 format. This class cannot be inherited.
   /// </summary>
   /// <remarks>
   ///     See <a href="http://www.ietf.org/rfc/rfc0822.txt">RFC #822: Standard for ARPA Internet Text Messages (Date and Time Specification)</a>
   /// </remarks>
   public static class Rfc822DateTimeParser
   {
      #region Parse and TryParse

      /// <summary>
      /// Converts the specified string representation of a RFC-822 formatted date to its <see cref="DateTime"/> equivalent.
      /// </summary>
      /// <param name="s">A string containing a RFC-822 formatted date to convert.</param>
      /// <returns>A <see cref="DateTime"/> equivalent to the RFC-822 formatted date contained in <paramref name="s"/>.</returns>
      /// <exception cref="ArgumentNullException">The <paramref name="value"/> is a null reference.</exception>
      /// <exception cref="ArgumentNullException">The <paramref name="value"/> is an empty string.</exception>
      /// <exception cref="FormatException">The <paramref name="value"/> is not a recognized as a RFC-822 formatted date.</exception>
      public static DateTime Parse(string s)
      {
         return Parse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces);
      }

      /// <summary>
      /// Converts the specified string representation of a RFC-822 formatted date to its <see cref="DateTime"/> equivalent.
      /// </summary>
      /// <param name="s">A string containing a RFC-822 formatted date to convert.</param>
      /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about s. </param>
      /// <returns>A <see cref="DateTime"/> equivalent to the RFC-822 formatted date contained in <paramref name="s"/>.</returns>
      public static DateTime Parse(string s, IFormatProvider provider)
      {
         return Parse(s, provider, DateTimeStyles.AllowWhiteSpaces);
      }

      /// <summary>
      /// Converts the specified string representation of a RFC-822 formatted date to its <see cref="DateTime"/> equivalent.
      /// </summary>
      /// <param name="s">A string containing a RFC-822 formatted date to convert.</param>
      /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about s. </param>
      /// <param name="styles">A bitwise combination of <see cref="DateTimeStyles"/> values that indicates the permitted format of s.</param>
      /// <returns>A <see cref="DateTime"/> equivalent to the RFC-822 formatted date contained in <paramref name="s"/>.</returns>
      public static DateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
      {
         if (string.IsNullOrEmpty(s))
         {
            throw new ArgumentNullException(nameof(s));
         }

         DateTime result;
         if (!TryParse(s, provider, styles, out result))
         {
            throw new FormatException($"{s} is not a valid RFC 822 string representation of a date and time.");
         }
         return result;
      }
      
      /// <summary>
      /// Converts the specified string representation of a RFC-822 formatted date to its <see cref="DateTime"/> equivalent.
      /// </summary>
      /// <param name="s">A string containing a RFC-822 formatted date to convert.</param>
      /// <param name="result">
      ///     When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="DateTime.MinValue">MinValue</see> if the conversion failed. 
      ///     The conversion fails if the <paramref name="s"/> parameter is a <b>null</b> or empty string, or does not contain a valid string representation of a RFC-822 formatted date. 
      ///     This parameter is passed uninitialized.
      /// </param>
      /// <returns><b>true</b> if the <paramref name="value"/> parameter was converted successfully; otherwise, <b>false</b>.</returns>
      public static bool TryParse(string s, out DateTime result)
      {
         return TryParse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces, out result);
      }

      /// <summary>
      /// Converts the specified string representation of a RFC-822 formatted date to its <see cref="DateTime"/> equivalent.
      /// </summary>
      /// <param name="s">A string containing a RFC-822 formatted date to convert.</param>
      /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about s. </param>
      /// <param name="styles">A bitwise combination of <see cref="DateTimeStyles"/> values that indicates the permitted format of s.</param>
      /// <param name="result">
      ///     When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <paramref name="s"/>, if the conversion succeeded, or <see cref="DateTime.MinValue">MinValue</see> if the conversion failed. 
      ///     The conversion fails if the <paramref name="s"/> parameter is a <b>null</b> or empty string, or does not contain a valid string representation of a RFC-822 formatted date. 
      ///     This parameter is passed uninitialized.
      /// </param>
      /// <returns><b>true</b> if the <paramref name="value"/> parameter was converted successfully; otherwise, <b>false</b>.</returns>
      public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result)
      {
         result = DateTime.MinValue;
         if (string.IsNullOrEmpty(s))
         {
            return false;
         }

         string possiblyZone = GetLastSection(s);
         if (timezoneOffset.ContainsKey(possiblyZone))
         {
            int index = s.LastIndexOf(possiblyZone);
            s = s.Substring(0, index) + timezoneOffset[possiblyZone] + s.Substring(index + possiblyZone.Length);
         }
         return DateTime.TryParseExact(s, formatStrings, provider, styles, out result);
      }

      #endregion

      #region Constructor

      static Rfc822DateTimeParser()
      {
         InitOffest();
         formatStrings = FormatStringBuilder.Build(ComponentsOfFormatString, false);
      }

      #endregion

      #region Set Offset of Time Zones

      private static Dictionary<string,string> timezoneOffset = new Dictionary<string, string>();
      
      private static void InitOffest()
      {
         InitOffsetForUniversalTimeZone();
         InitOffsetForLetterTimeZones();
         InitOffsetForTimeZonesInNorthAmerica();
      }

      private static void InitOffsetForUniversalTimeZone()
      {
         timezoneOffset["GMT"] = timezoneOffset["UTC"] = timezoneOffset["UT"] = "+00:00";
      }

      private static void InitOffsetForLetterTimeZones()
      {
         string sign = "+";
         int currOffset = 1;
         for (char letter = 'A'; letter <= 'Z'; ++letter)
         {
            if (letter == 'J')
            {
               continue; // Letter J is not used.
            }
            
            if (letter == 'N')
            {
               sign = "-";
               currOffset = 1;
            }
            else if (letter == 'Z')
            {
               currOffset = 0;
            }

            timezoneOffset[letter.ToString()] = string.Format(sign + "{0:00}:00", currOffset++);
         }
      }

      private static void InitOffsetForTimeZonesInNorthAmerica()
      {
         // Time zones in North America
         timezoneOffset["EST"] = "-05:00";
         timezoneOffset["EDT"] = "-04:00";
         timezoneOffset["CST"] = "-06:00";
         timezoneOffset["CDT"] = "-05:00";
         timezoneOffset["MST"] = "-07:00";
         timezoneOffset["MDT"] = "-06:00";
         timezoneOffset["PST"] = "-08:00";
         timezoneOffset["PDT"] = "-07:00";
      
      }
      
      #endregion

      #region Set Format Strings

      private static string[] formatStrings;

      private static string[][] ComponentsOfFormatString
      {
         get
         {
            // These are possible components used to build a format string.
            // Note that spaces in these string are a little tricky.
            return new string[][]
            {
               // Day of Week, Day, Month and Year
               FormatStringBuilder.Build(ComponentsOfDayFormatString, false),
               // Hour, Minute and Second
               FormatStringBuilder.Build(ComponentsOfTimeFormatString, true),
               // Timezone. Strictly this is compulsory, but you can omit this for convenience,
               // and the Kind property of the resulting DateTime will be Unspecified.
               new string[] {string.Empty, " K"}
            };
         }
      }

      private static string[][] ComponentsOfTimeFormatString
      {
         get
         {
            // Strictly only 2-digit forms of hour, minute and second is allowed in RFC-822,
            // but 1-digit forms is also allowed here for convenience.
            return new string[][]
            { 
               // Hour.
               new string[] {" H", " HH"},
               // Minute.
               new string[] {":m", ":mm"},
               // Second, optional.
               new string[] {string.Empty, ":s", ":ss"},
            };
         }
      }
      private static string[][] ComponentsOfDayFormatString
      {
         get
         {
            return new string[][] 
            { 
               // Day of week, three letters, optional
               new string[] {string.Empty, "ddd, "},
               // Day, 1 or 2 digits
               new string[] {"d ", "dd "}, 
               // Month, 3 letters
               new string[] {"MMM "},
               // Year. Strictly only 2 digits is allowed in RFC-822, but 4-digit form is also
               // allowed here to meet RSS 2.0 Spec.
               new string[] {"yyyy", "yy"},
            };
         }
      }
      
      #endregion

      #region Get Last Section of a Given Date-Time String
      
      private static char[] punctuation = new char[]
      {
            '\u0009', '\u000A', '\u000B', '\u000C', '\u000D', 
            '\u0020', '\u00A0', '\u2000', '\u2001', '\u2002',
            '\u2003', '\u2004', '\u2005', '\u2006', '\u2007',
            '\u2008', '\u2009', '\u200A', '\u3000'
      };

      private static string GetLastSection(string s)
      {
         var sections = s.Split(punctuation, StringSplitOptions.RemoveEmptyEntries);
         if (sections.Length == 0)
         {
            return string.Empty;
         }
        return sections.Last();
      }
      
      #endregion
   }

   /// <summary>
   /// Provides methods for build up date-time format strings from given components.
   /// </summary>
   static class FormatStringBuilder
   {
      public static string[] Build(string[][] formatParts, bool includeEmpty)
      {
         List<string> formats = new List<string>();
         if (includeEmpty)
         {
            formats.Add(string.Empty);
         }

         int[] buildIndex = new int[formatParts.Length];
         while (buildIndex[buildIndex.Length - 1] < formatParts[buildIndex.Length - 1].Length)
         {
            string newFormat = string.Empty;
            for (int i = 0; i < buildIndex.Length; ++i)
            {
               newFormat += formatParts[i][buildIndex[i]];
            }
            formats.Add(newFormat);
            NextBuildIndex(buildIndex, formatParts);
         }
         return formats.ToArray();
      }

      private static void NextBuildIndex(int[] buildIndex, string[][] formatParts)
      {
         for (int i = 0; ; ++i)
         {
            ++buildIndex[i];
            if (buildIndex[i] == formatParts[i].Length && i != buildIndex.Length - 1)
            {
               buildIndex[i] = 0;
            }
            else
            {
               return;
            }
         }
      }
   }
}