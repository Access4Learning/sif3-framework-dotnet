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

using Sif.Framework.Demo.Au.Provider.Models;
using Sif.Framework.Service;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Au.Provider.Service
{

    /// <summary>
    /// 
    /// </summary>
    public class SchoolInfoService : IGenericService<SchoolInfo, string>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Create(IEnumerable<SchoolInfo> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Create(SchoolInfo obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Delete(IEnumerable<SchoolInfo> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Delete(SchoolInfo obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<SchoolInfo> Retrieve()
        {
            SchoolInfo school = new SchoolInfo { SchoolName = "Applecross SHS" };
            List<SchoolInfo> schools = new List<SchoolInfo> { school };
            return schools;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ICollection<SchoolInfo> Retrieve(SchoolInfo obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SchoolInfo Retrieve(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objs"></param>
        public void Update(IEnumerable<SchoolInfo> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Update(SchoolInfo obj)
        {
            throw new NotImplementedException();
        }

    }

}