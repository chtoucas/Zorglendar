﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Specialized;

using Calendrie.Core.Intervals;
using Calendrie.Core.Schemas;
using Calendrie.Core.Validation;
using Calendrie.Hemerology;
using Calendrie.Hemerology.Scopes;

// We use GregorianStandardScope instead of s_Calendar.Scope because they
// are strictly equivalent. We could optimize PlusDays() by using
// GregorianStandardScope.DaysValidator instead of s_Scope.DaysValidator.

public partial struct CivilDate
{
    // WARNING: the order in which the static fields are written is __important__.

    private static readonly CivilSchema s_Schema = new();
    private static readonly CivilCalendar s_Calendar = new(s_Schema);
    private static readonly MinMaxYearScope s_Scope = s_Calendar.Scope;
    private static readonly Range<DayNumber> s_Domain = s_Calendar.Domain;
    private static readonly CivilAdjuster s_Adjuster = new(s_Scope);
    private static readonly CivilDate s_MinValue = new(s_Domain.Min.DaysSinceZero);
    private static readonly CivilDate s_MaxValue = new(s_Domain.Max.DaysSinceZero);

    private readonly int _daysSinceZero;

    /// <summary>Initializes a new instance of the <see cref="CivilDate"/> struct to the specified
    /// date parts.</summary>
    /// <exception cref="AoorException">The specified components do not form a valid date or
    /// <paramref name="year"/> is outside the range of supported years.</exception>
    public CivilDate(int year, int month, int day)
    {
        GregorianStandardScope.ValidateYearMonthDay(year, month, day);

        _daysSinceZero = CivilFormulae.CountDaysSinceEpoch(year, month, day);
    }

    /// <summary>Initializes a new instance of the <see cref="CivilDate"/> struct to the specified
    /// ordinal date parts.</summary>
    /// <exception cref="AoorException">The specified components do not form a valid ordinal date or
    /// <paramref name="year"/> is outside the range of supported years.</exception>
    public CivilDate(int year, int dayOfYear)
    {
        GregorianStandardScope.ValidateOrdinal(year, dayOfYear);

        _daysSinceZero = s_Schema.CountDaysSinceEpoch(year, dayOfYear);
    }

    /// <summary>Initializes a new instance of the <see cref="CivilDate"/> struct.
    /// <para>This method does NOT validate its parameter.</para></summary>
    internal CivilDate(int daysSinceZero)
    {
        _daysSinceZero = daysSinceZero;
    }

    /// <inheritdoc />
    /// <remarks>This static property is thread-safe.</remarks>
    public static CivilDate MinValue => s_MinValue;

    /// <inheritdoc />
    /// <remarks>This static property is thread-safe.</remarks>
    public static CivilDate MaxValue => s_MaxValue;

    /// <summary>Gets the date adjuster.
    /// <para>This static property is thread-safe.</para></summary>
    public static CivilAdjuster Adjuster => s_Adjuster;

    /// <inheritdoc />
    public static CivilCalendar Calendar => s_Calendar;

    /// <inheritdoc />
    public DayNumber DayNumber => new(_daysSinceZero);

    /// <summary>Gets the count of days since the Gregorian epoch.</summary>
    public int DaysSinceZero => _daysSinceZero;

    int IFixedDate.DaysSinceEpoch => _daysSinceZero;

    /// <inheritdoc />
    public Ord CenturyOfEra => Ord.FromInt32(Century);

    /// <inheritdoc />
    public int Century => YearNumbering.GetCentury(Year);

    /// <inheritdoc />
    public Ord YearOfEra => Ord.FromInt32(Year);

    /// <inheritdoc />
    public int YearOfCentury => YearNumbering.GetYearOfCentury(Year);

    /// <inheritdoc />
    public int Year => CivilFormulae.GetYear(_daysSinceZero);

    /// <inheritdoc />
    public int Month
    {
        get
        {
            CivilFormulae.GetDateParts(_daysSinceZero, out _, out int m, out _);
            return m;
        }
    }

    /// <inheritdoc />
    public int DayOfYear
    {
        get
        {
            _ = CivilFormulae.GetYear(_daysSinceZero, out int doy);
            return doy;
        }
    }

    /// <inheritdoc />
    public int Day
    {
        get
        {
            CivilFormulae.GetDateParts(_daysSinceZero, out _, out _, out int d);
            return d;
        }
    }

    /// <inheritdoc />
    public DayOfWeek DayOfWeek => DayNumber.DayOfWeek;

    /// <inheritdoc />
    public bool IsIntercalary
    {
        get
        {
            CivilFormulae.GetDateParts(_daysSinceZero, out _, out int m, out int d);
            return GregorianFormulae.IsIntercalaryDay(m, d);
        }
    }

    /// <inheritdoc />
    public bool IsSupplementary => false;

    /// <summary>Returns a culture-independent string representation of the current instance.
    /// </summary>
    [Pure]
    public override string ToString()
    {
        CivilFormulae.GetDateParts(_daysSinceZero, out int y, out int m, out int d);
        return FormattableString.Invariant($"{d:D2}/{m:D2}/{y:D4} ({s_Calendar})");
    }

    /// <inheritdoc />
    public void Deconstruct(out int year, out int month, out int day) =>
        CivilFormulae.GetDateParts(_daysSinceZero, out year, out month, out day);

    /// <inheritdoc />
    public void Deconstruct(out int year, out int dayOfYear) =>
        year = CivilFormulae.GetYear(_daysSinceZero, out dayOfYear);
}

public partial struct CivilDate // Factories
{
    /// <summary>Creates a new instance of the <see cref="CivilDate"/> struct from the specified
    /// day number.</summary>
    /// <exception cref="AoorException"><paramref name="dayNumber"/> is outside the range of
    /// supported values.</exception>
    public static CivilDate FromDayNumber(DayNumber dayNumber)
    {
        s_Domain.Validate(dayNumber);

        return new CivilDate(dayNumber.DaysSinceZero);
    }
}
