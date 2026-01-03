using System;
using System.Diagnostics.Contracts;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTimeOffsets.Days;

/// <summary>
/// Provides extension methods for <see cref="DateTimeOffset"/> to facilitate day-based operations.
/// This includes getting the start or end of the current, previous, or next day, with considerations for specific time zones.
/// </summary>
public static class DateTimeOffsetsDayExtension
{
    /// <summary>
    /// Adjusts the given <paramref name="dateTimeOffset"/> to the start of the current day (i.e., 00:00:00 or 12:00 AM).
    /// </summary>
    /// <param name="dateTimeOffset">The datetime to adjust.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the current day of the input date.</returns>
    /// <remarks>
    /// This method preserves the time zone offset of the provided DateTimeOffset.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToStartOf(UnitOfTime.Day);
    }

    /// <summary>
    /// Adjusts the given <paramref name="dateTimeOffset"/> to the end of the current day (i.e., 23:59:59.9999999 or one tick before midnight).
    /// </summary>
    /// <param name="dateTimeOffset">The datetime to adjust.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the very end of the current day of the input date.</returns>
    /// <remarks>
    /// This method preserves the time zone offset of the provided DateTimeOffset. It effectively goes to the next day and subtracts a single tick.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToEndOf(UnitOfTime.Day);
    }

    /// <summary>
    /// Adjusts the given <paramref name="dateTimeOffset"/> to the start of the next day.
    /// </summary>
    /// <param name="dateTimeOffset">The datetime to adjust.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the day following the input date.</returns>
    /// <remarks>
    /// This method preserves the time zone offset of the provided DateTimeOffset.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfNextDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToStartOfDay().AddDays(1);
    }

    /// <summary>
    /// Adjusts the given <paramref name="dateTimeOffset"/> to the start of the previous day.
    /// </summary>
    /// <param name="dateTimeOffset">The datetime to adjust.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the day prior to the input date.</returns>
    /// <remarks>
    /// This method preserves the time zone offset of the provided DateTimeOffset.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToStartOfDay().AddDays(-1);
    }

    /// <summary>
    /// Extends the <see cref="DateTimeOffset"/> struct with a method to get the end of the previous day.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> value to calculate the end of the previous day from.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the end of the previous day (23:59:59.9999999) based on the input <paramref name="dateTimeOffset"/> value.</returns>
    /// <example>
    /// For example, if the input <paramref name="dateTimeOffset"/> is "2023-04-01 12:34:56", the method will return "2023-03-31 23:59:59.9999999".
    /// </example>
    /// <remarks>
    /// This method is marked as <c>Pure</c>, which means it has no side effects and its return value is solely determined by its input value.
    /// It uses the <see cref="ToEndOfDay"/> method to get the end of the current day, and then subtracts one day using <see cref="DateTimeOffset.AddDays(double)"/> to get the end of the previous day.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToEndOfDay().AddDays(-1);
    }

    /// <summary>
    /// Extends the <see cref="DateTimeOffset"/> struct with a method to get the end of the next day.
    /// </summary>
    /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> value to calculate the end of the next day from.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the end of the next day (23:59:59.9999999) based on the input <paramref name="dateTimeOffset"/> value.</returns>
    /// <example>
    /// For example, if the input <paramref name="dateTimeOffset"/> is "2023-04-01 12:34:56", the method will return "2023-04-02 23:59:59.9999999".
    /// </example>
    /// <remarks>
    /// This method is marked as <c>Pure</c>, which means it has no side effects and its return value is solely determined by its input value.
    /// It uses the <see cref="ToEndOfDay"/> method to get the end of the current day, and then adds one day using <see cref="DateTimeOffset.AddDays(double)"/> to get the end of the next day.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextDay(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToEndOfDay().AddDays(1);
    }

    /// <summary>
    /// Converts the given UTC datetime (<paramref name="utcNow"/>) to the timezone specified by <paramref name="tzInfo"/>, 
    /// adjusts it to the start of the current day in that timezone, then converts back to UTC.
    /// </summary>
    /// <param name="utcNow">The current UTC datetime.</param>
    /// <param name="tzInfo">The timezone information to use for the conversion.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the current day in the specified timezone, converted back to UTC.</returns>
    /// <remarks>
    /// This method facilitates timezone-specific datetime calculations, ensuring the output is in UTC for consistent further processing.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToStartOfDay().ToUtc();
    }

    /// <summary>
    /// Converts the given UTC datetime (<paramref name="utcNow"/>) to the timezone specified by <paramref name="tzInfo"/>, 
    /// adjusts it to the start of the previous day in that timezone, then converts back to UTC.
    /// </summary>
    /// <param name="utcNow">The current UTC datetime.</param>
    /// <param name="tzInfo">The timezone information to use for the conversion.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the previous day in the specified timezone, converted back to UTC.</returns>
    /// <remarks>
    /// This method is useful for adjusting datetimes across timezones and ensuring the result is in UTC.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToStartOfPreviousDay().ToUtc();
    }

    /// <summary>
    /// Converts the given UTC datetime (<paramref name="utcNow"/>) to the timezone specified by <paramref name="tzInfo"/>, 
    /// adjusts it to the start of the next day in that timezone, then converts back to UTC.
    /// </summary>
    /// <param name="utcNow">The current UTC datetime.</param>
    /// <param name="tzInfo">The timezone information to use for the conversion.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the start of the next day in the specified timezone, converted back to UTC.</returns>
    /// <remarks>
    /// This method accounts for timezone differences and is useful for date calculations across timezones, with results standardized to UTC.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToStartOfNextTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToStartOfNextDay().ToUtc();
    }

    /// <summary>
    /// Calculates the very last moment of the current day in the specified timezone (<paramref name="tzInfo"/>) from the given UTC datetime (<paramref name="utcNow"/>), then converts it back to UTC.
    /// </summary>
    /// <param name="utcNow">The current UTC datetime.</param>
    /// <param name="tzInfo">The timezone information to use for the calculation.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the very last tick of the current day in the specified timezone, converted back to UTC.</returns>
    /// <remarks>
    /// Useful for end-of-day calculations across timezones. The result is adjusted to UTC to facilitate universal application.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToEndOfDay().ToUtc();
    }

    /// <summary>
    /// Calculates the very last moment of the previous day in the specified timezone (<paramref name="tzInfo"/>) from the given UTC datetime (<paramref name="utcNow"/>), then converts it back to UTC.
    /// </summary>
    /// <param name="utcNow">The current UTC datetime.</param>
    /// <param name="tzInfo">The timezone information to use for the calculation.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the very last tick of the previous day in the specified timezone, converted back to UTC.</returns>
    /// <remarks>
    /// This method ensures that end-of-day times are accurately reflected across different timezones, with the final result in UTC.
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToEndOfPreviousDay().ToUtc();
    }

    /// <summary>
    /// Extends the <see cref="DateTimeOffset"/> struct with a method to get the end of the next day in a specified time zone.
    /// </summary>
    /// <param name="utcNow">The <see cref="DateTimeOffset"/> value in UTC to calculate the end of the next day from.</param>
    /// <param name="tzInfo">The <see cref="TimeZoneInfo"/> representing the target time zone.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> instance representing the end of the next day (23:59:59.9999999) in the specified time zone, based on the input <paramref name="utcNow"/> value.</returns>
    /// <example>
    /// For example, if the input <paramref name="utcNow"/> is "2023-04-01 12:34:56" (in UTC) and the <paramref name="tzInfo"/> is "Eastern Standard Time", the method will return a <see cref="DateTimeOffset"/> value representing "2023-04-02 23:59:59.9999999" in the Eastern Time Zone.
    /// </example>
    /// <remarks>
    /// This method is marked as <c>Pure</c>, which means it has no side effects and its return value is solely determined by its input values.
    /// It uses the following steps:
    /// <list type="number">
    /// <item>
    /// <description>Converts the input <paramref name="utcNow"/> value from UTC to the specified time zone using the <see cref="DateTimeOffsetExtension.ToTz(DateTimeOffset,TimeZoneInfo)"/> method.</description>
    /// </item>
    /// <item>
    /// <description>Calls the <see cref="ToEndOfNextDay"/> extension method on the converted <see cref="DateTimeOffset"/> value to get the end of the next day in the specified time zone.</description>
    /// </item>
    /// <item>
    /// <description>Converts the resulting <see cref="DateTimeOffset"/> value back to UTC using the <see cref="DateTimeOffsetExtension.ToUtc(DateTimeOffset)"/> method.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextTzDay(this DateTimeOffset utcNow, TimeZoneInfo tzInfo)
    {
        return utcNow.ToTz(tzInfo).ToEndOfNextDay().ToUtc();
    }
}
