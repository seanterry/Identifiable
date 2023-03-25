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
    public class SqlServerFormatterTests
    {
        ITimeGuidFormatter instance => new SqlServerFormatter();

        public class Create : SqlServerFormatterTests
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
            /// Most significant bytes should contain the high time value.
            /// </summary>

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x01234567890ABCDEF )]

            public void returns_bytes10to15_timeHigh( long timeValue )
            {
                time = timeValue;
                var guid = invoke();
                var actual = guid.ToByteArray();

                // skip the low sequence
                timeValue >>= 12;

                for ( var i = 15; i >= 10; i-- )
                {
                    var expected = timeValue & 0xFF;
                    Assert.Equal( expected, actual[i] );
                    timeValue >>= 8;
                }
            }

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x0123456789ABCDEF )]

            public void returns_byte8_timeLow_with_variantVersion( long timeValue )
            {
                time = timeValue;
                var guid = invoke();
                var actual = guid.ToByteArray();
                var expected = 0x10 + ((timeValue >> 8) & 0x0F);
                Assert.Equal( expected, actual[8] );
            }

            [Theory]
            [InlineData( long.MinValue )]
            [InlineData( long.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x01234567890ABCDEF )]

            public void returns_bytes9_timeLow( long timeValue )
            {
                time = timeValue;
                var guid = invoke();
                var actual = guid.ToByteArray();
                var expected = time & 0xFF;
                Assert.Equal( expected, actual[9] );
            }

            [Theory]
            [InlineData( short.MinValue )]
            [InlineData( short.MaxValue )]
            [InlineData( 0 )]
            [InlineData( 0x1234 )]

            public void returns_bytes6_clockHigh_with_variant( short clockValue )
            {
                clock = clockValue;
                var guid = invoke();
                var actual = guid.ToByteArray();
                const int variant = 0x80;
                var expected = variant + ((clockValue >> 8) & 0x3F);

                Assert.Equal( expected, actual[6] );
            }

            /// <summary>
            /// Bytes 0 to 5 contain the node.
            /// </summary>

            [Fact]
            public void returns_bytes0to5_node()
            {
                node = new byte[6];
                new Random().NextBytes( node );
                var guid = invoke();
                var actual = guid.ToByteArray();

                for ( var i = 0; i < 6; i++ )
                {
                    Assert.Equal( node[i], actual[i] );
                }
            }
        }
    }
}