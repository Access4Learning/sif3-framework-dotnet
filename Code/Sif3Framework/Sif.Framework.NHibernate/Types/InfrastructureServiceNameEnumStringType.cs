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

using NHibernate.Type;
using Sif.Framework.Model.Infrastructure;

namespace Sif.Framework.NHibernate.Types
{
    /// <summary>
    /// <a href="https://blog.devart.com/string-enum-representation-in-entity-developer.html">String Enum Representation in Entity Developer</a>
    /// </summary>
    public class InfrastructureServiceNameEnumStringType : EnumStringType
    {
        public InfrastructureServiceNameEnumStringType() : base(typeof(InfrastructureServiceNames))
        {
        }
    }
}