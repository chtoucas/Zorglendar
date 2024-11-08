﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core;

/// <summary>Defines a calendrical validator.</summary>
public interface ICalendricalValidator
{
    /// <summary>Validates the specified month.</summary>
    /// <exception cref="AoorException">The validation failed.</exception>
    void ValidateYearMonth(int year, int month, string? paramName = null);

    /// <summary>Validates the specified date.</summary>
    /// <exception cref="AoorException">The validation failed.</exception>
    void ValidateYearMonthDay(int year, int month, int day, string? paramName = null);

    /// <summary>Validates the specified ordinal date.</summary>
    /// <exception cref="AoorException">The validation failed.</exception>
    void ValidateOrdinal(int year, int dayOfYear, string? paramName = null);
}