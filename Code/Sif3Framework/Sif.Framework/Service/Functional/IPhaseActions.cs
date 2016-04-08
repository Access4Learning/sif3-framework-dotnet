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

using Sif.Framework.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Service.Functional
{
    public interface IPhaseActions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="phase"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.CreateException" />
        string Create(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.NotFoundException" />
        string Retrieve(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="phase"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.UpdateException" />
        string Update(Job job, Phase phase, string body = null, string contentType = null, string accept = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="phase"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <exception cref="Sif.Framework.Model.Exceptions.DeleteException" />
        string Delete(Job job, Phase phase, string body = null, string contentType = null, string accept = null);
    }
}
