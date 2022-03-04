using AsyncExercise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExercise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static CancellationTokenSource  _cancellationToken = null;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> Index(ContentInfo  contentInfo)
        {
            ModelState.Clear();
            _cancellationToken = new CancellationTokenSource();

            List<string> resource = new List<string>();
            resource.Add("https://www.spec-india.com/");
            resource.Add("https://reqbin.com/");
            resource.Add("https://www.google.com/");
             contentInfo.ContentLength= await DoSumOfContentLength(resource);
            //ViewBag.Content = await GetContentLengthsum(urlList);
            return View(contentInfo);
        }
        private async Task<int> DoSumOfContentLength(List<string> resourcelist)
        {
            int contentlength = 0;
            try
            {
                foreach (string res in resourcelist)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        _cancellationToken.Token.ThrowIfCancellationRequested();
                        return contentlength;
                    }
                    contentlength += await GetResLenght(res);
                }
                return contentlength;
            }
            catch (Exception ex)
            {
                return contentlength;
            }
        }

        private async Task<int> GetResLenght(string res)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(res);
                string resultContent = await response.Content.ReadAsStringAsync();
                return resultContent.Length;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public IActionResult Cancel()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel();
            return Json("Cancelled Successfully");
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
