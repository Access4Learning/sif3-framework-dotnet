/*
 * Copyright 2015 Systemic Pty Ltd
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

namespace Sif.Framework.Consumer
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="PK"></typeparam>
    public interface IGenericConsumer<T, PK> where T : IPersistable<PK>
    {

        /// <summary>
        /// This method must be called before any other.
        /// </summary>
        void Register();

        /// <summary>
        /// This method should be called on completion.
        /// </summary>
        /// <param name="deleteOnUnregister">True to remove session data on unregister; false to leave session data.</param>
        void Unregister(bool? deleteOnUnregister = null);

        /// <summary>
        /// POST /StudentPersonals/StudentPersonal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        PK Create(T obj);

        /// <summary>
        /// POST /StudentPersonals
        /// </summary>
        /// <param name="objs"></param>
        void Create(IEnumerable<T> objs);

        /// <summary>
        /// DELETE /StudentPersonals/{id}
        /// </summary>
        /// <param name="id"></param>
        void Delete(PK id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        void Delete(IEnumerable<T> objs);

        /// <summary>
        /// GET /StudentPersonals/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Retrieve(PK id);

        /// <summary>
        /// GET /StudentPersonals
        /// </summary>
        /// <returns></returns>
        ICollection<T> Retrieve();

        /// <summary>
        /// PUT /StudentPersonals/{id}
        /// </summary>
        /// <param name="obj"></param>
        void Update(T obj);

        /// <summary>
        /// PUT /StudentPersonals
        /// </summary>
        /// <param name="objs"></param>
        void Update(IEnumerable<T> objs);

    }

}
