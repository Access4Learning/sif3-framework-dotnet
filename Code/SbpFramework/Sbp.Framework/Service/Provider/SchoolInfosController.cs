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

using Sbp.Framework.Model;
using Sif.Framework.Controller;
using Sif.Framework.Service;

namespace Sbp.Framework.Service.Provider
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class SchoolInfosController : GenericController<SchoolInfo, string>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public SchoolInfosController(IGenericService<SchoolInfo, string> service)
            : base(service)
        {

        }

    }

}
