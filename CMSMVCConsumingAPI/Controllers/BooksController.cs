using CMSMVCConsumingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CMSMVCConsumingAPI.Controllers
{
	public class BooksController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public BooksController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task<IActionResult> Index()
		{
			//Get the jwttoken stored in cookie
		   var tokenstring=	Request.Cookies["jwttoken"];

			var client = _httpClientFactory.CreateClient("API");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenstring);

			var response = await client.GetAsync("/api/Books");
			var books = await response.Content.ReadFromJsonAsync<IEnumerable<Book>>();

			return View(books);
		}
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			return View();
		}

	    [HttpPost]
		public async Task<IActionResult> Create(Book book)
		{
			//Get the jwttoken stored in cookie
			var tokenstring = Request.Cookies["jwttoken"];

			var client = _httpClientFactory.CreateClient("API");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenstring);

			var jsoncontent=JsonConvert.SerializeObject(book);
			var content = new StringContent(jsoncontent, System.Text.Encoding.UTF8, "application/json");

			var response = await client.PostAsync("/api/Books",content);
			
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Add book Failed");
				return View("Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var tokenstring = Request.Cookies["jwttoken"];
			var client = _httpClientFactory.CreateClient("API");
			client.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenstring);

			var response = await client.GetAsync($"/api/Books/{id}");
			if (response.IsSuccessStatusCode)
			{
				var bookJson = await response.Content.ReadAsStringAsync();
				var book = JsonConvert.DeserializeObject<Book>(bookJson);
				return View(book);
			}

			return NotFound();
		}
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
            var tokenstring = Request.Cookies["jwttoken"];
            var client = _httpClientFactory.CreateClient("API");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenstring);

            var response = await client.GetAsync($"/api/Books/{id}");
            if (response.IsSuccessStatusCode)
            {
                var bookJson = await response.Content.ReadAsStringAsync();
                var book = JsonConvert.DeserializeObject<Book>(bookJson);
                return View(book);
            }

            return NotFound();
        }

        [HttpPost]
		public async Task<IActionResult> Edit(int id, Book book)
		{
			if (id != book.BookId)
			{
				return BadRequest();
			}

            var tokenstring = Request.Cookies["jwttoken"];
            var client = _httpClientFactory.CreateClient("API");
            client.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenstring);

			var bookJson = JsonConvert.SerializeObject(book);
			var content = new StringContent(bookJson, Encoding.UTF8, "application/json");

			var response = await client.PutAsync($"/api/Books/{id}", content);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return NotFound();
			}

			return View(book);
		}
	}
}
