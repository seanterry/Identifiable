// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

namespace Identifiable;

partial class TimeGuid
{
    /// <summary>
    /// Defines a formatter for time-based GUIDs.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats and returns a time-based GUID.
        /// </summary>
        /// <param name="time">
        /// Number of 100 nanosecond intervals since the start of the Gregorian calendar (15 October 1582).
        /// The most-significant nibble (4 bits) will be overwritten by the identifier version, causing a roll-over
        /// of values in approximately 3400 CE (according to RFC 4122).
        /// </param>
        /// <param name="clock">
        /// Value whose least-significant 14 bits will be used as the clock sequence.
        /// According to RFC 4122, this value should be initialized to a random value for each node used.
        /// </param>
        /// <param name="node">
        /// 48-bit node identifier.
        /// If set to a random value, the multicast bit must be set.
        /// </param>
        /// <returns>A GUID composed of the given values formatted using the current algorithm.</returns>
        public Guid Format( long time, short clock, byte[] node );
    }
}
