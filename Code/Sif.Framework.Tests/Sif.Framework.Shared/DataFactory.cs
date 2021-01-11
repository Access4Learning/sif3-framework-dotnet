/*
 * Copyright 2021 Systemic Pty Ltd
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

using Bogus;
using Sif.Framework.Model.Sessions;
using System.Collections.Generic;
using System.Globalization;

namespace Sif.Framework.Shared
{
    public class DataFactory
    {
        private const char LowercaseZ = '\x7a';
        private const char Zero = '\x30';

        private static readonly string[] SifType = {"Consumer", "Provider"};

        private static readonly Faker<Session> FakeSessions = new Faker<Session>()
            .RuleFor(
                u => u.ApplicationKey,
                f => $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.Lorem.Word())}{f.PickRandom(SifType)}")
            .RuleFor(
                u => u.EnvironmentUrl,
                f => $"{f.Internet.Protocol()}://{f.Internet.DomainName()}/api/environments/{f.Random.Guid()}")
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.QueueId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.SessionToken, f => f.Random.String(44, Zero, LowercaseZ))
            .RuleFor(u => u.SolutionId, f => f.Lorem.Word())
            .RuleFor(u => u.SubscriptionId, f => f.Random.Guid().ToString());

        public static Session CreateSession() => FakeSessions.Generate();

        public static IList<Session> CreateSessions(uint count) => FakeSessions.Generate((int) count);
    }
}