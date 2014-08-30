/*
 * Copyright 2014 Systemic Pty Ltd
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

using Sif.Framework.Model.Persistence;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Model.Infrastructure
{

    public class Service : IPersistable<long>, IComparable<Service>
    {

        public virtual long Id { get; set; }

        /// <summary>
        /// The unique identity of a context element, which is associated with a Provider of this name and type
        /// operating in a Zone with this ID.  All Services with the same name in the same Zone must have different
        /// Context IDs. Only one such Service can have no Context.
        /// </summary>
        public virtual string ContextId { get; set; }

        /// <summary>
        /// The name of the Service. For utilities, this is fixed to one of the defined set of Utility Service Names.
        /// For objects and functions, it is defined by the Data Model.
        /// </summary>
        public virtual string Name { get; set; }

        public virtual IDictionary<string, Right> Rights { get; set; }

        public virtual string Type { get; set; }

        public virtual int CompareTo(Service service)
        {
            int comparison = 0;

            if (service == null)
            {
                comparison = 1;
            }
            else if (!service.Equals(this))
            {
                comparison = string.Compare(this.Name, service.Name);

                if (comparison == 0)
                {
                    comparison = string.Compare(this.ContextId, service.ContextId);
                }

            }

            return comparison;
        }

    }

}
