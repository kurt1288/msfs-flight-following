using Microsoft.AspNetCore.Mvc;

namespace MSFSFlightFollowing.Controllers
{
   public class HomeController : Controller
   {
      public IActionResult Index()
      {
         return View();
      }
   }
}
