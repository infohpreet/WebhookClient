using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var postData = DataStore.ReadFromDisk();

            var output = string.Empty;

            var list = postData.Items;

            list.Reverse();

            var counter = 0;

            list.ForEach(item => { item.Id = counter++; });

            ViewBag.Output = list;

            return View();
        }

        [Route("/post")]
        [HttpPost]
        public HttpStatusCode Post()
        {
            var data = string.Empty;

            var memoryStream = new MemoryStream();

            Request.Body.CopyToAsync(memoryStream);

            data = ASCIIEncoding.UTF8.GetString(memoryStream.ToArray());

            if (!String.IsNullOrWhiteSpace(data))
            {
                var postData = DataStore.ReadFromDisk();

                var headers = new List<KeyValue>();

                foreach (var header in Request.Headers.ToList())
                {
                    headers.Add(new KeyValue(header.Key, header.Value));
                }

                postData.Items.Add(new DataItem { Body = data, Headers = headers, TimeStamp = DateTime.Now });

                if (postData.Items.Count > 50)
                {
                    postData.Items = postData.Items.Skip(postData.Items.Count - 50).ToList();
                }

                DataStore.WriteToDisk(postData);
            }

            return HttpStatusCode.OK;
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
