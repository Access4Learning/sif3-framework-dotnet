/*
 * Copyright 2020 Systemic Pty Ltd
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

using Sif.Framework.Consumers;
using Sif.Framework.Demo.Au.Consumer.Models;
using Sif.Framework.Model.Settings;

namespace Sif.Framework.Demo.Au.Consumer.Consumers
{
    internal class StudentPersonalConsumer : BasicConsumer<StudentPersonal>
    {
        public StudentPersonalConsumer(
            Model.Infrastructure.Environment environment,
            IFrameworkSettings settings = null)
            : base(environment, settings)
        {
        }

        public StudentPersonalConsumer(
            string applicationKey,
            string instanceId = null,
            string userToken = null,
            string solutionId = null,
            IFrameworkSettings settings = null)
            : base(applicationKey, instanceId, userToken, solutionId, settings)
        {
        }
    }
}