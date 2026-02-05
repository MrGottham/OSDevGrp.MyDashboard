using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OSDevGrp.MyDashboard.Core.Tests.Utilities.Rfc822DateTimeParser
{
    [TestClass]
    public class ParseTests
    {
        [TestMethod]
        public void Parse_WhenCalledWithUniveralTime_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44 GMT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44 UT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44 UTC", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithNorthAmericaTimeZones_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 06:36:44 EST", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 07:36:44 EDT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 05:36:44 CST", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 06:36:44 CDT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 04:36:44 MST", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 05:36:44 MDT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 03:36:44 PST", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 04:36:44 PDT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithLetterTimeZone_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);

            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 12:36:44 A", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 13:36:44 B", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 14:36:44 C", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 15:36:44 D", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 16:36:44 E", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 17:36:44 F", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 18:36:44 G", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 19:36:44 H", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 20:36:44 I", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 21:36:44 K", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 22:36:44 L", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 23:36:44 M", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 10:36:44 N", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 09:36:44 O", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 08:36:44 P", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 07:36:44 Q", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 06:36:44 R", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 05:36:44 S", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 04:36:44 T", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 03:36:44 U", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 02:36:44 V", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 01:36:44 W", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 00:36:44 X", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Wed, 05 Aug 2009 23:36:44 Y", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44 Z", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithFourDigitTimeZone_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 13, 53, 44, DateTimeKind.Utc);

            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 10:23:44 -0330", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 15:53:44 +0200", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Fri, 07 Aug 2009 1:53:44 +1200", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 1:53:44 -1200", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 13:53:44 -0000", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 13:53:44 +0000", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithOneDigitInDayHourMinuteSecond_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 9, 6, 1, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 6 Aug 2009 09:06:01 GMT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 9:06:01 GMT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 09:6:01 GMT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 09:06:1 GMT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("Thu, 6 Aug 2009 9:6:1 GMT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithTwoDigitsInYear_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 9, 6, 1, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 09 09:06:01 GMT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithInvalidFormat_ThrowsException()
        {
            Parse_WhenCalledWithString_ThrowsException<ArgumentNullException>(null);
            Parse_WhenCalledWithString_ThrowsException<FormatException>("This a invalid string");
            Parse_WhenCalledWithString_ThrowsException<ArgumentNullException>("");
            // Should be Thu instead of Wed
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 11:36:44 UT");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 24:36:44 UT");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 11:60:44 UT");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 11:36:60 UT");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed,, 06 Aug 2009 11:36:44 UT");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 11:36:44 SB");
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Wed, 06 Aug 2009 11:36:44 J");
            // Invalid arguments
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Thu, 06 Aug 2009 11:36:44 UT ", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
            Parse_WhenCalledWithString_ThrowsException<FormatException>(" Thu, 06 Aug 2009 11:36:44 UT", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Thu,  06 Aug 2009 11:36:44 UT", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
            Parse_WhenCalledWithString_ThrowsException<FormatException>("Thu, 06  Aug 2009 11:36:44 UT", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithLeapYear_ThrowsException()
        {
            DateTime utcTime = new DateTime(2008, 2, 29, 11, 36, 44, DateTimeKind.Utc);

            Parse_WhenCalledWithString_ThrowsException<FormatException>("Mon, 29 Feb 2009 11:36:44 GMT");
            Parse_WhenCalledWithString_ExpectTime("Fri, 29 Feb 2008 11:36:44 UT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithTimeZoneNotSpecified_ExpectTime()
        {
            DateTime time = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);
            
            DateTime result = Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44", time, false);
            Assert.AreEqual(DateTimeKind.Unspecified, result.Kind);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithSecondsOmitted_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 00, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36 GMT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithDayOfWeekOmitted_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("06 Aug 2009 11:36:44 UT", utcTime);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithOnlyDateNoTime_ExpectDate()
        {
            Parse_WhenCalledWithString_ExpectDate("Thu, 06 Aug 2009 CDT", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("06 Aug 2009 CDT", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("Thu, 06 Aug 2009", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("06 Aug 2009", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("Thu, 06 Aug 09 CDT", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("06 Aug 09 CDT", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("Thu, 06 Aug 09", 6, 8, 2009);
            Parse_WhenCalledWithString_ExpectDate("06 Aug 09", 6, 8, 2009);
        }
        
        [TestMethod]
        public void Parse_WhenCalledWithExtraWhiteSpaces_ExpectUtcTime()
        {
            DateTime utcTime = new DateTime(2009, 8, 6, 11, 36, 44, DateTimeKind.Utc);
            
            Parse_WhenCalledWithString_ExpectTime("Thu, 06 Aug 2009 11:36:44 UT\n ", utcTime);
            Parse_WhenCalledWithString_ExpectTime("\nThu, 06 Aug 2009 11:36:44 UT", utcTime);
            Parse_WhenCalledWithString_ExpectTime("\nThu,   06\n Aug\t2009   11:36:44 UT", utcTime);
        }

        private DateTime Parse_WhenCalledWithString_ExpectTime(string value, DateTime expectedTime, bool asUtcTime = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            DateTime result = OSDevGrp.MyDashboard.Core.Utilities.Rfc822DateTimeParser.Parse(value);
            if (asUtcTime)
            {
                result = result.ToUniversalTime();
            }
            Assert.AreEqual(expectedTime, result);

            return result;
        }

        private void Parse_WhenCalledWithString_ExpectDate(string value, int expectedDay, int expectedMonth, int expectedYear)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            DateTime result = OSDevGrp.MyDashboard.Core.Utilities.Rfc822DateTimeParser.Parse(value);
            Assert.AreEqual(expectedDay, result.Day);
            Assert.AreEqual(expectedMonth, result.Month);
            Assert.AreEqual(expectedYear, result.Year);
        }

        private void Parse_WhenCalledWithString_ThrowsException<TException>(string value, IFormatProvider formatProvider = null, DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces) where TException : Exception
        {
            try
            {
                OSDevGrp.MyDashboard.Core.Utilities.Rfc822DateTimeParser.Parse(value, formatProvider ?? DateTimeFormatInfo.InvariantInfo, styles);

                Assert.Fail($"An {typeof(TException)} was expected.");
            }
            catch (TException)
            {
            }
            catch
            {
                Assert.Fail($"An {typeof(TException)} was expected.");
            }
        }
    }
}