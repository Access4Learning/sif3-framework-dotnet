/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Models.Exceptions;
using Sif.Framework.Models.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Sif.Framework.Utils
{
    /// <summary>
    /// Utility methods for Rights operations.
    /// </summary>
    public static class RightsUtils
    {
        public static readonly Right CreateApprovedRight =
            new Right { Type = RightType.CREATE.ToString(), Value = RightValue.APPROVED.ToString() };

        public static readonly Right DeleteApprovedRight =
            new Right { Type = RightType.DELETE.ToString(), Value = RightValue.APPROVED.ToString() };

        public static readonly Right QueryApprovedRight =
            new Right { Type = RightType.QUERY.ToString(), Value = RightValue.APPROVED.ToString() };

        public static readonly Right UpdateApprovedRight =
            new Right { Type = RightType.UPDATE.ToString(), Value = RightValue.APPROVED.ToString() };

        /// <summary>
        /// Checks to see if the given Right is contained in the collection of Rights. Throws a RejectedException if
        /// the right is not found.
        /// </summary>
        /// <param name="rights">The collection of rights to check.</param>
        /// <param name="right">The right to look for.</param>
        public static void CheckRight(ICollection<Right> rights, Right right)
        {
            bool exists = rights.FirstOrDefault(r => r.Type == right.Type && r.Value == right.Value) != null;

            if (!exists)
            {
                throw new RejectedException(
                    $"Insufficient rights for this operation, no right for {right.Type} given in the rights collection.");
            }
        }
    }
}