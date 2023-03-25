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
    /// Version fields values for standard variant GUIDs.
    /// </summary>

    public static class StandardGuidVersion
    {
        /// <summary>
        /// Version 1, based on system time.
        /// </summary>
        
        public const byte UUIDv1 = 0x10;

        /// <summary>
        /// Version 3, based on an MD5 hash.
        /// </summary>
        
        public const byte UUIDv3 = 0x30;

        /// <summary>
        /// Version 5, based on an SHA1 hash.
        /// </summary>
        
        public const byte UUIDv5 = 0x50;
    }
}