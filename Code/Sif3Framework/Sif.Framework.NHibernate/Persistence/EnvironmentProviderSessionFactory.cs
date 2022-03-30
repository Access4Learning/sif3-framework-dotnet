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

using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Configuration;
using System.IO;
using NHibernateConfiguration = NHibernate.Cfg.Configuration;

namespace Sif.Framework.NHibernate.Persistence
{
    /// <summary>
    /// A Singleton helper class for managing NHibernate sessions based upon the default configuration file.
    /// </summary>
    internal class EnvironmentProviderSessionFactory : IBaseSessionFactory
    {
        private static EnvironmentProviderSessionFactory _sessionFactory;

        private ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        public static EnvironmentProviderSessionFactory Instance
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = new EnvironmentProviderSessionFactory();
                    string configFilePath;
#if NETFULL
                    configFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SifFramework.cfg.xml");

                    if (configFilePath == null)
                    {
                        configFilePath = "SifFramework.cfg.xml";
                    }
#else
                    var map = new ExeConfigurationFileMap
                    {
                        ExeConfigFilename = $"{AppDomain.CurrentDomain.BaseDirectory}Web.Config"
                    };

                    Configuration systemConfig =
                        ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                    if (systemConfig.HasFile && !string.IsNullOrWhiteSpace(systemConfig.FilePath))
                    {
                        configFilePath = $"{Path.GetDirectoryName(systemConfig.FilePath)}/SifFramework.cfg.xml";
                    }
                    else
                    {
                        configFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}SifFramework.cfg.xml";
                    }
#endif

                    NHibernateConfiguration nHibernateConfig = new NHibernateConfiguration().Configure(configFilePath);
                    SchemaMetadataUpdater.QuoteTableAndColumns(nHibernateConfig);
                    _sessionFactory.SessionFactory = nHibernateConfig.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        /// <summary>
        /// Private constructor to ensure instantiation as a Singleton.
        /// </summary>
        private EnvironmentProviderSessionFactory()
        {
        }

        /// <summary>
        /// Open an NHibernate database session.
        /// </summary>
        /// <returns>NHibernate database session.</returns>
        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}