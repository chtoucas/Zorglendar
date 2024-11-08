﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Horology;

/// <summary>Represents a clock.</summary>
public interface IClock
{
    /// <summary>Obtains a <see cref="Moment"/> value representing the current time.</summary>
    [Pure] Moment Now();

    /// <summary>Obtains a <see cref="DayNumber"/> value representing the current date.</summary>
    [Pure] DayNumber Today();
}
