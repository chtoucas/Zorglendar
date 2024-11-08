﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core;

// REVIEW(api): if C# supported "static abstract" methods, we could use static
// properties instead of instance props. Idem with the other featurettes.

/// <summary>Defines a calendrical schema or a calendar with a fixed number of months in a year.
/// <para>We say that the schema/calendar is <i>regular</i>.</para>
/// <para>Most calendars implement this interface.</para></summary>
public interface IRegularFeaturette : ICalendricalKernel
{
    /// <summary>Gets the total number of months in a year.</summary>
    int MonthsInYear { get; }
}
