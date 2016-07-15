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

using Sif.Framework.Demo.Uk.Provider.Actions;
using Sif.Framework.Model;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Service.Functional;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Uk.Provider.Services
{

    public class PayloadService : FunctionalService
    {
        public PayloadService() : base()
        {
            phaseActions.Add("default", new DefaultActions());
            phaseActions.Add("xml", new XmlActions());
            phaseActions.Add("json", new JsonActions());
        }

        public override string GetServiceName()
        {
            return "Payloads";
        }

        protected override void Configure(Job job)
        {
            job.AddPhase(new Phase("default", true, RightsUtils.getRights(create: RightValue.APPROVED, query: RightValue.APPROVED, update: RightValue.APPROVED), RightsUtils.getRights(create: RightValue.APPROVED), PhaseStateType.NOTSTARTED));

            job.AddPhase(new Phase("xml", true, RightsUtils.getRights(update: RightValue.APPROVED), RightsUtils.getRights(create: RightValue.APPROVED), PhaseStateType.NOTSTARTED));

            job.AddPhase(new Phase("json", true, RightsUtils.getRights(update: RightValue.APPROVED), RightsUtils.getRights(create: RightValue.APPROVED), PhaseStateType.NOTSTARTED));

            job.Timeout = new TimeSpan(0, 1, 0);
        }

        protected override void JobShutdown(Job job)
        {
            // Throw an exception to prevent job deletion
            base.JobShutdown(job);
        }
    }
}
