using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MSFSFlightFollowing.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace MSFSFlightFollowing.Controllers
{
   public class HomeController : Controller
   {
      private readonly IWebHostEnvironment _host;

      public HomeController(IWebHostEnvironment host)
      {
         _host = host;
      }

      public IActionResult Index()
      {
         return View();
      }

      [HttpGet("get/airports/")]
      public JsonResult Airports()
      {
         var result = new List<AIRPORT>();
         var path = Path.Combine(_host.ContentRootPath, "Airports");
         var files = Directory.GetFiles(path).ToArray();

         foreach (var file in files)
         {
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
               if (fs.Length == 0)
                  continue;

               XmlSerializer serializer = new XmlSerializer(typeof(OPENAIP));
               var xml = (OPENAIP)serializer.Deserialize(fs);

               foreach (var airport in xml.WAYPOINTS.AIRPORT)
               {
                  if (airport.TYPE != "HELI_CIVIL")
                     result.Add(airport);
               }
            }
         }

         return Json(new { data = result });
      }
   }
}
