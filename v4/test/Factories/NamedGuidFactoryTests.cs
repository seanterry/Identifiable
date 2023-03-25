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
using System.Collections.Generic;
using Xunit;

namespace Identifiable.Factories
{
    public class NamedGuidFactoryTests
    {
        IHashAlgorithmFactory algorithmFactory = new HashAlgorithmFactory();
        INamedGuidFactory instance => new NamedGuidFactory( algorithmFactory );

        public class Constructor : NamedGuidFactoryTests
        {
            [Fact]
            public void requires_algorithmFactory()
            {
                algorithmFactory = null;
                Assert.Throws<ArgumentNullException>( nameof(algorithmFactory), ()=>instance );
            }
        }

        public class Create : NamedGuidFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            Guid @namespace = Guid.NewGuid();
            string name = string.Empty;

            Guid invoke() => instance.Create( algorithm, @namespace, name );

            [Fact]
            public void requires_name()
            {
                name = null;
                Assert.Throws<ArgumentNullException>( nameof(name), ()=>invoke() );
            }

            [Fact]
            public void requires_valid_algorithm()
            {
                algorithm = default(NamedGuidAlgorithm);
                Assert.Throws<NotImplementedException>( ()=> invoke() );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_variant_1( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first two bits
                var variant = actual.ToByteArray()[8] & 0b11000000;

                // ensure the first bit is on, the second bit is off
                Assert.Equal( 0b10000000, variant );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_version_for_algorithm( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first nibble
                var version = actual.ToByteArray()[7] & 0b11110000;
                
                switch ( algorithm )
                {
                    case NamedGuidAlgorithm.MD5:
                        Assert.Equal( StandardGuidVersion.UUIDv3, version );
                        break;
                    case NamedGuidAlgorithm.SHA1:
                        Assert.Equal( StandardGuidVersion.UUIDv5, version );
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Expected results for an identifier for the text "Hello World".
            /// </summary>
            
            public static IEnumerable<object[]> deterministic_results()
            {
                const string name = "Hello World";
                const string ns = "bb205f8f-f3c6-4204-bfdf-7ceeaca6d593";
                yield return new object[] { NamedGuidAlgorithm.MD5, new Guid( ns ), name, new Guid( "77d1b2af-f19e-34e8-a2a8-dba40d8596ba" ) };
                yield return new object[] { NamedGuidAlgorithm.SHA1, new Guid( ns ), name, new Guid( "d18fd408-dc10-52a9-9a61-d1eb6d6a3088" ) };
            }

            [Theory]
            [MemberData(nameof(deterministic_results))]

            public void returns_deterministic( NamedGuidAlgorithm algorithm, Guid @namespace, string name, Guid expected )
            {
                this.@namespace = @namespace;
                this.algorithm = algorithm;
                this.name = name;

                var actual = invoke();
                Assert.Equal( expected, actual );

                // ensure changing the text yields a different result
                this.name += "!";
                actual = invoke();
                Assert.NotEqual( expected, actual );
            }
        }

        public class Compute : NamedGuidFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            Guid @namespace = Guid.NewGuid();
            string name = string.Empty;

            Guid invoke() => instance.Compute( algorithm, @namespace, name );

            [Fact]
            public void requires_name()
            {
                name = null;
                Assert.Throws<ArgumentNullException>( nameof( name ), () => invoke() );
            }

            [Fact]
            public void requires_valid_algorithm()
            {
                algorithm = default( NamedGuidAlgorithm );
                Assert.Throws<NotImplementedException>( () => invoke() );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_variant_1( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first two bits
                var variant = actual.ToByteArray()[8] & 0b11000000;

                // ensure the first bit is on, the second bit is off
                Assert.Equal( 0b10000000, variant );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5 )]
            [InlineData( NamedGuidAlgorithm.SHA1 )]
            public void returns_version_for_algorithm( NamedGuidAlgorithm algorithm )
            {
                name = $"{Guid.NewGuid()}";
                this.algorithm = algorithm;
                var actual = invoke();

                // isolate the first nibble
                var version = actual.ToByteArray()[7] & 0b11110000;

                switch ( algorithm )
                {
                    case NamedGuidAlgorithm.MD5:
                        Assert.Equal( StandardGuidVersion.UUIDv3, version );
                        break;
                    case NamedGuidAlgorithm.SHA1:
                        Assert.Equal( StandardGuidVersion.UUIDv5, version );
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public class DeterministicResults : TheoryData<NamedGuidAlgorithm, Guid, string, Guid>
            {
                public DeterministicResults()
                {
                    // example from https://tools.ietf.org/html/rfc4122#appendix-B
                    // note the result comes from the errata: http://www.rfc-editor.org/errata_search.php?rfc=4122&eid=1352
                    Add( NamedGuidAlgorithm.MD5, new Guid( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" ), "www.widgets.com", new Guid( "3d813cbb-47fb-32ba-91df-831e1593ac29" ) );
                }
            }

            [Theory]
            [ClassData( typeof( DeterministicResults ) )]
            public void returns_deterministic( NamedGuidAlgorithm algorithm, Guid @namespace, string name, Guid expected )
            {
                this.@namespace = @namespace;
                this.algorithm = algorithm;
                this.name = name;

                var actual = invoke();
                Assert.Equal( expected, actual );

                // ensure changing the text yields a different result
                this.name += "!";
                actual = invoke();
                Assert.NotEqual( expected, actual );
            }
        }
    }
}