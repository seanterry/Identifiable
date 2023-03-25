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

namespace Identifiable.Factories
{
    /// <summary>
    /// Defines a factory for creating named GUIDs.
    /// </summary>

    public interface INamedGuidFactory
    {
        /// <summary>
        /// Creates and return a name-based GUID that is not compatible with RFC 4122.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        [Obsolete( "Consider using Compute method instead." )]
        Guid Create( in NamedGuidAlgorithm algorithm, in Guid @namespace, string name );

        /// <summary>
        /// Computes and return a name-based GUID using the given algorithm as defined in https://tools.ietf.org/html/rfc4122#section-4.3.
        /// </summary>
        /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
        /// <param name="namespace">Name space identifier.</param>
        /// <param name="name">Name for which to create a GUID.</param>

        Guid Compute( in NamedGuidAlgorithm algorithm, in Guid @namespace, string name );
    }
}
