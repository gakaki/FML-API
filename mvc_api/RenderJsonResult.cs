using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using mvc_api.Models;
using Newtonsoft.Json;

namespace mvc_api
{
    /// <summary>
    /// An action result that renders the given object using JSON to the response stream.
    /// </summary>
    public class RenderJsonResult : ActionResult
    {
        /// <summary>
        /// The result object to render using JSON.
        /// </summary>
        public object Result { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";

            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(context.HttpContext.Response.Output, this.Result);
        }
    }

}