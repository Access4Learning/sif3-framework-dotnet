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
using System;

namespace Sif.Framework.Demo.Setup.Utils
{

    /// <summary>
    /// Class used to generate the demo database.
    /// </summary>
    class DatabaseManager
    {
        private Configuration configuration;

        /// <summary>
        /// Instantiate this class.
        /// </summary>
        /// <param name="configurationFileName">NHibernate configuration file.</param>
        /// <exception cref="System.ArgumentNullException">NHibernate configuration file name is null or empty.</exception>
        public DatabaseManager(string configurationFileName)
        {

            if (string.IsNullOrWhiteSpace(configurationFileName))
            {
                throw new ArgumentNullException("configurationFileName");
            }

            configuration = new Configuration();
            configuration.Configure(configurationFileName);
        }

        /// <summary>
        /// Generate the database schema, apply it to the database and save the DDL used into a file.
        /// </summary>
        /// <param name="schemaOutputFileName"></param>
        public void CreateDatabaseTables(string schemaOutputFileName = null)
        {
            SchemaExport schemaExport = new SchemaExport(configuration);
            schemaExport.SetOutputFile(string.IsNullOrWhiteSpace(schemaOutputFileName) ? "Create database schema.ddl" : schemaOutputFileName);
            schemaExport.Create(true, true);
        }

        /// <summary>
        /// Drop the database schema, apply it to the database and save the DDL used into a file.
        /// </summary>
        /// <param name="schemaOutputFileName"></param>
        public void DropDatabaseTables(string schemaOutputFileName = null)
        {
            SchemaExport schemaExport = new SchemaExport(configuration);
            schemaExport.SetOutputFile(string.IsNullOrWhiteSpace(schemaOutputFileName) ? "Drop database schema.ddl" : schemaOutputFileName);
            schemaExport.Drop(true, true);
        }

    }

}
