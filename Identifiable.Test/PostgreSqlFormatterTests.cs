// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Identifiable.Test;

partial class TimeGuidFormatterTests
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public class PostgreSqlFormatterTests : TimeGuidFormatterTests
    {
        protected override TimeGuid.IFormatter instance() => new TimeGuid.PostgreSqlFormatter();

        [Theory]
        [InlineData( 0 )]
        [InlineData( long.MaxValue )]
        [InlineData( long.MinValue )]
        [InlineData( 0x0102030405060708 )]
        public void Returns_bytes_0_to_7_as_time_with_version_1( long time )
        {
            this.time = time;
            var result = method();
            var bytes = result.ToByteArray();
            var expected = BitConverter.GetBytes( time );
            if ( !BitConverter.IsLittleEndian ) Array.Reverse( expected );
            Assert.Equal( expected[1..8], bytes[1..8] );

            // version is set in the most significant nibble of byte 0
            Assert.Equal( ( expected[0] & 0x0f ) + 0x10, bytes[0] );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( short.MaxValue )]
        [InlineData( short.MinValue )]
        [InlineData( 0x1234 )]
        public void Returns_bytes_8_to_9_as_clock_with_variant_0( short clock )
        {
            this.clock = clock;
            var result = method();
            var bytes = result.ToByteArray();
            var expected = BitConverter.GetBytes( clock );
            if ( !BitConverter.IsLittleEndian ) Array.Reverse( expected );
            Assert.Equal( ( expected[0] & 0xbf ), bytes[8] );
            Assert.Equal( expected[1], bytes[9] );
        }

        [Fact]
        public void Returns_bytes_10_to_15_as_node()
        {
            RandomNumberGenerator.Fill( node );
            var result = method();
            var bytes = result.ToByteArray();
            Assert.Equal( node, bytes[10..] );
        }
    }
}
