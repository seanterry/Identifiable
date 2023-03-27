// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Identifiable.Test;

partial class TimeGuidFormatterTests
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public class SqlServerFormatterTests : TimeGuidFormatterTests
    {
        protected override TimeGuid.IFormatter instance() => new TimeGuid.SqlServerFormatter();

        [Theory]
        [InlineData( long.MinValue )]
        [InlineData( long.MaxValue )]
        [InlineData( 0 )]
        [InlineData( 0x01234567890ABCDEF )]
        public void Returns_bytes_10_to_15_as_time_high( long time )
        {
            this.time = time;
            var result = method();
            var bytes = result.ToByteArray();

            var expected = BitConverter.GetBytes( time << 4 );
            if ( BitConverter.IsLittleEndian ) Array.Reverse( expected );

            Assert.Equal( expected[..6], bytes[10..] );
        }

        [Theory]
        [InlineData( long.MinValue )]
        [InlineData( long.MaxValue )]
        [InlineData( 0 )]
        [InlineData( 0x0123456789ABCDEF )]
        public void Returns_bytes_8_to_9_as_time_low_with_version_in_variant_location( long time )
        {
            this.time = time;
            var result = method();
            var bytes = result.ToByteArray();

            var expected = BitConverter.GetBytes( time );
            if ( BitConverter.IsLittleEndian ) Array.Reverse( expected );

            Assert.Equal( expected[7], bytes[9] );

            // version is set in the most significant nibble of byte 8
            Assert.Equal( ( expected[6] & 0x0f ) + 0x10, bytes[8] );
        }

        [Theory]
        [InlineData( short.MinValue )]
        [InlineData( short.MaxValue )]
        [InlineData( 0 )]
        [InlineData( 0x1234 )]
        public void Returns_bytes_6_to_7_as_clock_with_original_variant( short clock )
        {
            this.clock = clock;
            var result = method();
            var bytes = result.ToByteArray();
            var expected = BitConverter.GetBytes( clock );
            if ( BitConverter.IsLittleEndian ) Array.Reverse( expected );
            Assert.Equal( ( expected[0] & 0x3f ) + 0x80, bytes[6] );
            Assert.Equal( expected[1], bytes[7] );
        }

        [Fact]
        public void Returns_bytes_0_to_5_as_node()
        {
            RandomNumberGenerator.Fill( node );
            var result = method();
            var bytes = result.ToByteArray();
            Assert.Equal( node[..6], bytes[..6] );
        }
    }
}
