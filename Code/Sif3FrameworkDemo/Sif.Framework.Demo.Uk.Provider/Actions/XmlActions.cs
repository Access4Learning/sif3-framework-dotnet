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
using System;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Demo.Uk.Provider.Models;
using Sif.Specification.DataModel.Uk;
using Sif.Framework.Service.Serialisation;

namespace Sif.Framework.Demo.Uk.Provider.Actions
{
    class XmlActions : PhaseActions
    {
        public override string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null)
        {
            job.UpdateState(JobStateType.INPROGRESS, "UPDATE to " + phase.Name);
            string response;
            if (!contentType.ToLower().Equals("application/xml"))
            {
                response = "Invalid Content-Type, expecting application/xml";
                job.UpdatePhaseState(phase.Name, PhaseStateType.FAILED, response);
                throw new RejectedException(response);
            }

            LearnerPersonal data;
            try {
                data = SerialiserFactory.GetXmlSerialiser<LearnerPersonal>().Deserialise(body);
            } catch(Exception e)
            {
                response = "Error decoding xml data: " + e.Message;
                job.UpdatePhaseState(phase.Name, PhaseStateType.FAILED, response);
                throw new RejectedException(response, e);
            }

            NameType name = data.PersonalInformation.Name;
            job.UpdatePhaseState(phase.Name, PhaseStateType.COMPLETED, "UPDATE");
            response = "Got UPDATE message for " + phase.Name + "@" + job.Id + " with content type " + contentType + " and accept " + accept + ".\nGot record for learner:" + name.GivenName + " " + name.FamilyName;
            return response;
        }
    }
}
