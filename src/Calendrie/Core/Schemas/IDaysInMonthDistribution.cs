﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core.Schemas;

// REVIEW(api): I'm not yet sure it's the right interface, ReadOnlySpan<byte>,
// int[] or uint[]? This interface is implemented explicitely to hide its
// method.
// Cannot be part of ICalendricalSchema as it would require "static abstract"
// to be allowed with abstract classes.

/// <summary>Defines support for a function returning the distribution of days in month.</summary>
internal interface IDaysInMonthDistribution
{
    /// <summary>Obtains the number of days in each month of a common or leap year.</summary>
    /// <remarks>The span index matches the month index <i>minus one</i>.</remarks>
    [Pure] static abstract ReadOnlySpan<byte> GetDaysInMonthDistribution(bool leap);
}
