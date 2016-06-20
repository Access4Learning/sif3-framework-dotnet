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

using log4net;
using Sif.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sif.Framework.Model.Settings
{

    /// <summary>
    /// This class represents Provider settings that are stored in the SifFramework.config custom configuration file.
    /// </summary>
    class ProviderSettings : ConfigFileBasedFrameworkSettings
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// <see cref="Sif.Framework.Model.Settings.ConfigFileBasedFrameworkSettings.SettingsPrefix"/>
        /// </summary>
        protected override string SettingsPrefix { get { return "provider"; } }

        protected Type[] classes = null;

        /// <summary>
        /// <see cref="Sif.Framework.Model.Settings.ConfigFileBasedFrameworkSettings.ConfigFileBasedFrameworkSettings()"/>
        /// </summary>
        public ProviderSettings() : base()
        {
        }

        public Type[] Classes
        {
            get
            {
                if(classes != null)
                {
                    return classes;
                }

                string setting = GetStringSetting(SettingsPrefix + ".provider.classes", "any");

                log.Debug("Attempting to load named providers: " + setting);

                if(StringUtils.IsEmpty(setting))
                {
                    classes = new Type[0];
                    return classes;
                }

                if (setting.ToLower().Equals("any")) {
                    classes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where ProviderUtils.isFunctionalService(type)
                     select type).ToArray();
                    return classes;
                }

                List<Type> providers = new List<Type>();
                string[] classNames = setting.Split(',');
                foreach(string className in classNames)
                {
                    Type provider = Type.GetType(className);
                    if(provider == null)
                    {
                        log.Error("Could not find provider with assembly qualified name " + className);
                    } else
                    {
                        providers.Add(provider);
                    }
                }
                classes = providers.ToArray();
                return classes;
            }
        }

        public int StartupDelay
        {
            get
            {
                return GetIntegerSetting(SettingsPrefix + ".startup.delay", 10);
            }
        }
    }
}
