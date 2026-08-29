[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.days.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.days/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.days/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.days/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.days.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.days/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.days/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.days/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Days
A collection of helpful DateTimeOffset day extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Days
```

## Quick start

```csharp
using Soenneker.Extensions.DateTimeOffsets.Days;

DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
var result = dateTimeOffset.ToStartOfDay();
```

## Common operations

- `ToStartOfDay()` - Returns the start of the day that contains `dateTimeOffset` (00:00:00.0000000). This method does not convert time zones and does not normalize to UTC; it operates on the calendar date implied by `dateTimeOffset` and preserves `DateTimeOffset.Offset`.
- `ToEndOfDay()` - Returns the end of the day that contains `dateTimeOffset` (one tick before the next day).
- `ToStartOfNextDay()` - Returns the start of the next day relative to `dateTimeOffset` (00:00:00.0000000 of the following day).
- `ToStartOfPreviousDay()` - Returns the start of the previous day relative to `dateTimeOffset` (00:00:00.0000000 of the prior day).
- `ToEndOfPreviousDay()` - Returns the end of the previous day relative to `dateTimeOffset` (one tick before the current day begins).
- `ToEndOfNextDay()` - Returns the end of the next day relative to `dateTimeOffset` (one tick before the day after next begins).
- `ToStartOfTzDay()` - Computes the start of the local day in `tz` that contains the instant `utcInstant`, returning the result as a UTC instant. This method determines the local calendar date in `tz` for the given instant, constructs local midnight (wall time), and converts that wall time to UTC using the time zone's adjustment rules.
- `ToStartOfPreviousTzDay()` - Computes the start of the previous local day in `tz` relative to the instant `utcInstant`, returning the result as a UTC instant. Equivalent to `ToStartOfTzDayCore(utcInstant, tz, -1)`.
- `ToStartOfNextTzDay()` - Computes the start of the next local day in `tz` relative to the instant `utcInstant`, returning the result as a UTC instant. Equivalent to `ToStartOfTzDayCore(utcInstant, tz, 1)`.
- `ToEndOfTzDay()` - Computes the end of the local day in `tz` that contains the instant `utcInstant`, returning the result as a UTC instant.
- `ToEndOfPreviousTzDay()` - Computes the end of the previous local day in `tz` relative to the instant `utcInstant`, returning the result as a UTC instant.
- `ToEndOfNextTzDay()` - Computes the end of the next local day in `tz` relative to the instant `utcInstant`, returning the result as a UTC instant.

The package also includes one additional operation for more specialized cases.
