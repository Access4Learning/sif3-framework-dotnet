using Sif.Framework.Controllers;
using Sif.Framework.Demo.Uk.Provider.Services;
using Sif.Framework.Model.Infrastructure;
using Sif.Framework.WebApi.ModelBinders;
using System;
using System.Web.Http;
using Sif.Specification.Infrastructure;
using System.Net.Http;
using System.Collections.Generic;

namespace Sif.Framework.Demo.Uk.Provider.Controllers
{
    /*
    [RoutePrefix("api/Payloads")]
    public class PayloadsProvider : JobsController<PayloadService>
    {
        public PayloadsProvider() : base(new PayloadService()) {
        }

        [HttpPost]
        [Route("Payload")]
        public override HttpResponseMessage Post(jobType item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Post(item, zone, context);
        }

        [HttpGet]
        [Route("")]
        public override ICollection<jobType> Get([MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Get(zone, context);
        }

        [HttpGet]
        [Route("{id}")]
        public override jobType Get(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Get(id, zone, context);
        }

        [HttpPut]
        [Route("{id}")]
        public override void Put(Guid id, jobType item, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            base.Put(id, item, zone, context);
        }

        [HttpDelete]
        [Route("{id}")]
        public override void Delete(Guid id, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            base.Delete(id, zone, context);
        }

        [HttpPost]
        [Route("{id}/phase/{phaseName}")]
        public override string Post(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Post(id, phaseName, zone, context);
        }

        [HttpGet]
        [Route("{id}/phase/{phaseName}")]
        public override string Get(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Get(id, phaseName, zone, context);
        }

        [HttpPut]
        [Route("{id}/phase/{phaseName}")]
        public override string Put(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Put(id, phaseName, zone, context);
        }

        [HttpDelete]
        [Route("{id}/phase/{phaseName}")]
        public override string Delete(Guid id, string phaseName, [MatrixParameter] string[] zone = null, [MatrixParameter] string[] context = null)
        {
            return base.Delete(id, phaseName, zone, context);
        }
    }
    */
}
