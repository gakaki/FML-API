using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_api.Models;


using DBUtility;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Types;

using Helpers;
using Newtonsoft.Json;

namespace mvc_api.Controllers
{
    public class AboutController : Controller
    {
        [HttpGet]
        public ActionResult get_sql_data2()
        {
            var point1  = SqlGeometry.Point(107.04352, 28.870554, 4326);
            var point2  = SqlGeometry.Point(103.84041, 29.170240, 4326);

            var sql1    = "insert geo_test (name,geo) values ('point1','" + point1.ToString() + "')";
            var sql2    = "insert geo_test (name,geo) values ('point2','" + point2.ToString() + "')";

            exec_sql(sql1);
            exec_sql(sql2);
            
            var data_d = SqlGeometry.Parse(point1.ToString());
            Response.Write(data_d);

            return new RenderJsonResult
            {
                Result =
                    new
                    {
                        count = 2,
                        page_size = 20,
                        page = 1,
                        point_x = point1.STX.Value,
                        point_y = point1.STY.Value,
                        point = point1.ToString(),

                        houses = new {
                        
                            gakai = 1212,
                            hxm   = "123123213"
                        }
                    }
            };

        }

        private static int exec_sql(string sql1)
        {
            var row_out = 0;
            using (var conn = new SqlDbHelper().GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var cmd = new SqlCommand(sql1, conn))
                {
                     int row = cmd.ExecuteNonQuery();
                     row_out = row;
                }
            }
            return row_out;
        }

        [HttpGet]
        public ActionResult get_sql_data()
        {
            

            DataTable table = new DataTable();


            DBUtility.SqlDbHelper s = new SqlDbHelper();
            using (var conn = s.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand comm = new SqlCommand("select top 20 * from familiarrealty_f.houseinfo order by posttime desc", conn);
                //comm.Parameters.Add(new SqlParameter("@begindate", beginDate));
                //comm.Parameters.Add("@enddate", SqlDbType.DateTime, 8, "Adate").Value = endDate + " 23:59:59.999";
                SqlDataReader sdr = comm.ExecuteReader();
                table.Load(sdr);
            }
            
            
            //var resultItems = (from DataRow dr in table.AsEnumerable()
            //                   select new 
            //                   {
            //                       num_room = dr["num_room"].ToString()
                                   
            //                   }).ToList();


            return new RenderJsonResult { Result = 
                new { 
                    count      = table.Rows.Count,
                    page_size  = 20,
                    page       = 1,
                   // houses     = JsonConvert.SerializeObject(table)
                    houses     = table
                } 
            };



            //return View(resultItems);
        }
        public ActionResult ShowAll()
        {
            var data = new List<About>()
            {
                new About {Title="title3",Name="name3"},
                new About {Title="title4",Name="name4"},
                new About {Title="title5",Name="name5"},

            };


            return View(data);
        }

        public ActionResult test_db()
        {
            fml_entity db = new fml_entity();
                var ins = new int[1,2,3,4];
             //var houses = from house in db.houseinfoes
             //             where house.id in ins
             //             select house;
            
                               
            var data = new List<About>()
            {
                new About {Title="title1",Name="name1"},
                new About {Title="title2",Name="name2"},
                new About {Title="title3",Name="name3"},

            };
            return View(data);
        }


        public ActionResult GetData()
        {
            var data = new List<About>()
            {
                new About {Title="title1",Name="name1"},
                new About {Title="title2",Name="name2"},
                new About {Title="title3",Name="name3"},

            };
            return View(data);
        }

        public string Show(string about)
        {
            string message = about;

            return message;
        }

        //
        // GET: /About/

        public ActionResult Index()
        {
            About a = new About()
            {
                Title = "new title",
                Name = "i change the name second is that ok oh no"
            };

            return View(a);
        }


        //http://localhost:49800/about/browse?genre=Disco
        // // GET: /Store/Browse?genre=Disco
        public string Browse(string genre)
        {
            string message = HttpUtility.HtmlEncode("Store.Browse, Genre = " + genre); 
            return message;
        }


        //http://localhost:49800/about/details/5
        // // GET: /Store/Details/5
        public string Details(int id)
        {
            string message = "Store.Details, ID = " + id; return message;
        }
    }
}
