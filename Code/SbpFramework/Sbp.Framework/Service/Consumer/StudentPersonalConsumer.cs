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
using Sif.Framework.Consumer;
using Sif.Framework.Model.Infrastructure;

namespace Sbp.Framework.Service.Consumer
{

    /// <summary>
    /// 
    /// </summary>
    public class StudentPersonalConsumer : GenericConsumer<StudentPersonal, string>, IStudentPersonalConsumer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationKey"></param>
        /// <param name="instanceId"></param>
        /// <param name="userToken"></param>
        public StudentPersonalConsumer(string applicationKey, string instanceId = null, string userToken = null)
            : base(applicationKey, instanceId, userToken)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        public StudentPersonalConsumer(Environment environment)
            : base(environment)
        {

        }

    }

}
