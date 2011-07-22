using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Wrappers;
using System.Threading;
using ServiceStack.Redis;


public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime d)
    {
        var epoch = d - new DateTime(1970, 1, 1, 0, 0, 0);

        return (long)epoch.TotalSeconds;
    }
}
//static DateTime ConvertFromUnixTimestamp(double timestamp)
//{
//    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
//    return origin.AddSeconds(timestamp);
//}


//static double ConvertToUnixTimestamp(DateTime date)
//{
//    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
//    TimeSpan diff = date - origin;
//    return Math.Floor(diff.TotalSeconds);
//}

namespace mvc_api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewBag.Message = String.Format("{0}:{1}:{2} this is the view bag message yes itis do you know it",
                                        RouteData.Values["controller"],
                                        RouteData.Values["action"],
                                        RouteData.Values["id"]);

            ViewBag.gakaki = "gak1aki";
            return View();
        }

        public ActionResult house_geo_process_to_mongo()
        {

            

            //third send data to redis for delay google parse address
        
         
            //mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]

            //string mongodb_connnection_string = "mongodb://gakaki:z5896321@127.0.0.1:27017";
            string mongodb_connnection_string = "mongodb://127.0.0.1:27017";
            MongoServer server = MongoServer.Create(mongodb_connnection_string);
            MongoDatabase db = server.GetDatabase("fml");

            //using (server.RequestStart(mongodb))
            //{
            //    // a series of operations that must be performed on the same connection

            //    MongoCollection<Employee> employees =
            //                db.GetCollection<Employee>("employees");


            //}
            MongoCollection<BsonDocument> houses = db.GetCollection<BsonDocument>("houses");
            BsonDocument house = new BsonDocument {
                  {  "address"           , "上海市普陀区金鼎路298弄45号601" },
                  {  "zone_name"         , "上海家园小区" },
                  {  "sql_id"            , 123123123123 },
                  {  "master"            , "业主"},
                  {  "master_mobile"     , 13917447328},            //业主电话
                  {  "manage"            , "经纪人"},
                  {  "manage_mobile"     , 13917447328}            //经纪人电话
            };


            dynamic house_j         = new JObject();
            house_j.address         = "上海市普陀区金鼎路298弄45号601";
            house_j.zone_name       = "上海家园小区";
            house_j.sql_id          = 123123123123;
            house_j.master          = "业主";
            house_j.master_mobile   = 13917447328;              //业主电话
            house_j.manage          = "经纪人";
            house_j.manage_mobile   = 13917447328;             //经纪人电话
            house_j.create_time     = DateTime.UtcNow.ToUnixTimestamp();


            dynamic loc             = new JObject();
            loc.lat                 = 1223212.13223;
            loc.lang                = 123123.241234;

            house_j.loc             = loc;


            var bson_doc = (BsonDocument)BsonSerializer.Deserialize<BsonDocument>(house_j.ToString());
           // var house_obj = BsonSerializer.Deserialize<QueryDocument>(house);
           // return new RenderJsonResult { Result = new { house = house_obj } };
            using (server.RequestStart(db))
            {
                // a series of operations that must be performed on the same connection
                houses.Insert(bson_doc);
          
                //server.Disconnect();
            }
          //  【PS教程】新 Lion 风格的平底锅图标制作方法 http://mac.pcbeta.com/thread-51768-1-1.html
            //second insert data to redis
            
            bson_doc["oid"] = bson_doc["_id"].ToString();
            var string_obj = bson_doc.ToJson<MongoDB.Bson.BsonDocument>(new JsonWriterSettings
            {
                GuidRepresentation = GuidRepresentation.Standard,
                OutputMode = JsonOutputMode.JavaScript
            });


            //second insert data to redis



          return Content(string_obj, "application/json");
        //    return new JsonResult { Data = new  { house = string_obj }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

           //return new RenderJsonResult { Result = new { house = house_j } };
        }

        public ActionResult test_json()
        {
            dynamic product = new JObject();
            product.ProductName = "Elbow Grease";
            product.Enabled = true;
            product.Price = 4.90m;
            product.StockCount = 9000;
            product.StockValue = 44100;

            // All Elbow Grease must go sale!
            // 50% off price

            product.Price = product.Price / 2;
            product.StockValue = product.StockCount * product.Price;
            product.ProductName = product.ProductName + " (SALE)";

            return new RenderJsonResult { Result = new { product = product } };
        }
  

        public ActionResult test_redis()
        {

            dynamic house_j         = new JObject();
            house_j.address         = "上海市普陀区金鼎路298弄45号601";
            house_j.zone_name       = "上海家园小区";
            house_j.sql_id          = 123123123123;
            house_j.master          = "业主";
            house_j.master_mobile   = 13917447328;              //业主电话
            house_j.manage          = "经纪人";
            house_j.manage_mobile   = 13917447328;             //经纪人电话
            house_j.create_time     = DateTime.UtcNow.ToUnixTimestamp();

            var channel_name        = "address:googlemap:geo:update";
            using (var redisPublisher = new RedisClient())
            {
                for (var i = 1; i <= 10; i++)
                {
                    redisPublisher.PublishMessage(channel_name,house_j.ToString() );
                }
            }
            return new RenderJsonResult { Result = new { house = house_j } };
        }

    }
}
