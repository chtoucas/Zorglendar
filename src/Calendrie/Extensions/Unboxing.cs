﻿// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) Tran Ngoc Bich. All rights reserved.

namespace Calendrie.Extensions;

// We provide two methods to <i>explicitely</i> unbox a boxed object.
// Explicitely because one can do it indirectly, e.g. via Select().
// Unboxing should only be done by developers of new core types, this is
// the main reason why we prefer extension methods to instance methods ---
// Box<T> has only one public method (Select), if Unbox() and TryUnbox()
// were instance methods, it would be like encouraging developers to use
// them, defeating the "raison d'être" of this class.
//
// For better alternatives, see BoxExtensions in Simple.

/// <summary>Provides extension methods for <see cref="Box{T}"/>.
/// <para>This class cannot be inherited.</para></summary>
public static class Unboxing
{
    /// <summary>Obtains the enclosed object.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="box"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="box"/> is an empty box.</exception>
    [Pure]
    public static T Unbox<T>(this Box<T> box)
        where T : class
    {
        Requires.NotNull(box);

        return box.IsEmpty ? Throw.EmptyBox<T>() : box.Content;
    }

    /// <summary>Attempts to obtain the enclosed object.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="box"/> is null.</exception>
    [Pure]
    public static bool TryUnbox<T>(this Box<T> box, [NotNullWhen(true)] out T? obj)
        where T : class
    {
        Requires.NotNull(box);

        if (box.IsEmpty)
        {
            obj = null;
            return false;
        }
        else
        {
            obj = box.Content;
            return true;
        }
    }
}
