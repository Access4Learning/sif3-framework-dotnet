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

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Sif.Framework.Demo.Provider.Models;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Provider.Tests
{

    /// <summary>
    /// This class is used to generate test data for unit testing.
    /// </summary>
    static class DataFactory
    {
        private static Random random = new Random();

        /// <summary>
        /// Generate the database schema, apply it to the database and save the DDL used into a file.
        /// </summary>
        public static void CreateDatabase()
        {
            Configuration configuration = new Configuration();
            configuration.Configure("StudentPersonal.cfg.xml");
            SchemaExport schemaExport = new SchemaExport(configuration);
            schemaExport.SetOutputFile("SQLite database schema.ddl");
            schemaExport.Create(true, true);
        }

        public static StudentPersonal CreateStudent()
        {
            Name name = new Name { Type = NameType.LGL, FamilyName = RandomNameGenerator.FamilyName, GivenName = RandomNameGenerator.GivenName };
            PersonInfo personInfo = new PersonInfo { Name = name };
            StudentPersonal studentPersonal = new StudentPersonal { LocalId = random.Next(10000, 99999).ToString(), PersonInfo = personInfo };

            return studentPersonal;
        }

        public static ICollection<StudentPersonal> CreateStudents(int count)
        {
            ICollection<StudentPersonal> students = new List<StudentPersonal>();

            for (int i = 1; i <= count; i++)
            {
                students.Add(CreateStudent());
            }

            return students;
        }

    }

}
