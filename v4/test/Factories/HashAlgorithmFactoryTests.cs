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
using System.Security.Cryptography;
using Xunit;

namespace Identifiable.Factories
{
    public class HashAlgorithmFactoryTests
    {
        IHashAlgorithmFactory instance => new HashAlgorithmFactory();
        
        public class Create : HashAlgorithmFactoryTests
        {
            NamedGuidAlgorithm algorithm;
            HashAlgorithm invoke( out byte version ) => instance.Create( algorithm, out version );

            [Fact]
            public void when_algorithm_unknown_throws()
            {
                algorithm = default(NamedGuidAlgorithm);
                Assert.Throws<NotImplementedException>( ()=> invoke( out byte version ) );
            }

            [Theory]
            [InlineData( NamedGuidAlgorithm.MD5, typeof(MD5), StandardGuidVersion.UUIDv3 )]
            [InlineData( NamedGuidAlgorithm.SHA1, typeof(SHA1), StandardGuidVersion.UUIDv5 )]
            public void returns_known_algorithm( NamedGuidAlgorithm algorithm, Type expectedType, byte expectedVersion )
            {
                this.algorithm = algorithm;
            
                using ( var actualAlgorithm = invoke( out byte version ) )
                {
                    Assert.IsAssignableFrom( expectedType, actualAlgorithm );
                    Assert.Equal( expectedVersion, version );
                }
            }
        }
    }
}