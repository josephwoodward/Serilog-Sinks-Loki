using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog.Sinks.Loki.ExampleWebApp.Models;

namespace Serilog.Sinks.Loki.ExampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string res = null;
            throw new ArgumentNullException(nameof(res));
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
