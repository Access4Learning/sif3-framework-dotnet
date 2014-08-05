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

using Sif.Framework.Demo.Provider.Models;
using Sif.Framework.Demo.Provider.Persistence;
using Sif.Framework.Demo.Setup.Utils;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence.NHibernate;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Setup
{

    class DatabaseCreator
    {

        static void Main(string[] args)
        {

            try
            {
                DatabaseManager frameworkDatabaseManager = new DatabaseManager("SifFramework.cfg.xml");
                frameworkDatabaseManager.CreateDatabaseTables();
                ApplicationRegister applicationRegister = DataFactory.CreateApplicationRegister();
                ApplicationRegisterRepository applicationRegisterRepository = new ApplicationRegisterRepository();
                applicationRegisterRepository.Save(applicationRegister);

                DatabaseManager demoDatabaseManager = new DatabaseManager("StudentPersonal.cfg.xml");
                demoDatabaseManager.CreateDatabaseTables();
                ICollection<StudentPersonal> students = DataFactory.CreateStudents(100);
                StudentPersonalRepository studentPersonalRepository = new StudentPersonalRepository();
                studentPersonalRepository.Save(students);
            }
            finally
            {
                System.Console.WriteLine("Press any key to continue ...");
                System.Console.ReadKey();
            }

        }

    }

}
