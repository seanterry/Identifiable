// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Identifiable;

/// <summary>
/// Creates GUIDs based on the current system time.
/// </summary>
public static partial class TimeGuid
{
    /// <summary>
    /// Start of the Gregorian calendar, used per RFC-4122 for Version 1.
    /// </summary>
    static readonly long GregorianCalendarStart = new DateTime( 1582, 10, 15, 0, 0, 0, DateTimeKind.Utc ).Ticks;

    /// <summary>
    /// Static cache of formatters.
    /// </summary>
    static readonly ConcurrentDictionary<TimeGuidLayout, IFormatter> Formatters = new();

    /// <summary>
    /// Creates and returns the formatter for the given layout.
    /// </summary>
    static IFormatter FormatterFactory( TimeGuidLayout layout ) =>
        layout switch
        {
            TimeGuidLayout.Standard => new StandardFormatter(),
            TimeGuidLayout.SqlServer => new SqlServerFormatter(),
            TimeGuidLayout.PostgreSql => new PostgreSqlFormatter(),
            _ => throw new ArgumentOutOfRangeException( nameof( layout ) )
        };

    /// <summary>
    /// Returns the formatter for the given layout.
    /// </summary>
    static IFormatter GetFormatter( TimeGuidLayout layout ) =>
        Formatters.GetOrAdd( layout, FormatterFactory );

    /// <summary>
    /// Creates and returns a time-based GUID using the given formatter.
    /// Use of this method allows custom formatters to be used.
    /// </summary>
    /// <param name="formatter">Formatter for the resulting GUID.</param>
    public static Guid Create( IFormatter formatter )
    {
        var time = DateTime.UtcNow.Ticks - GregorianCalendarStart;
        var clockBytes = new byte[2];
        var node = new byte[6];

        // since node is different every call, clock should likewise be random
        RandomNumberGenerator.Fill( node );
        RandomNumberGenerator.Fill( clockBytes );

        // set multicast bit for random node
        node[0] |= 0x01;
        var clock = BitConverter.ToInt16( clockBytes, 0 );

        return formatter.Format( time, clock, node );
    }

    /// <summary>
    /// Creates and returns a time-based GUID using the given layout.
    /// </summary>
    /// <param name="layout">Layout of the identifier.</param>
    public static Guid Create( TimeGuidLayout layout ) =>
        Create( GetFormatter( layout ) );
}
