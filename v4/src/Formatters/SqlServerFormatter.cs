/*  Copyright 2017 Sean Terry

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;

namespace Identifiable.Formatters
{
    /// <summary>
    /// Custom formatter for formatting a time-based GUID that is optimal for use in SQL Server clustered indexes.
    /// </summary>

    public class SqlServerFormatter : ITimeGuidFormatter
    {
        /// <summary>
        /// Gets the default instance of the type.
        /// </summary>
        
        public static ITimeGuidFormatter Instance { get; } = new SqlServerFormatter();

        /// <summary>
        /// Creates and returns a time-based identifier.
        /// </summary>
        /// <param name="time">
        /// Number of 100 nanosecond intervals since 00:00:00.00, 15 October 1582.
        /// The most-significant nibble (4 bits) will be overwritten by the identifier version, 
        /// causing a roll-over of values once every approximately two-thousand years.
        /// </param>
        /// <param name="clock">
        /// Value whose least-significant 14 bits will be used as the clock sequence.
        /// According to spec, this value should be initialized to a random number for each node used.
        /// </param>
        /// <param name="node">
        /// 48-bit node identifier.
        /// If set to a random value, the mutlicast bit must be set.
        /// </param>

        public Guid Create( long time, short clock, byte[] node )
        {
            if ( node == null ) throw new ArgumentNullException( nameof( node ) );
            if ( node.Length != 6 ) throw new ArgumentException( $"{nameof( node )} must be a 6-byte array", nameof( node ) );

            // first nibble would be overwritten for the version field in UUIDv1.
            // to maximize compatibility, move the version nibble to where it will
            // report this identifier as variant zero.
            time = unchecked((time & 0x0FFFFFFFFFFFF000) << 4) + (time & 0x0FFF);
            clock &= 0x3FFF;

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );

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

            return new Guid( output );
        }
    }
}