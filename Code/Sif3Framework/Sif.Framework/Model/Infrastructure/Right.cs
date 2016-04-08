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
using System.Collections.Generic;

namespace Sif.Framework.Model.Infrastructure
{

    public class Right : IPersistable<long>
    {

        public virtual long Id { get; set; }

        public virtual string Type { get; set; }

        public virtual string Value { get; set; }

        public Right() { }

        public Right(RightType type, RightValue value)
        {
            
            Type = type.ToString();
            Value = value.ToString();
        }

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
