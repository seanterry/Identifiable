// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

namespace Identifiable;

partial class TimeGuid
{
    /// <summary>
    /// Custom formatter for time-based GUIDs that are sort sequentially in PostgreSQL.
    /// </summary>
    public class PostgreSqlFormatter : IFormatter
    {
        /// <inheritdoc/>
        public Guid Format( long time, short clock, byte[] node )
        {
            if ( node == null ) throw new ArgumentNullException( nameof(node) );
            if ( node.Length != 6 ) throw new ArgumentException( $"{nameof(node)} must be a 6-byte array", nameof(node) );

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );

            // time is reversed from the standard format
            // clock is in the same order as the standard format
            if ( !BitConverter.IsLittleEndian )
            {
                Array.Reverse( timeBytes );
                Array.Reverse( clockBytes );
            }

            var output = new byte[16];
            Array.Copy( timeBytes, 0, output, 0, 8 );
            Array.Copy( clockBytes, 0, output, 8, 2 );
            Array.Copy( node, 0, output, 10, 6 );

            // set version
            // since time is reversed, this is moved to byte 0.
            output[0] &= 0x1f;
            output[0] |= 0x10;

            // set variant; report as variant 0
            output[8] &= 0xBF;

            return new( output );
        }
    }
}
