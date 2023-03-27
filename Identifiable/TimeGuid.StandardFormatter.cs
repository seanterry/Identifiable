// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

namespace Identifiable;

partial class TimeGuid
{
    /// <summary>
    /// Formatter for creating standard time-based GUIDs as defined in RFC 4122.
    /// </summary>
    public class StandardFormatter : IFormatter
    {
        /// <inheritdoc/>
        public Guid Format( long time, short clock, byte[] node )
        {
            if ( node == null ) throw new ArgumentNullException( nameof(node) );
            if ( node.Length != 6 ) throw new ArgumentException( $"{nameof(node)} must be a 6-byte array", nameof(node) );

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );

            // reverse the clock bytes on big-endian systems
            // reverse the time bytes on little-endian systems
            Array.Reverse( BitConverter.IsLittleEndian ? timeBytes : clockBytes );

            var output = new byte[16];
            Array.Copy( timeBytes, 0, output, 0, 8 );
            Array.Copy( clockBytes, 0, output, 8, 2 );
            Array.Copy( node, 0, output, 10, 6 );

            // set version
            output[7] &= 0x1f;
            output[7] |= 0x10;

            // set variant
            output[8] &= 0xBF;
            output[8] |= 0x80;

            return new( output );
        }
    }
}
