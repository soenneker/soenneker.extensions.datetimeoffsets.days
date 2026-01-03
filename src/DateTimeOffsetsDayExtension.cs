using Soenneker.Enums.UnitOfTime;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Enums.DayOfWeek;

namespace Soenneker.Extensions.DateTimeOffsets.Days;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> focused on day boundaries,
/// including helpers that compute day starts/ends in a specified time zone while returning UTC instants.
/// </summary>
public static class DateTimeOffsetsDayExtension
{
    /// <summary>
    /// Returns the start of the day containing <paramref name="dateTimeOffset"/> (00:00:00).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>The first moment of the day containing <paramref name="dateTimeOffset"/>. The original offset is preserved.</returns>
    [Pure]
    public static DateTimeOffset ToStartOfDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOf(UnitOfTime.Day);

    /// <summary>
    /// Returns the end of the day containing <paramref name="dateTimeOffset"/> (one tick before the next day).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>The last tick of the day containing <paramref name="dateTimeOffset"/>. The original offset is preserved.</returns>
    [Pure]
    public static DateTimeOffset ToEndOfDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToEndOf(UnitOfTime.Day);

    /// <summary>
    /// Returns the start of the next day relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfNextDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfDay()
                      .AddDays(1);

    /// <summary>
    /// Returns the start of the previous day relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfDay()
                      .AddDays(-1);

    /// <summary>
    /// Returns the end of the previous day relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <remarks>Computed as <c>ToStartOfDay() - 1 tick</c>.</remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfDay()
                      .AddTicks(-1);

    /// <summary>
    /// Returns the end of the next day relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <remarks>Computed as <c>ToStartOfNextDay().AddDays(1) - 1 tick</c>.</remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextDay(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfNextDay()
                      .AddDays(1)
                      .AddTicks(-1);

    /// <summary>
    /// Computes the start of the day in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcInstant">An instant in time (any offset is normalized to UTC).</param>
    /// <param name="tz">The time zone whose local calendar rules determine day boundaries.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/> representing the start of the local day in <paramref name="tz"/>.</returns>
    [Pure]
    public static DateTimeOffset ToStartOfTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        ToStartOfTzDayCore(utcInstant, tz, dayOffset: 0);

    /// <summary>
    /// Computes the start of the previous day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        ToStartOfTzDayCore(utcInstant, tz, dayOffset: -1);

    /// <summary>
    /// Computes the start of the next day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfNextTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        ToStartOfTzDayCore(utcInstant, tz, dayOffset: 1);

    /// <summary>
    /// Computes the end of the day in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <remarks>Computed as <c>StartOfNextTzDay - 1 tick</c>.</remarks>
    [Pure]
    public static DateTimeOffset ToEndOfTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfNextTzDay(tz)
                  .AddTicks(-1);

    /// <summary>
    /// Computes the end of the previous day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <remarks>Computed as <c>StartOfTzDay - 1 tick</c>.</remarks>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfTzDay(tz)
                  .AddTicks(-1);

    /// <summary>
    /// Computes the end of the next day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <remarks>Computed as <c>StartOfNextNextTzDay - 1 tick</c>.</remarks>
    [Pure]
    public static DateTimeOffset ToEndOfNextTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) =>
        utcInstant.ToStartOfNextTzDay(tz)
                  .AddDays(1)
                  .AddTicks(-1);

    [Pure]
    private static DateTimeOffset ToStartOfTzDayCore(DateTimeOffset utcInstant, TimeZoneInfo tz, int dayOffset)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        // Normalize to a UTC instant
        DateTimeOffset utc = utcInstant.ToUniversalTime();

        // Convert to the zone to figure out the local *date* we’re anchoring on
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utc, tz);

        // Local midnight of that date (+ optional day offset), as a wall-clock time
        DateTime localMidnight = new(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Unspecified);
        if (dayOffset != 0)
            localMidnight = localMidnight.AddDays(dayOffset);

        // Map local wall-clock midnight -> UTC, handling DST gaps/folds robustly
        DateTime utcStart = ConvertLocalToUtcRobust(localMidnight, tz);

        return new DateTimeOffset(utcStart, TimeSpan.Zero);
    }

    [Pure]
    private static DateTime ConvertLocalToUtcRobust(DateTime localUnspecified, TimeZoneInfo tz)
    {
        // Handle spring-forward gap (invalid local time): advance minute-by-minute to first valid instant
        if (tz.IsInvalidTime(localUnspecified))
        {
            DateTime probe = localUnspecified;
            do
            {
                probe = probe.AddMinutes(1);
            }
            while (tz.IsInvalidTime(probe));

            return TimeZoneInfo.ConvertTimeToUtc(probe, tz);
        }

        // Handle fall-back fold (ambiguous local time): choose the earlier UTC instant
        if (tz.IsAmbiguousTime(localUnspecified))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localUnspecified);
            // earlier UTC = local - larger offset
            TimeSpan chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1];
            return DateTime.SpecifyKind(localUnspecified - chosen, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeToUtc(localUnspecified, tz);
    }

    /// <summary>
    /// Converts the specified <paramref name="dateTimeOffset"/> to a <see cref="DayOfWeekType"/>,
    /// which represents the day of the week.
    /// </summary>
    /// <param name="dateTimeOffset">
    /// The datetime offset from which to extract the day of the week.
    /// </param>
    /// <returns>
    /// A <see cref="DayOfWeekType"/> representing the day of the week for the specified datetime.
    /// </returns>
    /// <remarks>
    /// Uses the local calendar day represented by the <see cref="DateTimeOffset"/> value.
    /// No timezone or offset conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DayOfWeekType ToDayOfWeekType(this DateTimeOffset dateTimeOffset)
    {
        return DayOfWeekType.FromValue(dateTimeOffset.DayOfWeek.ToString());
    }
}