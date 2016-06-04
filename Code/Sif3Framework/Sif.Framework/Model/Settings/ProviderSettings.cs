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
                string setting = GetStringSetting(SettingsPrefix + ".provider.classes", "any");
                if(StringUtils.IsEmpty(setting))
                {
                    return new Type[0];
                }

                if (setting.ToLower().Equals("any")) {
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where ProviderUtils.isController(type)
                     select type).ToArray();
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
                return providers.ToArray();
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
