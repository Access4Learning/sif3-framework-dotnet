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

using Sif.Framework.Demo.Setup.Utils;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Persistence.NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Sif.Framework.Demo.Setup
{

    /// <summary>
    /// 
    /// </summary>
    class DatabaseCreator
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            try
            {
                string prop = null;

                if (args.Length == 1)
                {
                    prop = args[0];
                }
                else
                {
                    prop = ConfigurationManager.AppSettings["demo.locale"];
                }

                string locale = (prop != null && ("AU".Equals(prop.ToUpper()) || "UK".Equals(prop.ToUpper()) || "US".Equals(prop.ToUpper())) ? prop.ToUpper() : null);

                if (locale == null)
                {
                    Console.WriteLine("To execute, setup requires a parameter which specifies locale, i.e. AU, UK or US.");
                }
                else
                {
                    Console.WriteLine("Configuring the demonstration for the " + locale + " locale.");
                    DatabaseManager frameworkDatabaseManager = new DatabaseManager("SifFramework.cfg.xml");
                    frameworkDatabaseManager.CreateDatabaseTables("SifFramework schema.ddl");
                    ICollection<ApplicationRegister> applicationRegisters = DataFactory.CreateApplicationRegisters(locale);
                    ApplicationRegisterRepository applicationRegisterRepository = new ApplicationRegisterRepository();
                    applicationRegisterRepository.Save(applicationRegisters);
                }

            }
            finally
            {
                Console.WriteLine("Press any key to continue ...");
                Console.ReadKey();
            }

        }

    }

}
