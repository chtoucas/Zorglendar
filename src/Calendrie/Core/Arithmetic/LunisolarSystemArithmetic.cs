﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core.Arithmetic;

using __Lunisolar = CalendricalConstants.Lunisolar;

/// <summary>
/// Provides the core mathematical operations on dates for schemas with profile
/// <see cref="CalendricalProfile.Lunisolar"/>.
/// <para>This class cannot be inherited.</para>
/// </summary>
internal sealed partial class LunisolarSystemArithmetic : SystemArithmetic
{
    private const int MinDaysInYear = __Lunisolar.MinDaysInYear;
    private const int MinDaysInMonth = __Lunisolar.MinDaysInMonth;
    private const int MaxDaysViaDayOfYear_ = MinDaysInYear;
    private const int MaxDaysViaDayOfMonth_ = MinDaysInMonth;

    /// <summary>
    /// Initializes a new instance of the <see cref="LunisolarSystemArithmetic"/> class with the
    /// specified schema.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="segment"/> is null.</exception>
    /// <exception cref="ArgumentException">The underlying schema does not have the expected
    /// profile <see cref="CalendricalProfile.Lunisolar"/>.
    /// </exception>
    public LunisolarSystemArithmetic(SystemSegment segment) : base(segment)
    {
        Debug.Assert(MaxDaysViaDayOfMonth_ >= MinMinDaysInMonth);

        Requires.Profile(Schema, CalendricalProfile.Lunisolar, nameof(segment));

        MaxDaysViaDayOfYear = MaxDaysViaDayOfYear_;
        MaxDaysViaDayOfMonth = MaxDaysViaDayOfMonth_;
    }
}

internal partial class LunisolarSystemArithmetic // Operations on Yemoda
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
        int daysSinceEpoch = checked(Schema.CountDaysSinceEpoch(y, m, d) + days);
        DaysValidator.CheckOverflow(daysSinceEpoch);

        return Schema.GetDateParts(daysSinceEpoch);
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
                (_, m, int d0) = Schema.GetDatePartsAtEndOfYear(y);
                dom += d0;
            }
            else
            {
                m--;
                dom += Schema.CountDaysInMonth(y, m);
            }
        }
        else if (dom > MinDaysInMonth)
        {
            int daysInMonth = Schema.CountDaysInMonth(y, m);
            if (dom > daysInMonth)
            {
                dom -= daysInMonth;
                if (m == Schema.CountMonthsInYear(y))
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
            d < MinDaysInMonth || d < Schema.CountDaysInMonth(y, m)
                ? new Yemoda(y, m, d + 1)
            : m < Schema.CountMonthsInYear(y) ? Yemoda.AtStartOfMonth(y, m + 1)
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

internal partial class LunisolarSystemArithmetic // Operations on Yedoy
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

        return Schema.GetOrdinalParts(daysSinceEpoch);
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
            doy += Schema.CountDaysInYear(y);
        }
        else
        {
            int daysInYear = Schema.CountDaysInYear(y);
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
            doy < MinDaysInYear || doy < Schema.CountDaysInYear(y)
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

internal partial class LunisolarSystemArithmetic // Operations on Yemo
{
    /// <inheritdoc />
    [Pure]
    public override Yemo AddMonths(Yemo ym, int months)
    {
        ym.Unpack(out int y, out int m);

        int monthsSinceEpoch = checked(Schema.CountMonthsSinceEpoch(y, m) + months);
        MonthsValidator.CheckOverflow(monthsSinceEpoch);

        return Schema.GetMonthParts(monthsSinceEpoch);
    }

    /// <inheritdoc />
    [Pure]
    public override int CountMonthsBetween(Yemo start, Yemo end)
    {
        start.Unpack(out int y0, out int m0);
        end.Unpack(out int y1, out int m1);

        return Schema.CountMonthsSinceEpoch(y1, m1) - Schema.CountMonthsSinceEpoch(y0, m0);
    }
}

internal partial class LunisolarSystemArithmetic // Non-standard operations
{
    /// <inheritdoc />
    [Pure]
    public override Yemoda AddYears(Yemoda ymd, int years, out int roundoff)
    {
        ymd.Unpack(out int y0, out int m, out int d);

        int y = checked(y0 + years);
        YearsValidator.CheckOverflow(y);

        var sch = Schema;
        int monthsInYear = sch.CountMonthsInYear(y);
        if (m > monthsInYear)
        {
            // The target year y has less months than the year y0, we
            // return the end of the target year.
            // roundoff =
            //   "days" after the end of (y0, monthsInYear) until (y0, m, d) included
            //   + diff between end of (y0, monthsInYear) and (y, monthsInYear)
            roundoff = d;
            for (int i = monthsInYear + 1; i < m; i++)
            {
                roundoff += sch.CountDaysInMonth(y0, i);
            }
            m = monthsInYear;
            int daysInMonth = sch.CountDaysInMonth(y, m);
            roundoff += Math.Max(0, d - daysInMonth);
            return new Yemoda(y, m, roundoff > 0 ? daysInMonth : d);
        }
        else
        {
            int daysInMonth = sch.CountDaysInMonth(y, m);
            roundoff = Math.Max(0, d - daysInMonth);
            return new Yemoda(y, m, roundoff > 0 ? daysInMonth : d);
        }
    }

    /// <inheritdoc />
    [Pure]
    public override Yemoda AddMonths(Yemoda ymd, int months, out int roundoff)
    {
        int d = ymd.Day;

        var (y, m) = AddMonths(ymd.Yemo, months);
        YearsValidator.CheckOverflow(y);

        int daysInMonth = Schema.CountDaysInMonth(y, m);
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

        int daysInYear = Schema.CountDaysInYear(y);
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

        int monthsInYear = Schema.CountMonthsInYear(y);
        roundoff = Math.Max(0, m - monthsInYear);
        return new Yemo(y, roundoff > 0 ? monthsInYear : m);
    }
}
