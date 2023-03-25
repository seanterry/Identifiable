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

namespace Identifiable
{
    /// <summary>
    /// Enumeration of named GUID algorithms.
    /// </summary>

    public enum NamedGuidAlgorithm
    {
        /// <summary>
        /// Generates a name-based GUID using the MD5 algorithm.
        /// </summary>
        
        MD5 = 3,

        /// <summary>
        /// Generates a name-based GUID using the SHA-1 algorithm (preferred).
        /// </summary>
        
        SHA1 = 5,
    }
}