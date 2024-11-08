﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core.Arithmetic;

using Calendrie.Core.Intervals;
using Calendrie.Core.Schemas;

using __Solar = CalendricalConstants.Solar;
using __Solar12 = CalendricalConstants.Solar12;

/// <summary>
/// Provides the core mathematical operations on dates within the Gregorian calendar.
/// <para>This class cannot be inherited.</para>
/// </summary>
internal sealed partial class GregorianSystemArithmetic : SystemArithmetic
{
    private const int MonthsInYear = __Solar12.MonthsInYear;
    private const int MinDaysInYear = __Solar.MinDaysInYear;
    private const int MinDaysInMonth = __Solar.MinDaysInMonth;
    private const int MaxDaysViaDayOfYear_ = MinDaysInYear;
    private const int MaxDaysViaDayOfMonth_ = MinDaysInMonth;

    /// <summary>
    /// Initializes a new instance of the <see cref="GregorianSystemArithmetic"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="schema"/> is null.</exception>
    /// <exception cref="AoorException"><paramref name="supportedYears"/> is NOT a subinterval
    /// of the range of supported years by <paramref name="schema"/>.</exception>
    public GregorianSystemArithmetic(GregorianSchema schema, Range<int> supportedYears)
        : this(SystemSegment.Create(schema, supportedYears)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GregorianSystemArithmetic"/> class.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="segment"/> is null.</exception>
    internal GregorianSystemArithmetic(SystemSegment segment) : base(segment)
    {
        Debug.Assert(MaxDaysViaDayOfMonth_ >= MinMinDaysInMonth);
        Debug.Assert(Schema is GregorianSchema);

        MaxDaysViaDayOfYear = MaxDaysViaDayOfYear_;
        MaxDaysViaDayOfMonth = MaxDaysViaDayOfMonth_;
    }
}

internal partial class GregorianSystemArithmetic // Operations on Yemoda
{
    /// <inheritdoc />
    [Pure]
    public override Yemoda AddDays(Yemoda ymd, int days)
    {
        // Fast tracks.
        if (-MaxDaysViaDayOfMonth_ <= days && days <= MaxDaysViaDayOfMonth_)
        {
            return AddDaysViaDayOfMonth(ymd, days);
        }

        ymd.Unpack(out int y, out int m, out int d);
        if (-MaxDaysViaDayOfYear_ <= days && days <= MaxDaysViaDayOfYear_)
        {
            int doy = Schema.GetDayOfYear(y, m, d);
            var (newY, newDoy) = AddDaysViaDayOfYear(new Yedoy(y, doy), days);
            return Schema.GetDateParts(newY, newDoy);
        }

        // Slow track.
        int daysSinceEpoch = checked(GregorianFormulae.CountDaysSinceEpoch(y, m, d) + days);
        DaysValidator.CheckOverflow(daysSinceEpoch);

        return GregorianFormulae.GetDateParts(daysSinceEpoch);
    }

    /// <inheritdoc />
    [Pure]
    protected internal override Yemoda AddDaysViaDayOfMonth(Yemoda ymd, int days)
    {
        Debug.Assert(-MaxDaysViaDayOfMonth_ <= days);
        Debug.Assert(days <= MaxDaysViaDayOfMonth_);

        ymd.Unpack(out int y, out int m, out int d);

        int dom = d + days;
        if (dom < 1)
        {
            if (m == 1)
            {
                if (y == MinYear) Throw.DateOverflow();
                y--;
                m = MonthsInYear;
                dom += 31;
            }
            else
            {
                m--;
                dom += GregorianFormulae.CountDaysInMonth(y, m);
            }
        }
        else if (dom > MinDaysInMonth)
        {
            int daysInMonth = GregorianFormulae.CountDaysInMonth(y, m);
            if (dom > daysInMonth)
            {
                dom -= daysInMonth;
                if (m == MonthsInYear)
                {
                    if (y == MaxYear) Throw.DateOverflow();
                    y++;
                    m = 1;
                }
                else
                {
                    m++;
                }
            }
        }

        return new Yemoda(y, m, dom);
    }

    /// <inheritdoc />
    [Pure]
    public override Yemoda NextDay(Yemoda ymd)
    {
        ymd.Unpack(out int y, out int m, out int d);

        return
            d < MinDaysInMonth || d < GregorianFormulae.CountDaysInMonth(y, m)
                ? new Yemoda(y, m, d + 1)
            : m < MonthsInYear ? Yemoda.AtStartOfMonth(y, m + 1)
            : y < MaxYear ? Yemoda.AtStartOfYear(y + 1)
            : Throw.DateOverflow<Yemoda>();
    }

    /// <inheritdoc />
    [Pure]
    public override Yemoda PreviousDay(Yemoda ymd)
    {
        ymd.Unpack(out int y, out int m, out int d);

        return
            d > 1 ? new Yemoda(y, m, d - 1)
            : m > 1 ? Schema.GetDatePartsAtEndOfMonth(y, m - 1)
            : y > MinYear ? Schema.GetDatePartsAtEndOfYear(y - 1)
            : Throw.DateOverflow<Yemoda>();
    }
}

internal partial class GregorianSystemArithmetic // Operations on Yedoy
{
    /// <inheritdoc />
    [Pure]
    public override Yedoy AddDays(Yedoy ydoy, int days)
    {
        // Fast track.
        if (-MaxDaysViaDayOfYear_ <= days && days <= MaxDaysViaDayOfYear_)
        {
            return AddDaysViaDayOfYear(ydoy, days);
        }

        ydoy.Unpack(out int y, out int doy);

        // Slow track.
        int daysSinceEpoch = checked(Schema.CountDaysSinceEpoch(y, doy) + days);
        DaysValidator.CheckOverflow(daysSinceEpoch);

        return GregorianFormulae.GetOrdinalParts(daysSinceEpoch);
    }

