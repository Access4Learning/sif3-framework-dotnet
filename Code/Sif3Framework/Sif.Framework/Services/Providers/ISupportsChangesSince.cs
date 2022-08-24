/*
 * Copyright 2022 Systemic Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Sif.Framework.Services.Providers
{
    /// <summary>
    /// This interface defines operations associated with the "Changes Since" mechanism.
    /// </summary>
    public interface ISupportsChangesSince
    {
        /// <summary>
        /// Get the current Changes Since marker.
        /// </summary>
        /// <returns>Changes Since marker.</returns>
        string ChangesSinceMarker { get; }

        /// <summary>
        /// Increment the current Changes Since marker and return. This value then becomes the current Changes Since
        /// marker.
        /// </summary>
        /// <returns>Next Changes Since marker.</returns>
        string NextChangesSinceMarker { get; }
    }
}