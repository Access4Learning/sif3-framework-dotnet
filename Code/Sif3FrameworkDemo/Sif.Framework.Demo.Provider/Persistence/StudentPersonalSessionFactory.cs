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

using NHibernate;
using NHibernate.Cfg;
using Sif.Framework.Persistence.NHibernate;

namespace Sif.Framework.Demo.Provider.Persistence
{

    class StudentPersonalSessionFactory : IBaseSessionFactory
    {
        private static StudentPersonalSessionFactory studentPersonalSessionFactory;

        private ISessionFactory SessionFactory { get; set; }

        private StudentPersonalSessionFactory()
        {

        }

        public static StudentPersonalSessionFactory Instance
        {

            get
            {

                if (studentPersonalSessionFactory == null)
                {
                    studentPersonalSessionFactory = new StudentPersonalSessionFactory();
                    string configurationFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Demo.cfg.xml");

                    if (configurationFilePath == null)
                    {
                        configurationFilePath = "Demo.cfg.xml";
                    }

                    studentPersonalSessionFactory.SessionFactory = new Configuration().Configure(configurationFilePath).BuildSessionFactory();
                }

                return studentPersonalSessionFactory;
            }

        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

    }

}
