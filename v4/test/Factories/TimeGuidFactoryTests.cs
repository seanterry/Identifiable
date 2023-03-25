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
using System.Linq;
using Xunit;

namespace Identifiable.Factories
{
    public class TimeGuidFactoryTests
    {
        ITimeGuidFactory instance => new TimeGuidFactory();

        /// <summary>
        /// Tests of the Epoch property.
        /// </summary>

        public class Epoch
        {
            [Fact]
            public void equals_gegorianReformDate_October_15_1582()
            {
                var expected = new DateTime( 1582, 10, 15 ).Ticks;
                var actual = TimeGuidFactory.Epoch;

                Assert.Equal( expected, actual );
            }
        }

        public class Create : TimeGuidFactoryTests
        {
            TimeGuidLayout layout = default( TimeGuidLayout );
            Guid invoke() => instance.Create( layout );

            [Fact]
            public void requires_valid_layout()
            {
                layout = default( TimeGuidLayout );
                Assert.Throws<NotImplementedException>( () => invoke() );
            }
            
            [Fact]
            public void standard_returns_variant_one()
            {
                layout = TimeGuidLayout.Standard;
                var actual = invoke().ToByteArray();

                // first bit of byte 8 should be set
                Assert.NotEqual( 0, actual[8] & 0x80 );
                // second bit of byte 8 should not be set
                Assert.Equal( 0, actual[8] & 0x40 );
            }

            /// <summary>
            /// Standard layout should identify as version 1 uuid.
            /// </summary>

            [Fact]
            public void standard_returns_version_one()
            {
                layout = TimeGuidLayout.Standard;
                var actual = invoke().ToByteArray();
                Assert.Equal( 0x10, actual[7] & 0xF0 );
            }

            /// <summary>
            /// Multicast bit of the node must be set when randomly generated.
            /// </summary>

            [Fact]
            public void standard_returns_node_with_multicast_bit()
            {
                layout = TimeGuidLayout.Standard;
                var actual = invoke().ToByteArray();
                Assert.Equal( 1, actual[10] & 0x01 );
            }

            /// <summary>
            /// SQL Server layout should identify as variant zero to avoid collisions with real Guids.
            /// </summary>

            [Fact]
            public void sqlServer_returns_variant_zero()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = invoke().ToByteArray();

                // first bit of byte 8 should be zero
                Assert.Equal( 0, actual[8] & 0x80 );
            }

            /// <summary>
            /// The version has been moved to the same field as the variant in SQL Server layout.
            /// </summary>

            [Fact]
            public void sqlServer_returns_version_one()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = invoke().ToByteArray();

                // first nibble of byte 8 should be one
                Assert.Equal( 1, actual[8] >> 4 );
            }

            /// <summary>
            /// The originating variant (for UUIDv1 transposability) should be one.
            /// </summary>

            [Fact]
            public void sqlServer_returns_original_variant_one()
            {
                layout = TimeGuidLayout.SqlServer;
                var actual = invoke().ToByteArray();
                Assert.NotEqual( 0, actual[6] & 0x80 );
                Assert.Equal( 0, actual[6] & 0x40 );
            }

            /// <summary>
            /// Multicast bit of the node must be set when randomly generated.
            /// </summary>

            [Fact]
            public void sqlServer_returns_node_with_multicast_bit()
            {
                layout = TimeGuidLayout.SqlServer;
                var guid = invoke();
                var actual = guid.ToByteArray();
                Assert.Equal( 1, actual[0] & 0x01 );
            }

            /// <summary>
            /// All hex characters in SQL server and Standard layouts should be transposable.
            /// This just checks that all of the characters are present, not that they are in the correct order.
            /// </summary>

            [Fact]
            public void sqlServer_returns_transposed_standard()
            {
                var time = DateTime.UtcNow.Ticks - TimeGuidFactory.Epoch;
                short clock = 0x6677;
                var node = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 };

                var sql = Formatters.SqlServerFormatter.Instance.Create( time, clock, node ).ToString().OrderBy( _ => _ );
                var standard = Formatters.StandardFormatter.Instance.Create( time, clock, node ).ToString().OrderBy( _ => _ );

                Assert.True( sql.SequenceEqual( standard ) );
            }
        }

    }
}
