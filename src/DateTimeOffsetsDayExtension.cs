using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Enums.DayOfWeek;

namespace Soenneker.Extensions.DateTimeOffsets.Days;

/// <summary>
/// Provides high-performance <see cref="DateTimeOffset"/> extension methods for working with day boundaries.
/// </summary>
/// <remarks>
/// <para>
/// The non-time-zone methods (<see cref="ToStartOfDay(DateTimeOffset)"/>, <see cref="ToEndOfDay(DateTimeOffset)"/>, etc.)
/// operate purely on the date components of the provided <see cref="DateTimeOffset"/> and preserve its original offset.
/// No time zone conversions are performed.
/// </para>
/// <para>
/// The time-zone methods (<see cref="ToStartOfTzDay(DateTimeOffset, TimeZoneInfo)"/>, etc.) treat the input as an instant in time,
/// determine the corresponding local calendar day in the specified <see cref="TimeZoneInfo"/>, and return the computed boundary as a UTC instant
/// (offset <c>+00:00</c>). These methods are robust across DST transitions (gaps and folds).
/// </para>
/// </remarks>
public static class DateTimeOffsetsDayExtension
{
    private const long _oneTick = 1;

    /// <summary>
    /// Returns the start of the day that contains <paramref name="dateTimeOffset"/> (00:00:00.0000000).
    /// </summary>
    /// <param name="dateTimeOffset">The value whose containing day boundary will be computed.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing local midnight of the containing day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// This method does not convert time zones and does not normalize to UTC; it operates on the calendar date implied by
    /// <paramref name="dateTimeOffset"/> and preserves <see cref="DateTimeOffset.Offset"/>.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfDay(this DateTimeOffset dateTimeOffset) =>
        new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);

    /// <summary>
    /// Returns the end of the day that contains <paramref name="dateTimeOffset"/> (one tick before the next day).
    /// </summary>
    /// <param name="dateTimeOffset">The value whose containing day boundary will be computed.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last tick of the containing day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// Computed as <c>ToStartOfDay().AddDays(1).AddTicks(-1)</c>. No time zone conversion is performed and the original offset is preserved.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfDay(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToStartOfDay()
                                                                                                 .AddDays(1)
                                                                                                 .AddTicks(-_oneTick);

    /// <summary>
    /// Returns the start of the next day relative to <paramref name="dateTimeOffset"/> (00:00:00.0000000 of the following day).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the start of the next day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// Computed as <c>ToStartOfDay().AddDays(1)</c>. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfNextDay(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToStartOfDay()
                                                                                                       .AddDays(1);

    /// <summary>
    /// Returns the start of the previous day relative to <paramref name="dateTimeOffset"/> (00:00:00.0000000 of the prior day).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the start of the previous day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// Computed as <c>ToStartOfDay().AddDays(-1)</c>. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfPreviousDay(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToStartOfDay()
        .AddDays(-1);

    /// <summary>
    /// Returns the end of the previous day relative to <paramref name="dateTimeOffset"/> (one tick before the current day begins).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last tick of the previous day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// Computed as <c>ToStartOfDay().AddTicks(-1)</c>. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfPreviousDay(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToStartOfDay()
                                                                                                         .AddTicks(-_oneTick);

    /// <summary>
    /// Returns the end of the next day relative to <paramref name="dateTimeOffset"/> (one tick before the day after next begins).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last tick of the next day, preserving the original offset.
    /// </returns>
    /// <remarks>
    /// Computed as <c>ToStartOfDay().AddDays(2).AddTicks(-1)</c>. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfNextDay(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToStartOfDay()
                                                                                                     .AddDays(2)
                                                                                                     .AddTicks(-_oneTick);

    /// <summary>
    /// Computes the start of the local day in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant in time. Any offset is accepted; the value is treated as an instant and normalized to UTC before applying
    /// the time zone calendar rules.
    /// </param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the UTC instant corresponding to local midnight
    /// at the start of the containing day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// This method determines the local calendar date in <paramref name="tz"/> for the given instant, constructs local midnight (wall time),
    /// and converts that wall time to UTC using the time zone's adjustment rules. DST gaps and folds are handled by
    /// <see cref="ConvertLocalToUtcRobust(DateTime, TimeZoneInfo)"/>.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, 0);

    /// <summary>
    /// Computes the start of the previous local day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">An instant in time, normalized to UTC before conversion.</param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the UTC instant corresponding to local midnight
    /// at the start of the previous day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// Equivalent to <c>ToStartOfTzDayCore(utcInstant, tz, -1)</c>. DST-safe.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfPreviousTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, -1);

    /// <summary>
    /// Computes the start of the next local day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">An instant in time, normalized to UTC before conversion.</param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the UTC instant corresponding to local midnight
    /// at the start of the next day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// Equivalent to <c>ToStartOfTzDayCore(utcInstant, tz, 1)</c>. DST-safe.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfNextTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, 1);

    /// <summary>
    /// Computes the end of the local day in <paramref name="tz"/> that contains the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">An instant in time, normalized to UTC before conversion.</param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the last tick of the containing local day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// Computed as <c>StartOfNextTzDay - 1 tick</c> (DST-safe).
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, 1)
        .AddTicks(-_oneTick);

    /// <summary>
    /// Computes the end of the previous local day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">An instant in time, normalized to UTC before conversion.</param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the last tick of the previous local day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// Computed as <c>StartOfTzDay - 1 tick</c> (DST-safe).
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfPreviousTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, 0)
        .AddTicks(-_oneTick);

    /// <summary>
    /// Computes the end of the next local day in <paramref name="tz"/> relative to the instant <paramref name="utcInstant"/>,
    /// returning the result as a UTC instant.
    /// </summary>
    /// <param name="utcInstant">An instant in time, normalized to UTC before conversion.</param>
    /// <param name="tz">The time zone whose local calendar day boundaries should be used.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> with offset <c>+00:00</c> representing the last tick of the next local day in <paramref name="tz"/>.
    /// </returns>
    /// <remarks>
    /// Computed as <c>StartOfTzDayCore(dayOffset: 2) - 1 tick</c>, which is equivalent to
    /// one tick before the start of the day after next in <paramref name="tz"/> (DST-safe).
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfNextTzDay(this DateTimeOffset utcInstant, TimeZoneInfo tz) => ToStartOfTzDayCore(utcInstant, tz, 2)
        .AddTicks(-_oneTick);

    /// <summary>
    /// Core implementation used by the time-zone day boundary methods.
    /// </summary>
    /// <param name="utcInstant">An instant in time (any offset), treated as an instant and normalized to UTC.</param>
    /// <param name="tz">The time zone whose local day boundaries are used.</param>
    /// <param name="dayOffset">
    /// A signed day offset applied in the local calendar of <paramref name="tz"/>. For example:
    /// <c>0</c> = containing day, <c>-1</c> = previous local day, <c>1</c> = next local day.
    /// </param>
    /// <returns>
    /// A UTC <see cref="DateTimeOffset"/> (offset <c>+00:00</c>) representing the start of the selected local day in <paramref name="tz"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    private static DateTimeOffset ToStartOfTzDayCore(DateTimeOffset utcInstant, TimeZoneInfo tz, int dayOffset)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        // Normalize to UTC DateTime once (no extra DateTimeOffset conversions)
        DateTime utc = utcInstant.UtcDateTime;

        if (utc.Kind != DateTimeKind.Utc)
            utc = utcInstant.ToUniversalTime()
                            .UtcDateTime;

        // Convert instant to local to get the correct local calendar date
        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);

        // Construct local midnight (wall time)
        DateTime localMidnight = new(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Unspecified);
        if (dayOffset != 0)
            localMidnight = localMidnight.AddDays(dayOffset);

        DateTime utcStart = ConvertLocalToUtcRobust(localMidnight, tz);
        return new DateTimeOffset(utcStart, TimeSpan.Zero);
    }

    /// <summary>
    /// Converts a local (unspecified-kind) wall time in <paramref name="tz"/> into a UTC <see cref="DateTime"/>,
    /// handling DST gaps and folds deterministically.
    /// </summary>
    /// <param name="localUnspecified">
    /// A local wall-clock time with <see cref="DateTime.Kind"/> set to <see cref="DateTimeKind.Unspecified"/>.
    /// This value is interpreted in the context of <paramref name="tz"/>.
    /// </param>
    /// <param name="tz">The time zone whose adjustment rules are applied.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTime.Kind"/> equal to <see cref="DateTimeKind.Utc"/>
    /// representing the chosen UTC instant.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For invalid local times (spring-forward gaps), this method selects the earliest valid local instant at or after
    /// <paramref name="localUnspecified"/> by probing forward and optionally tightening toward the gap edge.
    /// </para>
    /// <para>
    /// For ambiguous local times (fall-back folds), this method selects the earlier UTC instant. This corresponds to choosing
    /// the larger of the two possible offsets and computing <c>UTC = local - offset</c>.
    /// </para>
    /// </remarks>
    [Pure]
    private static DateTime ConvertLocalToUtcRobust(DateTime localUnspecified, TimeZoneInfo tz)
    {
        // Handle invalid time (gap): jump first, then tighten.
        if (tz.IsInvalidTime(localUnspecified))
        {
            // Most gaps are ~1 hour. Jump an hour; if still invalid, grow.
            DateTime probe = localUnspecified.AddHours(1);

            // If this zone has larger/odd gaps, expand quickly (rare).
            int hops = 0;
            while (tz.IsInvalidTime(probe))
            {
                probe = probe.AddHours(1);
                if (++hops >= 6) // extreme safety; should basically never hit
                    probe = probe.AddMinutes(1);
            }

            // Optionally tighten to earliest valid minute in the hour window.
            // Binary-ish search on minutes (max 6 probes) after we found a valid probe.
            DateTime lo = localUnspecified;
            DateTime hi = probe;
            for (int i = 0; i < 6; i++)
            {
                DateTime mid = lo.AddMinutes((hi - lo).TotalMinutes / 2);
                // mid is a double-based op; still tiny and very rare path.
                if (tz.IsInvalidTime(mid))
                    lo = mid;
                else
                    hi = mid;
            }

            // hi should be valid or very near-valid; ConvertTimeToUtc will finalize.
            return TimeZoneInfo.ConvertTimeToUtc(hi, tz);
        }

        // Handle ambiguous time (fold): choose earlier UTC instant (local - larger offset)
        if (tz.IsAmbiguousTime(localUnspecified))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localUnspecified);
            TimeSpan chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1];
            return DateTime.SpecifyKind(localUnspecified - chosen, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeToUtc(localUnspecified, tz);
    }

    /// <summary>
    /// Converts <paramref name="dateTimeOffset"/>'s <see cref="System.DayOfWeek"/> into a <see cref="DayOfWeekType"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The value from which to obtain the day of week.</param>
    /// <returns>
    /// A <see cref="DayOfWeekType"/> representing the day of week of <paramref name="dateTimeOffset"/>.
    /// </returns>
    /// <remarks>
    /// Uses the day-of-week associated with the calendar date represented by <paramref name="dateTimeOffset"/> and its offset.
    /// No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DayOfWeekType ToDayOfWeekType(this DateTimeOffset dateTimeOffset) => dateTimeOffset.DayOfWeek switch
    {
        DayOfWeek.Sunday => DayOfWeekType.Sunday,
        DayOfWeek.Monday => DayOfWeekType.Monday,
        DayOfWeek.Tuesday => DayOfWeekType.Tuesday,
        DayOfWeek.Wednesday => DayOfWeekType.Wednesday,
        DayOfWeek.Thursday => DayOfWeekType.Thursday,
        DayOfWeek.Friday => DayOfWeekType.Friday,
        _ => DayOfWeekType.Saturday,
    };
}