using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ODataBookStoreWebClient.Controllers
{
    public class PressController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public PressController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5262/api/presses";
        }
        // GET: BookController
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<Press>>(strData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(books);
        }

        // GET: PressControll/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PressControll/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PressControll/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PressControll/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PressControll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PressControll/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PressControll/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
