﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Core.Utilities;

using System.Numerics;

/// <summary>Defines a mechanism for comparing two values to determine equality.</summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
/// <typeparam name="TOther">The type that will be compared with <typeparamref name="TSelf" />.
/// </typeparam>
public interface IEqualityOperators<TSelf, TOther> :
    IEqualityOperators<TSelf, TOther, bool>,
    IEquatable<TOther>
    where TSelf : IEqualityOperators<TSelf, TOther>?
{ }
