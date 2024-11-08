﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool. Changes to this file may cause incorrect
// behaviour and will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable

namespace Calendrie.Specialized;

using Calendrie.Core.Schemas;
using Calendrie.Hemerology;
using Calendrie.Hemerology.Scopes;

/// <summary>Represents the Julian calendar.
/// <para>This class cannot be inherited.</para></summary>
public sealed partial class JulianCalendar : SpecialCalendar<JulianDate>
{
    /// <summary>Initializes a new instance of the <see cref="JulianCalendar"/> class.</summary>
    public JulianCalendar() : this(new JulianSchema()) { }

    internal JulianCalendar(JulianSchema schema) : base("Julian", GetScope(schema))
    {
        OnInitializing(schema);
    }

    private static partial MinMaxYearScope GetScope(JulianSchema schema);

    partial void OnInitializing(JulianSchema schema);

    private protected sealed override JulianDate GetDate(int daysSinceEpoch) => new(daysSinceEpoch);
}

/// <summary>Provides common adjusters for <see cref="JulianDate"/>.
/// <para>This class cannot be inherited.</para></summary>
public sealed partial class JulianAdjuster : SpecialAdjuster<JulianDate>
{
    /// <summary>Initializes a new instance of the <see cref="JulianAdjuster"/> class.</summary>
    public JulianAdjuster() : base(JulianDate.Calendar.Scope) { }

    internal JulianAdjuster(MinMaxYearScope scope) : base(scope) { }

    private protected sealed override JulianDate GetDate(int daysSinceEpoch) => new(daysSinceEpoch);
}

/// <summary>Represents the Julian date.
/// <para><see cref="JulianDate"/> is an immutable struct.</para></summary>
public readonly partial struct JulianDate :
    IDate<JulianDate, JulianCalendar>,
    IAdjustable<JulianDate>
{ }

public partial struct JulianDate // Counting
{
    /// <inheritdoc />
    [Pure]
    public int CountElapsedDaysInYear() => s_Schema.CountDaysInYearBefore(_daysSinceEpoch);

    /// <inheritdoc />
    [Pure]
    public int CountRemainingDaysInYear() => s_Schema.CountDaysInYearAfter(_daysSinceEpoch);

    /// <inheritdoc />
    [Pure]
    public int CountElapsedDaysInMonth() => s_Schema.CountDaysInMonthBefore(_daysSinceEpoch);

    /// <inheritdoc />
    [Pure]
    public int CountRemainingDaysInMonth() => s_Schema.CountDaysInMonthAfter(_daysSinceEpoch);
}

