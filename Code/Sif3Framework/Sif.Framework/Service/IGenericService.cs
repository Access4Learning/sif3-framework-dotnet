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

using Sif.Framework.Model.Query;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.Service
{
    public interface IGenericService<TEntity, TKey> where TEntity : IHasUniqueIdentifier<TKey>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TKey Create(TEntity obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="objs"></param>
        void Create(IEnumerable<TEntity> objs);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void Delete(TKey id);

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        void Delete(TEntity obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="objs"></param>
        void Delete(IEnumerable<TEntity> objs);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Retrieve(TKey id);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ICollection<TEntity> Retrieve();

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ICollection<TEntity> Retrieve(TEntity obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        ICollection<TEntity> Retrieve(int pageIndex, int pageSize);

        /// <summary>
        ///
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        ICollection<TEntity> Retrieve(IEnumerable<EqualCondition> conditions);

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        void Update(TEntity obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="objs"></param>
        void Update(IEnumerable<TEntity> objs);
    }
}