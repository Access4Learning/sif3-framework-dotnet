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

using Sif.Framework.Service.Functional;
using Sif.Framework.Model.Infrastructure;

namespace Sif.Framework.Demo.Uk.Provider.Actions
{
    class DefaultActions : PhaseActions
    {
        public override string Create(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            job.UpdateState(JobStateType.INPROGRESS, "CREATE to " + phase.Name);
            job.UpdatePhaseState(phase.Name, PhaseStateType.INPROGRESS, "CREATE");
            return "Got CREATE message for " + phase.Name + "@" + job.Id + " with content type " + contentType + " and accept " + accept + ".\nBODY START\n" + body + ".\nBODY END.";
        }

        public override string Retrieve(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            job.UpdateState(JobStateType.INPROGRESS, "RETRIEVE to " + phase.Name);
            job.UpdatePhaseState(phase.Name, PhaseStateType.INPROGRESS, "RETRIEVE");
            return "Got RETRIEVE message for " + phase.Name + "@" + job.Id + " with content type " + contentType + " and accept " + accept + ".\nBODY START\n" + body + ".\nBODY END.";
        }

        public override string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            job.UpdateState(JobStateType.INPROGRESS, "UPDATE to " + phase.Name);
            job.UpdatePhaseState(phase.Name, PhaseStateType.COMPLETED, "UPDATE");
            return "Got UPDATE message for " + phase.Name + "@" + job.Id + " with content type " + contentType + " and accept " + accept + ".\nBODY START\n" + body + ".\nBODY END.";
        }

        public override string Delete(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            job.UpdateState(JobStateType.INPROGRESS, "DELETE to " + phase.Name);
            job.UpdatePhaseState(phase.Name, PhaseStateType.COMPLETED, "DELETE");
            return "Got DELETE message for " + phase.Name + "@" + job.Id + " with content type " + contentType + " and accept " + accept + ".\nBODY START\n" + body + ".\nBODY END.";
        }
    }
}
