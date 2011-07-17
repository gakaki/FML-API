using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace mvc_api
{
    public class RenderBsonResult : ActionResult
    {
        public object Result { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpContextBase httpContextBase = context.HttpContext;
            httpContextBase.Response.Buffer = true;
            httpContextBase.Response.Clear();

            httpContextBase.Response.ContentType = "application/bson";
            
            JsonSerializer serializer = new JsonSerializer();
         
            BsonWriter writer = new BsonWriter(httpContextBase.Response.OutputStream);
            serializer.Serialize(writer, Result);
            httpContextBase.Response.Write(writer);

           

        }
    }
}