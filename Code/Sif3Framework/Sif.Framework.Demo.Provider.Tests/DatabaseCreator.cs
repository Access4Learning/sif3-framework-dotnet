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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sif.Framework.Demo.Provider.Models;
using Sif.Framework.Demo.Provider.Persistence;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Provider.Tests
{

    [TestClass]
    public class DatabaseCreator
    {

        private static void PopulateStudents()
        {
            (new StudentPersonalRepository(StudentPersonalSessionFactory.Instance)).Save(DataFactory.CreateStudents(100));
        }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            DataFactory.CreateDatabase();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }

        [TestMethod]
        public void CreateStudent()
        {
            StudentPersonal studentPersonal = DataFactory.CreateStudent();
            string xmlString;
            SerialisationUtils.XmlSerialise<StudentPersonal>(studentPersonal, out xmlString);
            Console.WriteLine(xmlString);
        }

        [TestMethod]
        public void PopulateDatabase()
        {
            PopulateStudents();
        }

        [TestMethod]
        public void SaveAndRetrieve()
        {
            StudentPersonal saved = DataFactory.CreateStudent();
            Guid environmentId = (new StudentPersonalRepository(StudentPersonalSessionFactory.Instance)).Save(saved);
            StudentPersonal retrieved = (new StudentPersonalRepository(StudentPersonalSessionFactory.Instance)).Retrieve(environmentId);
            Assert.AreEqual(saved.Id, retrieved.Id);
            Assert.AreEqual(saved.LocalId, retrieved.LocalId);
            Assert.AreEqual(saved.PersonInfo.Id, retrieved.PersonInfo.Id);
            Assert.AreEqual(saved.PersonInfo.Name.FamilyName, retrieved.PersonInfo.Name.FamilyName);
            Assert.AreEqual(saved.PersonInfo.Name.GivenName, retrieved.PersonInfo.Name.GivenName);
            Assert.AreEqual(saved.PersonInfo.Name.Id, retrieved.PersonInfo.Name.Id);
            Assert.AreEqual(saved.PersonInfo.Name.Type, retrieved.PersonInfo.Name.Type);
        }

        [TestMethod]
        public void SaveAndRetrieveMany()
        {
            ICollection<StudentPersonal> saved = DataFactory.CreateStudents(100);
            (new StudentPersonalRepository(StudentPersonalSessionFactory.Instance)).Save(saved);
            ICollection<StudentPersonal> retrieved = (ICollection<StudentPersonal>)(new StudentPersonalRepository(StudentPersonalSessionFactory.Instance)).Retrieve();
        }

    }

}
