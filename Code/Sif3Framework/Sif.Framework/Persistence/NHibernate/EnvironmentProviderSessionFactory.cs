/*
 * Copyright 2018 Systemic Pty Ltd
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
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Sif.Framework.Persistence.NHibernate
{
    /// <summary>
    /// A Singleton helper class for managing NHibernate sessions based upon the default configuration file.
    /// </summary>
    internal class EnvironmentProviderSessionFactory : IBaseSessionFactory
    {
        private static EnvironmentProviderSessionFactory environmentProviderSessionFactory;

        private ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// Singleton instance of this class.
        /// </summary>
        public static EnvironmentProviderSessionFactory Instance
        {
            get
            {
                if (environmentProviderSessionFactory == null)
                {
                    environmentProviderSessionFactory = new EnvironmentProviderSessionFactory();
                    string configurationFilePath;
#if NETFULL
                    configurationFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/SifFramework.cfg.xml");

                    if (configurationFilePath == null)
                    {
                        configurationFilePath = "SifFramework.cfg.xml";
                    }
#else
                    System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

                    if (config.HasFile && !string.IsNullOrWhiteSpace(config.FilePath))
                    {
                        configurationFilePath = $"{System.IO.Path.GetDirectoryName(config.FilePath)}/SifFramework.cfg.xml";
                    }
                    else
                    {
                        configurationFilePath = "SifFramework.cfg.xml";
                    }
#endif

                    Configuration configuration = new Configuration().Configure(configurationFilePath);
                    SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
                    environmentProviderSessionFactory.SessionFactory = configuration.BuildSessionFactory();
                }

                return environmentProviderSessionFactory;
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