    /// <inheritdoc />
    [Pure]
    protected internal override Yedoy AddDaysViaDayOfYear(Yedoy ydoy, int days)
    {
        Debug.Assert(-MaxDaysViaDayOfYear_ <= days);
        Debug.Assert(days <= MaxDaysViaDayOfYear_);

        ydoy.Unpack(out int y, out int doy);

        doy += days;
        if (doy < 1)
        {
            if (y == MinYear) Throw.DateOverflow();
            y--;
            doy += GregorianFormulae.CountDaysInYear(y);
        }
        else
        {
            int daysInYear = GregorianFormulae.CountDaysInYear(y);
            if (doy > daysInYear)
            {
                if (y == MaxYear) Throw.DateOverflow();
                y++;
                doy -= daysInYear;
            }
        }

        return new Yedoy(y, doy);
    }

    /// <inheritdoc />
    [Pure]
    public override Yedoy NextDay(Yedoy ydoy)
    {
        ydoy.Unpack(out int y, out int doy);

        return
            doy < MinDaysInYear || doy < GregorianFormulae.CountDaysInYear(y)
                ? new Yedoy(y, doy + 1)
            : y < MaxYear ? Yedoy.AtStartOfYear(y + 1)
            : Throw.DateOverflow<Yedoy>();
    }

    /// <inheritdoc />
    [Pure]
    public override Yedoy PreviousDay(Yedoy ydoy)
    {
        ydoy.Unpack(out int y, out int doy);

        return doy > 1 ? new Yedoy(y, doy - 1)
            : y > MinYear ? Schema.GetOrdinalPartsAtEndOfYear(y - 1)
            : Throw.DateOverflow<Yedoy>();
    }
}

internal partial class GregorianSystemArithmetic // Operations on Yemo
{
    /// <inheritdoc />
    [Pure]
    public override Yemo AddMonths(Yemo ym, int months)
    {
        ym.Unpack(out int y, out int m);

        m = 1 + MathZ.Modulo(checked(m - 1 + months), MonthsInYear, out int y0);
        y += y0;
        YearsValidator.CheckForMonth(y);

        return new Yemo(y, m);
    }

    // TODO(code): cleanup.
    ///// <inheritdoc />
    //[Pure]
    //public override Yemo NextMonth(Yemo ym)
    //{
    //    ym.Unpack(out int y, out int m);

    //    return
    //        m < MonthsInYear ? new Yemo(y, m + 1)
    //        : y < MaxYear ? Yemo.AtStartOfYear(y + 1)
    //        : Throw.DateOverflow<Yemo>();
    //}

    ///// <inheritdoc />
    //[Pure]
    //public override Yemo PreviousMonth(Yemo ym)
    //{
    //    ym.Unpack(out int y, out int m);

    //    return
    //        m > 1 ? new Yemo(y, m - 1)
    //        : y > MinYear ? Schema.GetMonthPartsAtEndOfYear(y - 1)
    //        : Throw.DateOverflow<Yemo>();
    //}

    /// <inheritdoc />
    [Pure]
    public override int CountMonthsBetween(Yemo start, Yemo end)
    {
        start.Unpack(out int y0, out int m0);
        end.Unpack(out int y1, out int m1);

        return (y1 - y0) * MonthsInYear + m1 - m0;
    }
}

internal partial class GregorianSystemArithmetic // Non-standard operations
{
    /// <inheritdoc />
    [Pure]
    public override Yemoda AddYears(Yemoda ymd, int years, out int roundoff)
    {
        ymd.Unpack(out int y, out int m, out int d);

        y = checked(y + years);
        YearsValidator.CheckOverflow(y);

        int daysInMonth = GregorianFormulae.CountDaysInMonth(y, m);
        roundoff = Math.Max(0, d - daysInMonth);
        // On retourne le dernier jour du mois si d > daysInMonth.
        return new Yemoda(y, m, roundoff > 0 ? daysInMonth : d);
    }

    /// <inheritdoc />
    [Pure]
    public override Yemoda AddMonths(Yemoda ymd, int months, out int roundoff)
    {
        ymd.Unpack(out int y, out int m, out int d);

        // On retranche 1 à "m" pour le rendre algébrique.
        m = 1 + MathZ.Modulo(checked(m - 1 + months), MonthsInYear, out int y0);
        y += y0;
        YearsValidator.CheckOverflow(y);

        int daysInMonth = GregorianFormulae.CountDaysInMonth(y, m);
        roundoff = Math.Max(0, d - daysInMonth);
        return new Yemoda(y, m, roundoff > 0 ? daysInMonth : d);
    }

    /// <inheritdoc />
    [Pure]
    public override Yedoy AddYears(Yedoy ydoy, int years, out int roundoff)
    {
        ydoy.Unpack(out int y, out int doy);

        y = checked(y + years);
        YearsValidator.CheckOverflow(y);

        int daysInYear = GregorianFormulae.CountDaysInYear(y);
        roundoff = Math.Max(0, doy - daysInYear);
        return new Yedoy(y, roundoff > 0 ? daysInYear : doy);
    }

    /// <inheritdoc />
    [Pure]
    public override Yemo AddYears(Yemo ym, int years, out int roundoff)
    {
        ym.Unpack(out int y, out int m);

        y = checked(y + years);
        YearsValidator.CheckForMonth(y);

        roundoff = 0;
        return new Yemo(y, m);
    }
}
