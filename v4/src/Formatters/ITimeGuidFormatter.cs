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

namespace Identifiable.Formatters
{
    /// <summary>
    /// Defines a formatter for time-based GUIDs.
    /// </summary>

    public interface ITimeGuidFormatter
    {
        /// <summary>
        /// Creates and returns a time-based identifier.
        /// </summary>
        /// <param name="time">
        /// Number of 100 nanosecond intervals since 00:00:00.00, 15 October 1582.
        /// The most-significant nibble (4 bits) will be overwritten by the identifier version, 
        /// causing a roll-over of values once every approximately two-thousand years.
        /// </param>
        /// <param name="clock">
        /// Value whose least-significant 14 bits will be used as the clock sequence.
        /// According to spec, this value should be initialized to a random number for each node used.
        /// </param>
        /// <param name="node">
        /// 48-bit node identifier.
        /// If set to a random value, the mutlicast bit must be set.
        /// </param>
        
        Guid Create( long time, short clock, byte[] node );
    }
}