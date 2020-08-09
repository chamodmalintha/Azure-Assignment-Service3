using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackExchange.Redis;

namespace WebAppTwo.Controllers
{
    public class HomeController : Controller
    {
        public static string connectionstring;
        public static string CacheConnection = "redis-se-2015-043.redis.cache.windows.net:6380,password=o5Qw67PQ17VML5EFiwn7aGFI071ieOA8PvNZ1tA5vtw=,ssl=True,abortConnect=False";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "No Of Employees";

            connectionstring = "Server=tcp:server-se-2015-043.database.windows.net,1433;Initial Catalog=DB-se-2015-043;Persist Security Info=False;User ID=admin-se-2015-043;Password=SEloginS43;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var conn = new SqlConnection(connectionstring);
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", conn);

            conn.Open();
            Int32 NofEmployees = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();

            ViewBag.Message = "Employee Count : " +NofEmployees;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Redis Cache";
            connectionstring = "Server=tcp:server-se-2015-043.database.windows.net,1433;Initial Catalog=DB-se-2015-043;Persist Security Info=False;User ID=admin-se-2015-043;Password=SEloginS43;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connection = new SqlConnection(connectionstring);
            var command = new SqlCommand("SELECT COUNT(*) FROM Employee", connection);

            connection.Open();
            Int32 rowCount = Convert.ToInt32(command.ExecuteScalar());
            command.Dispose();
            connection.Close();

            IDatabase cache = lazyConnection.Value.GetDatabase();
            ViewBag.MessageOne = "Reading Cache : " + cache.StringGet("EmployeeCount").ToString();
            ViewBag.MessageTwo = "Writing Cache : " + cache.StringSet("EmployeeCount", "No of Employees : " + rowCount.ToString() + " @ " + DateTime.Now.ToShortTimeString()).ToString();
            ViewBag.MessageThree = "Reading Cache : " + cache.StringGet("EmployeeCount").ToString();
            cache.KeyExpire("EmployeeCount", DateTime.Now.AddMinutes(1));
            lazyConnection.Value.Dispose();

            return View();
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            return ConnectionMultiplexer.Connect(CacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

    }
}