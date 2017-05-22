﻿using System;

namespace Fidget.Extensions.Guids.Internal
{
    /// <summary>
    /// Provider for creating sequential GUIDs for use with SQL Server.
    /// </summary>

    class SqlServerSequentialGuidFactory : ISequentialGuidFactory
    {
        /// <summary>
        /// Constructs a provider for creating sequential GUIDs for SQL Server.
        /// </summary>
        
        SqlServerSequentialGuidFactory() {}

        /// <summary>
        /// Gets a singleton instance of the type.
        /// </summary>
        
        public static ISequentialGuidFactory Instance { get; } = new SqlServerSequentialGuidFactory();

        /// <summary>
        /// Creates and returns a time-based sequential identifier (UUIDv1) arranged for sorting
        /// within SQL Server. The values generated by this algorithm are not valid GUID structures
        /// but can be transposed to UUIDv1.
        /// </summary>
        /// <param name="time">Number of 100 nanosecond intervals since 00:00:00.00, 15 October 1582.</param>
        /// <param name="clock">Value that represents an incremented clock sequence.</param>
        /// <param name="node">48-bit node identifier.</param>
        
        public Guid Create( long time, int clock, byte[] node )
        {
            // first nibble would be overwritten for the version field in UUIDv1.
            // to maximize compatibility, move the version nibble to where it will
            // report this identifier as variant zero.
            time = unchecked( ( time & 0x0FFFFFFFFFFFF000 ) << 4 ) + ( time & 0x0FFF );
            clock &= 0x3FFF;

            var timeBytes = BitConverter.GetBytes( time );
            var clockBytes = BitConverter.GetBytes( clock );

            if ( BitConverter.IsLittleEndian ) {
                Array.Reverse( timeBytes );
                Array.Reverse( clockBytes );
            }
            
            var output = new byte[16];
            Array.Copy( node, 0, output, 0, 6 );
            Array.Copy( clockBytes, 2, output, 6, 2 );
            Array.Copy( timeBytes, 6, output, 8, 2 );
            Array.Copy( timeBytes, 0, output, 10, 6 );
            
            // version and variant are in the same position in this format
            output[8] |= 0x10;
            output[6] |= 0x80;
            
            return new Guid( output );
        }
    }
}