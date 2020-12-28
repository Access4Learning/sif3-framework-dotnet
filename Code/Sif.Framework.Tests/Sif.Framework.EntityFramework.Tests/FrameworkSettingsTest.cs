/*
 * Copyright 2020 Systemic Pty Ltd
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

using Sif.Framework.Settings;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.Model.Requests;
using Sif.Framework.Model.Settings;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFramework.Configurations;
using Xunit;

namespace Sif.Framework.EntityFramework.Tests
{
    public class FrameworkSettingsTest
    {
        private readonly IFrameworkSettings settings;

        public FrameworkSettingsTest()
        {
            settings = new ConsumerSettings(
                new ApplicationConfiguration(new AppSettingsConfigurationSource("name=SettingsDb")));
        }

        [Fact]
        public void GetSettings_ValidSettings_Success()
        {
            Assert.Equal(Accept.XML, settings.Accept);
            Assert.Equal("Sif3DemoConsumer", settings.ApplicationKey);
            Assert.Equal("SIF_HMACSHA256", settings.AuthenticationMethod);
            Assert.False(settings.CompressPayload);
            Assert.Equal("DemoConsumer", settings.ConsumerName);
            Assert.Equal(ContentType.XML, settings.ContentType);
            Assert.Equal("http://www.sifassociation.org/datamodel/au/3.4", settings.DataModelNamespace);
            Assert.False(settings.DeleteOnUnregister);
            Assert.Equal(EnvironmentType.DIRECT, settings.EnvironmentType);
            Assert.Equal("http://localhost:62921/api/environments/environment", settings.EnvironmentUrl);
            Assert.Equal(10, settings.EventProcessingWaitTime);
            Assert.Equal("http://www.sifassociation.org/infrastructure/3.2.1", settings.InfrastructureNamespace);
            Assert.Null(settings.InstanceId);
            Assert.True(settings.JobBinding);
            Assert.Equal("any", settings.JobClasses);
            Assert.True(settings.JobTimeoutEnabled);
            Assert.Equal(60, settings.JobTimeoutFrequency);
            Assert.Equal(5, settings.NavigationPageSize);
            Assert.Equal("SecretDem0", settings.SharedSecret);
            Assert.Equal("Sif3Framework", settings.SolutionId);
            Assert.Equal(10, settings.StartupDelay);
            Assert.Equal("3.2.1", settings.SupportedInfrastructureVersion);
            Assert.Null(settings.UserToken);
        }
    }
}