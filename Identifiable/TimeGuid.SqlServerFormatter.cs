// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

namespace Identifiable;

partial class TimeGuid
{
    /// <summary>
    /// Custom formatter for time-based GUIDs that are optimal for use in SQL Server clustered indexes.
    /// </summary>
    public class SqlServerFormatter : IFormatter
    {
        /// <inheritdoc/>
        public Guid Format( long time, short clock, byte[] node )
        {
            if ( node == null ) throw new ArgumentNullException( nameof(node) );
            if ( node.Length != 6 ) throw new ArgumentException( $"{nameof(node)} must be a 6-byte array", nameof(node) );

            // first nibble would be overwritten by the version field in UUIDv1.
            // to maximize compatibility, move the version nibble to where it will report the identifier as variant 0.
            time = unchecked((time & 0x0FFFFFFFFFFFF000) << 4) + (time & 0x0FFF);
            clock &= 0x3FFF;

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );

            // endian swap the time and clock bytes on little-endian systems
            if ( BitConverter.IsLittleEndian )
            {
                Array.Reverse( timeBytes );
                Array.Reverse( clockBytes );
            }

            var output = new byte[16];
            Array.Copy( node, 0, output, 0, 6 );
            Array.Copy( clockBytes, 0, output, 6, 2 );
            Array.Copy( timeBytes, 6, output, 8, 2 );
            Array.Copy( timeBytes, 0, output, 10, 6 );

            // version and variant are transposed
            output[8] |= 0x10;
            output[6] |= 0x80;

            return new( output );
        }
    }
}
