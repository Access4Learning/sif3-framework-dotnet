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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
