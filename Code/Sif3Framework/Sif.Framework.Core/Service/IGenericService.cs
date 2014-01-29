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

using Sif.Framework.Model.Persistence;
using System.Collections.Generic;

namespace Sif.Framework.Service
{

    public interface IGenericService<UI, DB> where DB : IPersistable
    {

        long Create(UI item);

        void Create(IEnumerable<UI> items);

        void Delete(UI item);

        void Delete(IEnumerable<UI> items);

        UI Retrieve(long id);

        IEnumerable<UI> Retrieve(UI item);

        IEnumerable<UI> Retrieve();

        void Update(UI item);

        void Update(IEnumerable<UI> items);

    }

}
