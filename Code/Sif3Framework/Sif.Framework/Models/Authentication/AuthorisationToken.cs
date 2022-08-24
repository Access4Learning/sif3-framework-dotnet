﻿/*
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

namespace Sif.Framework.Models.Authentication
{
    /// <summary>
    /// Authorisation token used for application authentication.
    /// </summary>
    public class AuthorisationToken
    {
        /// <summary>
        /// Authorisation token value.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Timestamp associated with the authorisation token.
        /// </summary>
        public string Timestamp { get; set; }
    }
}