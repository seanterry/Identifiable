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
using Xunit;

namespace Identifiable.Formatters
{
    public class StandardFormatterTests
    {
        ITimeGuidFormatter instance => new StandardFormatter();

        public class Create : StandardFormatterTests
        {
            long time = 0;
            short clock = 0;
            byte[] node = new byte[6];

            Guid invoke() => instance.Create( time, clock, node );

            [Fact]
            public void requires_node()
            {
                node = null;
                Assert.Throws<ArgumentNullException>( "node", () => invoke() );
            }

            [Theory]
            [InlineData( 5 )]
            [InlineData( 7 )]
            public void requires_node_6_bytes( int size )
            {
                node = new byte[size];
                Assert.Throws<ArgumentException>( "node", () => invoke() );
            }

            /// <summary>
            /// The least-significant time bytes contain the time value.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( long.MaxValue )]
            [InlineData( long.MinValue )]
            [InlineData( 0x0102030405060708 )]
            public void returns_bytes0through6_time( long timeValue )
            {
                time = timeValue;
                var guid = invoke();
                var actual = guid.ToByteArray();

                for ( var i = 0; i <= 6; i++ )
                {
                    var expected = (timeValue >> (i * 8)) & 0xff;
                    Assert.Equal( expected, actual[i] );
                }
            }

            /// <summary>
            /// The most significant nibble of the time stores the version.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( long.MaxValue )]
            [InlineData( long.MinValue )]
            [InlineData( 0x0102030405060708 )]
            public void returns_byte7_timeHigh_with_version( long timeValue )
            {
                time = timeValue;
                var guid = invoke();
                var actual = guid.ToByteArray();
                const int version = 0x10;
                var expected = version + (((timeValue >> 56) & 0x0f));

                Assert.Equal( expected, actual[7] );
            }

            /// <summary>
            /// Byte 8 should contain the high clock sequence and the variant.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( short.MaxValue )]
            [InlineData( short.MinValue )]
            [InlineData( 0x1234 )]
            public void returns_byte8_clockHigh_with_variant( short clockValue )
            {
                clock = clockValue;
                var guid = invoke();
                var actual = guid.ToByteArray();

                const int variant = 0x80;
                var expected = variant + ((clockValue >> 8) & 0x3f);

                Assert.Equal( expected, actual[8] );
            }

            /// <summary>
            /// Byte 9 should contain the lower clock value sequence.
            /// </summary>

            [Theory]
            [InlineData( 0 )]
            [InlineData( short.MaxValue )]
            [InlineData( short.MinValue )]
            [InlineData( 0x1234 )]
            public void returns_byte9_clockLow( short clockValue )
            {
                clock = clockValue;
                var guid = invoke();
                var actual = guid.ToByteArray();
                var expected = clockValue & 0xff;

                Assert.Equal( expected, actual[9] );
            }

            /// <summary>
            /// Bytes 10 to 15 contain the node.
            /// </summary>

            [Fact]
            public void returns_bytes10to15_node()
            {
                node = new byte[6];
                new Random().NextBytes( node );
                var guid = invoke();
                var actual = guid.ToByteArray();

                for ( var i = 0; i < 6; i++ )
                {
                    Assert.Equal( node[i], actual[i + 10] );
                }
            }
        }
    }
}