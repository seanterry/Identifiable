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

namespace Identifiable.Factories
{
    /// <summary>
    /// Factory for generating hash algorithms for name-based identifiers.
    /// </summary>

    public class HashAlgorithmFactory : IHashAlgorithmFactory
    {
        /// <summary>
        /// Default instance of the type.
        /// </summary>
        
        internal static IHashAlgorithmFactory Instance { get; } = new HashAlgorithmFactory();

        /// <summary>
        /// Returns the hash algorithm implementation to use for generating the identifier.
        /// </summary>
        /// <param name="algorithm">Algorithm to return.</param>
        /// <param name="version">GUID version field.</param>
        
        public HashAlgorithm Create( in NamedGuidAlgorithm algorithm, out byte version )
        {
            switch ( algorithm )
            {
                case NamedGuidAlgorithm.MD5:
                    version = StandardGuidVersion.UUIDv3;
                    return MD5.Create();
                case NamedGuidAlgorithm.SHA1:
                    version = StandardGuidVersion.UUIDv5;
                    return SHA1.Create();
                default: throw new NotImplementedException();
            }
        }
    }
}