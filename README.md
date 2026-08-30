[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.days.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.days/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.days/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.days/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.days.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.days/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.days/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.days/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Days

Computes current, previous, and next day boundaries for `DateTimeOffset`, either in its stored offset or in a named time zone with UTC results.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Days
```

## Boundaries in the stored offset

```csharp
using Soenneker.Extensions.DateTimeOffsets.Days;

DateTimeOffset value = new(2026, 8, 29, 16, 42, 30, TimeSpan.FromHours(-4));

DateTimeOffset start = value.ToStartOfDay();
DateTimeOffset end = value.ToEndOfDay();
DateTimeOffset previousStart = value.ToStartOfPreviousDay();
DateTimeOffset nextEnd = value.ToEndOfNextDay();
```

| Method | Result |
| --- | --- |
| `ToStartOfDay()` | Midnight on the stored calendar date |
| `ToEndOfDay()` | One tick before the next stored date |
| `ToStartOfPreviousDay()` | Midnight on the previous stored date |
| `ToEndOfPreviousDay()` | One tick before the current stored date |
| `ToStartOfNextDay()` | Midnight on the next stored date |
| `ToEndOfNextDay()` | One tick before the date after next |

These methods preserve the value's offset and use calendar arithmetic on its existing fields. They do not consult a `TimeZoneInfo`, so that fixed offset does not automatically change across DST.

## Boundaries in a named time zone

```csharp
TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
DateTimeOffset instant = new(2026, 8, 29, 18, 0, 0, TimeSpan.Zero);

DateTimeOffset localDayStartUtc = instant.ToStartOfTzDay(eastern);
DateTimeOffset localDayEndUtc = instant.ToEndOfTzDay(eastern);
```

Time-zone variants determine the instant's local calendar date and return the current, previous, or next local-day boundary with offset `+00:00`:

- `ToStartOfTzDay()` / `ToEndOfTzDay()`
- `ToStartOfPreviousTzDay()` / `ToEndOfPreviousTzDay()`
- `ToStartOfNextTzDay()` / `ToEndOfNextTzDay()`

Any input offset is accepted because `DateTimeOffset` identifies an instant. A midnight inside a time-zone gap advances minute-by-minute to the first valid local minute. An ambiguous midnight selects the earlier UTC instant. End values are one tick before the following valid local-day boundary, so 23-hour and 25-hour days are not treated as fixed 24-hour intervals.

## Day-of-week mapping

```csharp
DayOfWeekType day = value.ToDayOfWeekType();
```

`ToDayOfWeekType()` maps the `System.DayOfWeek` of the value's stored calendar date to `Soenneker.Enums.DayOfWeek.DayOfWeekType`. It does not perform a time-zone conversion first.
