﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Hemerology;

using Calendrie.Hemerology.Scopes;

/// <summary>Represents a basic calendar with dates on or after a given date.</summary>
/// <remarks>The aforementioned date can NOT be the start of a year.</remarks>
public class BoundedBelowBasicCalendar : BasicCalendar<BoundedBelowScope>
{
    /// <summary>Initializes a new instance of the <see cref="BoundedBelowBasicCalendar"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException">One of the parameters is null.</exception>
    public BoundedBelowBasicCalendar(string name, BoundedBelowScope scope) : base(name, scope)
    {
        Debug.Assert(scope != null);

        MinYear = scope.MinYear;
        MinDateParts = scope.MinDateParts;
        MinOrdinalParts = scope.MinOrdinalParts;
        MinMonthParts = scope.MinMonthParts;
    }

    /// <summary>Gets the earliest supported year.</summary>
    public int MinYear { get; }

    /// <summary>Gets the earliest supported month parts.</summary>
    public MonthParts MinMonthParts { get; }

    /// <summary>Gets the earliest supported date parts.</summary>
    public DateParts MinDateParts { get; }

    /// <summary>Gets the earliest supported ordinal date parts.</summary>
    public OrdinalParts MinOrdinalParts { get; }

    // NB : pour optimiser les choses on pourrait traiter d'abord le cas
    // limite (première année ou premier mois) puis le cas général.
    // Attention, il ne faudrait alors pas écrire
    // > if (new Yemo(year, month) == MinYemoda.Yemo) { ... }
    // mais plutôt
    // > if (year == MinYear && month == MinYemoda.Month) { ... }
    // car on n'a justement pas validé les paramètres.

    /// <inheritdoc />
    [Pure]
    public sealed override int CountMonthsInYear(int year)
    {
        YearsValidator.Validate(year);
        return year == MinYear
            ? CountMonthsInFirstYear()
            : Schema.CountMonthsInYear(year);
    }

    /// <inheritdoc />
    [Pure]
    public sealed override int CountDaysInYear(int year)
    {
        YearsValidator.Validate(year);
        return year == MinYear
            ? CountDaysInFirstYear()
            : Schema.CountDaysInYear(year);
    }

    /// <inheritdoc />
    [Pure]
    public sealed override int CountDaysInMonth(int year, int month)
    {
        Scope.ValidateYearMonth(year, month);
        return new MonthParts(year, month) == MinMonthParts
            ? CountDaysInFirstMonth()
            : Schema.CountDaysInMonth(year, month);
    }

    /// <summary>Obtains the number of months in the first supported year.</summary>
    [Pure]
    public int CountMonthsInFirstYear() =>
        Schema.CountMonthsInYear(MinYear) - MinDateParts.Month + 1;

    /// <summary>Obtains the number of days in the first supported year.</summary>
    [Pure]
    public int CountDaysInFirstYear() =>
        Schema.CountDaysInYear(MinYear) - MinOrdinalParts.DayOfYear + 1;

    /// <summary>Obtains the number of days in the first supported month.</summary>
    [Pure]
    public int CountDaysInFirstMonth()
    {
        var (y, m, d) = MinDateParts;
        return Schema.CountDaysInMonth(y, m) - d + 1;
    }
}
