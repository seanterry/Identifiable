// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

namespace Identifiable;

/// <summary>
/// Hashing algorithms for <see cref="NamedGuid" />.
/// </summary>
public enum NamedGuidAlgorithm
{
    /// <summary>
    /// Generates a <see cref="NamedGuid" /> using the MD5 hash algorithm.
    /// This will produce a version 3 UUID.
    /// </summary>
    MD5 = 3,

    /// <summary>
    /// Generates a <see cref="NamedGuid" /> using the SHA1 hash algorithm.
    /// This will produce a version 5 UUID.
    /// </summary>
    SHA1 = 5,
}
