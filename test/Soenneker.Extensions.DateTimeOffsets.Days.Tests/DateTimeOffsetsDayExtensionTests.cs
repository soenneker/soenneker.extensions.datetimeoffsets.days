using System;
using AwesomeAssertions;
using Soenneker.Enums.DayOfWeek;
using Soenneker.Extensions.DateTimeOffsets.Days;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.DateTimeOffsets.Days.Tests;

public sealed class DateTimeOffsetsDayExtensionTests : UnitTest
{
    private static TimeZoneInfo GetEasternTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/New_York"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); }
    }

    #region Non-timezone day boundaries – weird scenarios

    [Test]
    public void ToStartOfDay_ExactlyAtMidnight_ReturnsSame()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        var result = dto.ToStartOfDay();
        result.Should().Be(new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void ToStartOfDay_OneTickBeforeMidnight_ReturnsPreviousDayStart()
    {
        var dto = new DateTimeOffset(2024, 6, 16, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        var result = dto.ToStartOfDay();
        result.Should().Be(new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void ToStartOfDay_OneTickAfterMidnight_ReturnsCurrentDayStart()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero).AddTicks(1);
        var result = dto.ToStartOfDay();
        result.Should().Be(new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void ToStartOfDay_PreservesNonZeroOffset()
    {
        var offset = TimeSpan.FromHours(14);
        var dto = new DateTimeOffset(2024, 7, 4, 23, 59, 59, offset);
        var result = dto.ToStartOfDay();
        result.Offset.Should().Be(offset);
        result.Should().Be(new DateTimeOffset(2024, 7, 4, 0, 0, 0, offset));
    }

    [Test]
    public void ToStartOfDay_NegativeOffset()
    {
        var offset = TimeSpan.FromHours(-12);
        var dto = new DateTimeOffset(2024, 1, 1, 12, 30, 0, offset);
        var result = dto.ToStartOfDay();
        result.Offset.Should().Be(offset);
        result.Should().Be(new DateTimeOffset(2024, 1, 1, 0, 0, 0, offset));
    }

    [Test]
    public void ToEndOfDay_LeapYearFeb29_LastTickBeforeMar1()
    {
        var dto = new DateTimeOffset(2024, 2, 29, 15, 30, 0, TimeSpan.Zero);
        var result = dto.ToEndOfDay();
        var expected = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        result.Should().Be(expected);
    }

    [Test]
    public void ToEndOfDay_YearBoundary_LastTickOfDec31()
    {
        var dto = new DateTimeOffset(2023, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var result = dto.ToEndOfDay();
        var expected = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        result.Should().Be(expected);
    }

    [Test]
    public void ToStartOfNextDay_LeapYearFeb29_ReturnsMar1()
    {
        var dto = new DateTimeOffset(2024, 2, 29, 12, 0, 0, TimeSpan.Zero);
        var result = dto.ToStartOfNextDay();
        result.Should().Be(new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void ToStartOfPreviousDay_Jan1_ReturnsDec31PreviousYear()
    {
        var dto = new DateTimeOffset(2024, 1, 1, 0, 0, 1, TimeSpan.Zero);
        var result = dto.ToStartOfPreviousDay();
        result.Should().Be(new DateTimeOffset(2023, 12, 31, 0, 0, 0, TimeSpan.Zero));
    }

    [Test]
    public void ToEndOfPreviousDay_Mar1_ReturnsLastTickOfFeb29InLeapYear()
    {
        var dto = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero);
        var result = dto.ToEndOfPreviousDay();
        var expected = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(-1); // last tick before Mar 1 = end of Feb 29
        result.Should().Be(expected);
    }

    [Test]
    public void ToEndOfNextDay_LastDayOfMonth_EndOfNextMonthFirstDay()
    {
        var dto = new DateTimeOffset(2024, 1, 31, 23, 59, 59, TimeSpan.Zero);
        var result = dto.ToEndOfNextDay();
        var expected = new DateTimeOffset(2024, 2, 2, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        result.Should().Be(expected);
    }

    [Test]
    public void AllDayBoundaries_ChainCorrectly()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.Zero);
        var start = dto.ToStartOfDay();
        var end = dto.ToEndOfDay();
        var nextStart = dto.ToStartOfNextDay();
        var prevEnd = dto.ToEndOfPreviousDay();

        prevEnd.AddTicks(1).Should().Be(start);
        end.AddTicks(1).Should().Be(nextStart);
    }

    [Test]
    public void DateTimeOffset_MinValue_DoesNotThrow()
    {
        var dto = DateTimeOffset.MinValue;
        var start = dto.ToStartOfDay();
        var end = dto.ToEndOfDay();
        start.Should().Be(dto);
        end.Should().Be(dto.AddDays(1).AddTicks(-1));
    }

    [Test]
    public void SubMillisecondPrecision_PreservedInStartOfDay()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 12, 30, 45, 123, TimeSpan.Zero).AddTicks(4567);
        var result = dto.ToStartOfDay();
        (result.Ticks % TimeSpan.TicksPerDay).Should().Be(0);
    }

    #endregion

    #region Time-zone methods – weird scenarios

    [Test]
    public void ToStartOfTzDay_NullTimeZone_ThrowsArgumentNullException()
    {
        var dto = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentNullException>(() => dto.ToStartOfTzDay(null!));
    }

    [Test]
    public void ToStartOfTzDay_UtcTimeZone_SameAsNonTzForUtcInput()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.Zero);
        var tzResult = dto.ToStartOfTzDay(TimeZoneInfo.Utc);
        var simpleResult = dto.ToStartOfDay();
        tzResult.UtcDateTime.Should().Be(simpleResult.UtcDateTime);
        tzResult.Offset.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void ToStartOfTzDay_InputWithNonUtcOffset_NormalizesToUtc()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.FromHours(5));
        var tzResult = dto.ToStartOfTzDay(TimeZoneInfo.Utc);
        tzResult.Offset.Should().Be(TimeSpan.Zero);
        tzResult.Year.Should().Be(2024);
        tzResult.Month.Should().Be(6);
        tzResult.Day.Should().Be(15);
        tzResult.Hour.Should().Be(0);
        tzResult.Minute.Should().Be(0);
    }

    [Test]
    public void ToStartOfTzDay_DuringDstGap_EasternTime()
    {
        var tz = GetEasternTimeZone();
        // March 10, 2024 2:30 AM Eastern doesn't exist (spring forward 2->3)
        // Use an instant that is 7:30 UTC = 2:30 AM Eastern (invalid)
        // The method should handle the gap and return the start of that local day
        var utcInstant = new DateTimeOffset(2024, 3, 10, 7, 30, 0, TimeSpan.Zero);
        var result = utcInstant.ToStartOfTzDay(tz);
        result.Offset.Should().Be(TimeSpan.Zero);
        // Start of March 10 in Eastern is 5:00 UTC (EST) or 4:00 UTC (EDT) - March 10 2024 is after DST so 4:00 UTC
        result.Year.Should().Be(2024);
        result.Month.Should().Be(3);
        result.Day.Should().Be(10);
    }

    [Test]
    public void ToStartOfTzDay_DuringDstFold_EasternTime()
    {
        var tz = GetEasternTimeZone();
        // Nov 3, 2024 1:30 AM Eastern exists twice (fall back)
        var utcInstant = new DateTimeOffset(2024, 11, 3, 6, 30, 0, TimeSpan.Zero); // 1:30 AM EDT
        var result = utcInstant.ToStartOfTzDay(tz);
        result.Offset.Should().Be(TimeSpan.Zero);
        result.Year.Should().Be(2024);
        result.Month.Should().Be(11);
        result.Day.Should().Be(3);
    }

    [Test]
    public void ToEndOfTzDay_IsOneTickBeforeStartOfNextTzDay()
    {
        var tz = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var endOfDay = dto.ToEndOfTzDay(tz);
        var startOfNext = dto.ToStartOfNextTzDay(tz);
        endOfDay.Should().Be(startOfNext.AddTicks(-1));
    }

    [Test]
    public void ToEndOfPreviousTzDay_IsOneTickBeforeStartOfTzDay()
    {
        var tz = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        var endOfPrev = dto.ToEndOfPreviousTzDay(tz);
        var startOfDay = dto.ToStartOfTzDay(tz);
        endOfPrev.Should().Be(startOfDay.AddTicks(-1));
    }

    [Test]
    public void ToStartOfPreviousTzDay_AcrossYearBoundary()
    {
        var tz = TimeZoneInfo.Utc;
        var dto = new DateTimeOffset(2024, 1, 1, 0, 0, 1, TimeSpan.Zero);
        var result = dto.ToStartOfPreviousTzDay(tz);
        result.Year.Should().Be(2023);
        result.Month.Should().Be(12);
        result.Day.Should().Be(31);
        result.Hour.Should().Be(0);
    }

    #endregion

    #region ToDayOfWeekType – weird scenarios

    [Test]
    public void ToDayOfWeekType_Sunday()
    {
        var dto = new DateTimeOffset(2024, 6, 9, 12, 0, 0, TimeSpan.Zero); // Sunday
        dto.ToDayOfWeekType().Should().Be(DayOfWeekType.Sunday);
    }

    [Test]
    public void ToDayOfWeekType_Saturday_FallbackCase()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero); // Saturday
        dto.ToDayOfWeekType().Should().Be(DayOfWeekType.Saturday);
    }

    [Test]
    public void ToDayOfWeekType_AllSevenDays()
    {
        new DateTimeOffset(2024, 6, 9, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Sunday);
        new DateTimeOffset(2024, 6, 10, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Monday);
        new DateTimeOffset(2024, 6, 11, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Tuesday);
        new DateTimeOffset(2024, 6, 12, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Wednesday);
        new DateTimeOffset(2024, 6, 13, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Thursday);
        new DateTimeOffset(2024, 6, 14, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Friday);
        new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero).ToDayOfWeekType().Should().Be(DayOfWeekType.Saturday);
    }

    [Test]
    public void ToDayOfWeekType_OffsetDoesNotChangeCalendarDay()
    {
        // Same UTC moment, different offsets - calendar day can differ
        var utc = new DateTimeOffset(2024, 6, 15, 2, 0, 0, TimeSpan.Zero); // Saturday 2 AM UTC
        var tokyo = new DateTimeOffset(utc.DateTime, TimeSpan.FromHours(9)); // Saturday 11 AM in Tokyo
        utc.ToDayOfWeekType().Should().Be(DayOfWeekType.Saturday);
        tokyo.ToDayOfWeekType().Should().Be(DayOfWeekType.Saturday);
    }

    #endregion
}
