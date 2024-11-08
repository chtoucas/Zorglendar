﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core;

/// <summary>Defines a calendrical schema or a calendar with a virtual month.</summary>
public interface IVirtualMonthFeaturette : ICalendricalKernel
{
    /// <summary>Gets the virtual month.</summary>
    int VirtualMonth { get; }
}
