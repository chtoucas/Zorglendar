﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Specialized;

using System.Linq;

using Calendrie.Hemerology;
using Calendrie.Hemerology.Scopes;

// See comments in SpecialAdjuster<>.

/// <summary>Represents a calendar with dates within a range of years and provides a base for
/// derived classes.
/// <para>This class works best when <typeparamref name="TDate"/> is based on the count of
/// consecutive days since the epoch.</para>
/// <para>This class can ONLY be inherited from within friend assemblies.</para></summary>
/// <typeparam name="TDate">The type of date object.</typeparam>
public abstract class SpecialCalendar<TDate> : MinMaxYearBasicCalendar, ICalendar<TDate>
{
    /// <summary>Called from constructors in derived classes to initialize the
    /// <see cref="SpecialCalendar{TDate}"/> class.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="scope"/> is null.</exception>
    private protected SpecialCalendar(string name, MinMaxYearScope scope) : base(name, scope) { }

    /// <summary>Creates a new instance of <typeparamref name="TDate"/> from the specified count of
    /// consecutive days since the epoch.
    /// <para>This method does NOT validate its parameter.</para></summary>
    [Pure] private protected abstract TDate GetDate(int daysSinceEpoch);

    /// <inheritdoc/>
    [Pure]
    public IEnumerable<TDate> GetDaysInYear(int year)
    {
        YearsValidator.Validate(year);

        int startOfYear = Schema.GetStartOfYear(year);
        int daysInYear = Schema.CountDaysInYear(year);

        return from daysSinceEpoch
               in Enumerable.Range(startOfYear, daysInYear)
               select GetDate(daysSinceEpoch);
    }

    /// <inheritdoc/>
    [Pure]
    public IEnumerable<TDate> GetDaysInMonth(int year, int month)
    {
        Scope.ValidateYearMonth(year, month);

        int startOfMonth = Schema.GetStartOfMonth(year, month);
        int daysInMonth = Schema.CountDaysInMonth(year, month);

        return from daysSinceEpoch
               in Enumerable.Range(startOfMonth, daysInMonth)
               select GetDate(daysSinceEpoch);
    }

    /// <inheritdoc/>
    [Pure]
    public TDate GetStartOfYear(int year)
    {
        YearsValidator.Validate(year);
        int daysSinceEpoch = Schema.GetStartOfYear(year);
        return GetDate(daysSinceEpoch);
    }

    /// <inheritdoc/>
    [Pure]
    public TDate GetEndOfYear(int year)
    {
        YearsValidator.Validate(year);
        int daysSinceEpoch = Schema.GetEndOfYear(year);
        return GetDate(daysSinceEpoch);
    }

    /// <inheritdoc/>
    [Pure]
    public TDate GetStartOfMonth(int year, int month)
    {
        Scope.ValidateYearMonth(year, month);
        int daysSinceEpoch = Schema.GetStartOfMonth(year, month);
        return GetDate(daysSinceEpoch);
    }

    /// <inheritdoc/>
    [Pure]
    public TDate GetEndOfMonth(int year, int month)
    {
        Scope.ValidateYearMonth(year, month);
        int daysSinceEpoch = Schema.GetEndOfMonth(year, month);
        return GetDate(daysSinceEpoch);
    }
}
