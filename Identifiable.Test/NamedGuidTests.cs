// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using AutoFixture;
using System.Diagnostics.CodeAnalysis;

namespace Identifiable.Test;

[SuppressMessage("ReSharper", "ParameterHidesMember")]
public class NamedGuidTests
{
    public class Compute : NamedGuidTests
    {
        NamedGuidAlgorithm algorithm;
        Guid @namespace = Guid.NewGuid();
        string name = new Fixture().Create<string>();
        Guid method() => NamedGuid.ComputeInternal( algorithm, @namespace, name );

        [Fact]
        public void Requires_name()
        {
            name = null!;
            Assert.Throws<ArgumentNullException>( nameof(name), () => method() );
        }

        [Fact]
        public void Requires_Valid_algorithm()
        {
            algorithm = (NamedGuidAlgorithm) int.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>( nameof(algorithm), () => method() );
        }

        [Theory]
        [InlineData( NamedGuidAlgorithm.MD5 )]
        [InlineData( NamedGuidAlgorithm.SHA1 )]
        public void Returns_Variant_1_GUID( NamedGuidAlgorithm algorithm)
        {
            this.algorithm = algorithm;
            var actual = method();

            // isolate the variant bits
            var variant = actual.ToByteArray()[8] & 0b11000000;

            // ensure the first bit is on, the second is off
            Assert.Equal( 0b10000000, variant );
        }

        [Theory]
        [InlineData( NamedGuidAlgorithm.MD5, 0x30 )]
        [InlineData( NamedGuidAlgorithm.SHA1, 0x50 )]
        public void Returns_VersionForAlgorithm( NamedGuidAlgorithm algorithm, byte expected )
        {
            this.algorithm = algorithm;
            var actual = method();

            // isolate the first nibble
            var version = actual.ToByteArray()[7] & 0b11110000;

            // ensure the version is correct
            Assert.Equal( expected, version );
        }

        public class DeterministicCases : TheoryData<NamedGuidAlgorithm,string,Guid,Guid>
        {
            public DeterministicCases()
            {
                // example from https://tools.ietf.org/html/rfc4122#appendix-B
                // note the result comes from the errata: http://www.rfc-editor.org/errata_search.php?rfc=4122&eid=1352
                Add( NamedGuidAlgorithm.MD5, "www.widgets.com", new( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" ), new( "3d813cbb-47fb-32ba-91df-831e1593ac29" ) );

                // result for the same inputs in SHA1
                Add( NamedGuidAlgorithm.SHA1, "www.widgets.com", new( "6ba7b810-9dad-11d1-80b4-00c04fd430c8" ), new( "21f7f8de-8051-5b89-8680-0195ef798b6a" ) );
            }
        }

        [Theory]
        [ClassData( typeof( DeterministicCases ) )]
        public void Returns_Deterministic_Guid( NamedGuidAlgorithm algorithm, string name, Guid @namespace, Guid expected )
        {
            this.algorithm = algorithm;
            this.name = name;
            this.@namespace = @namespace;
            var actual = method();

            Assert.Equal( expected, actual );
        }
    }
}
