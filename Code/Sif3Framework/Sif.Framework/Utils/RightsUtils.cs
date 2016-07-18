/*
 * Crown Copyright © Department for Education (UK) 2016
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

using Sif.Framework.Model.Exceptions;
using Sif.Framework.Model.Infrastructure;
using System.Collections.Generic;

namespace Sif.Framework.Utils
{
    /// <summary>
    /// Utility methods for Rights operations.
    /// </summary>
    public static class RightsUtils
    {
        /// <summary>
        /// Checks to see if the given Right is contained in the dictionary of Rights. Throws a RejectedException if the right is not found.
        /// </summary>
        /// <param name="rights">The dictionary of rights to check</param>
        /// <param name="right">The right to look for</param>
        public static void CheckRight(IDictionary<string, Right> rights, Right right)
        {
            if (!rights.ContainsKey(right.Type))
            {
                throw new RejectedException("Insufficient rights for this operation, no right for " + right.Type + " given in the rights collection");
            }
            Right r = rights[right.Type];
            if (r == null || !r.Value.Equals(right.Value))
            {
                throw new RejectedException("Insufficient rights for this operation");
            }
        }

        /// <summary>
        /// Gets a dictionary of rights. If no arguments are supplied all rights are assumed to have the value REJECTED.
        /// </summary>
        /// <param name="admin">The value of the ADMIN right</param>
        /// <param name="create">The value of the CREATE right</param>
        /// <param name="delete">The value of the DELETE right</param>
        /// <param name="provide">The value of the PROVIDE right</param>
        /// <param name="query">The value of the QUERY right</param>
        /// <param name="subscribe">The value of the SUBSCRIBE right</param>
        /// <param name="update">The value of the UPDATE right</param>
        /// <returns>A dictionary of rights.</returns>
        public static IDictionary<string, Right> getRights(RightValue admin = RightValue.REJECTED, RightValue create = RightValue.REJECTED, RightValue delete = RightValue.REJECTED, RightValue provide = RightValue.REJECTED, RightValue query = RightValue.REJECTED, RightValue subscribe = RightValue.REJECTED, RightValue update = RightValue.REJECTED)
        {
            IDictionary<string, Right> rights = new Dictionary<string, Right>();
            rights.Add(RightType.ADMIN.ToString(), new Right(RightType.ADMIN, admin));
            rights.Add(RightType.CREATE.ToString(), new Right(RightType.CREATE, create));
            rights.Add(RightType.DELETE.ToString(), new Right(RightType.DELETE, delete));
            rights.Add(RightType.PROVIDE.ToString(), new Right(RightType.PROVIDE, provide));
            rights.Add(RightType.QUERY.ToString(), new Right(RightType.QUERY, query));
            rights.Add(RightType.SUBSCRIBE.ToString(), new Right(RightType.SUBSCRIBE, subscribe));
            rights.Add(RightType.UPDATE.ToString(), new Right(RightType.UPDATE, update));
            return rights;
        }
    }
}
