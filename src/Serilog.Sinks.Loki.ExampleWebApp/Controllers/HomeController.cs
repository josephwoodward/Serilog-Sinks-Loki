using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog.Sinks.Loki.ExampleWebApp.Models;

namespace Serilog.Sinks.Loki.ExampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string result;
            throw new ArgumentNullException(nameof(result));
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
