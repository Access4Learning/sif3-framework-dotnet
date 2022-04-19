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

namespace Sif.Framework.Model.Infrastructure
{
    /// <summary>
    /// Job initialisation object.
    /// TODO Currently not supported in the ASP.NET Core version of the SIF Framework.
    /// </summary>
    public class Initialization
    {
        /// <summary>
        /// Payload.
        /// </summary>
        public virtual object Payload { get; set; }

        /// <summary>
        /// Phase name.
        /// </summary>
        public virtual string PhaseName { get; set; }
    }
}