public partial struct JulianDate // Adjustments
{
    /// <inheritdoc />
    /// <remarks>See also <seealso cref="Adjuster"/>.</remarks>
    [Pure]
    public JulianDate Adjust(Func<JulianDate, JulianDate> adjuster)
    {
        Requires.NotNull(adjuster);

        return adjuster.Invoke(this);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate Previous(DayOfWeek dayOfWeek)
    {
        var dayNumber = DayNumber.Previous(dayOfWeek);
        if (s_Domain.Contains(dayNumber) == false) Throw.DateOverflow();
        return new JulianDate(dayNumber - s_Epoch);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate PreviousOrSame(DayOfWeek dayOfWeek)
    {
        var dayNumber = DayNumber.PreviousOrSame(dayOfWeek);
        if (s_Domain.Contains(dayNumber) == false) Throw.DateOverflow();
        return new JulianDate(dayNumber - s_Epoch);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate Nearest(DayOfWeek dayOfWeek)
    {
        var dayNumber = DayNumber.Nearest(dayOfWeek);
        if (s_Domain.Contains(dayNumber) == false) Throw.DateOverflow();
        return new JulianDate(dayNumber - s_Epoch);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate NextOrSame(DayOfWeek dayOfWeek)
    {
        var dayNumber = DayNumber.NextOrSame(dayOfWeek);
        if (s_Domain.Contains(dayNumber) == false) Throw.DateOverflow();
        return new JulianDate(dayNumber - s_Epoch);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate Next(DayOfWeek dayOfWeek)
    {
        var dayNumber = DayNumber.Next(dayOfWeek);
        if (s_Domain.Contains(dayNumber) == false) Throw.DateOverflow();
        return new JulianDate(dayNumber - s_Epoch);
    }
}

public partial struct JulianDate // IEquatable
{
    /// <inheritdoc />
    public static bool operator ==(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch == right._daysSinceEpoch;

    /// <inheritdoc />
    public static bool operator !=(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch != right._daysSinceEpoch;

    /// <inheritdoc />
    [Pure]
    public bool Equals(JulianDate other) => _daysSinceEpoch == other._daysSinceEpoch;

    /// <inheritdoc />
    [Pure]
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is JulianDate date && Equals(date);

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => _daysSinceEpoch;
}

public partial struct JulianDate // IComparable
{
    /// <inheritdoc />
    public static bool operator <(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch < right._daysSinceEpoch;

    /// <inheritdoc />
    public static bool operator <=(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch <= right._daysSinceEpoch;

    /// <inheritdoc />
    public static bool operator >(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch > right._daysSinceEpoch;

    /// <inheritdoc />
    public static bool operator >=(JulianDate left, JulianDate right) =>
        left._daysSinceEpoch >= right._daysSinceEpoch;

    /// <inheritdoc />
    [Pure]
    public static JulianDate Min(JulianDate x, JulianDate y) => x < y ? x : y;

    /// <inheritdoc />
    [Pure]
    public static JulianDate Max(JulianDate x, JulianDate y) => x > y ? x : y;

    /// <inheritdoc />
    [Pure]
    public int CompareTo(JulianDate other) => _daysSinceEpoch.CompareTo(other._daysSinceEpoch);

    [Pure]
    int IComparable.CompareTo(object? obj) =>
        obj is null ? 1
        : obj is JulianDate date ? CompareTo(date)
        : Throw.NonComparable(typeof(JulianDate), obj);
}

public partial struct JulianDate // Math
{
#pragma warning disable CA2225 // Operator overloads have named alternates (Usage) ✓
    // Friendly alternates do exist but use domain-specific names.

    /// <summary>Subtracts the two specified dates and returns the number of days between them.</summary>
    public static int operator -(JulianDate left, JulianDate right) => left.CountDaysSince(right);

    /// <summary>Adds a number of days to the specified date, yielding a new date.</summary>
    /// <exception cref="OverflowException">The operation would overflow either the capacity of
    /// <see cref="Int32"/> or the range of supported dates.</exception>
    public static JulianDate operator +(JulianDate value, int days) => value.PlusDays(days);

    /// <summary>Subtracts a number of days to the specified date, yielding a new date.</summary>
    /// <exception cref="OverflowException">The operation would overflow either the capacity of
    /// <see cref="Int32"/> or the range of supported dates.</exception>
    public static JulianDate operator -(JulianDate value, int days) => value.PlusDays(-days);

    /// <summary>Adds one day to the specified date, yielding a new date.</summary>
    /// <exception cref="OverflowException">The operation would overflow the latest supported date.</exception>
    public static JulianDate operator ++(JulianDate value) => value.NextDay();

    /// <summary>Subtracts one day to the specified date, yielding a new date.</summary>
    /// <exception cref="OverflowException">The operation would overflow the earliest supported date.</exception>
    public static JulianDate operator --(JulianDate value) => value.PreviousDay();

#pragma warning restore CA2225

    /// <inheritdoc />
    [Pure]
    public int CountDaysSince(JulianDate other) =>
        // No need to use a checked context here.
        _daysSinceEpoch - other._daysSinceEpoch;

    /// <inheritdoc />
    [Pure]
    public JulianDate PlusDays(int days)
    {
        int daysSinceEpoch = checked(_daysSinceEpoch + days);
        // Don't write (the addition may also overflow...):
        // > s_Domain.CheckOverflow(s_Epoch + daysSinceEpoch);
        s_Scope.DaysValidator.CheckOverflow(daysSinceEpoch);
        return new(daysSinceEpoch);
    }

    /// <inheritdoc />
    [Pure]
    public JulianDate NextDay() =>
        this == s_MaxValue ? Throw.DateOverflow<JulianDate>()
        : new JulianDate(_daysSinceEpoch + 1);

    /// <inheritdoc />
    [Pure]
    public JulianDate PreviousDay() =>
        this == s_MinValue ? Throw.DateOverflow<JulianDate>()
        : new JulianDate(_daysSinceEpoch - 1);
}
