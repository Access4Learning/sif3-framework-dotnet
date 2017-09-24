/*
 * Copyright 2017 Systemic Pty Ltd
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

using Sif.Framework.Demo.Broker.Models;
using Sif.Framework.Demo.Broker.Services;
using Sif.Framework.Providers;
using Sif.Framework.Service.Providers;
using Sif.Framework.WebApi.ModelBinders;
using System.Collections.Generic;
using System.Web.Http;

namespace Sif.Framework.Demo.Broker.Controllers
{

    public class SubscriptionsProvider : BasicProvider<Subscription>
    {

        protected SubscriptionsProvider() : base(new SubscriptionService())
        {
        }

        protected SubscriptionsProvider(IBasicProviderService<Subscription> service) : base(service)
        {
        }

        [NonAction]
        public override IHttpActionResult BroadcastEvents(string zoneId = null, string contextId = null)
        {
            return base.BroadcastEvents(zoneId, contextId);
        }

        [NonAction]
        public override IHttpActionResult Post(List<Subscription> objs, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(objs, zoneId, contextId);
        }

        public override IHttpActionResult Post(Subscription obj, [MatrixParameter] string[] zoneId = null, [MatrixParameter] string[] contextId = null)
        {
            return base.Post(obj, zoneId, contextId);
        }

    }

}