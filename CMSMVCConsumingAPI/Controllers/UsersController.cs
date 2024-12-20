using CMSMVCConsumingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMSMVCConsumingAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var apiurl = "https://localhost:7225/api/Users";

            using var client = _httpClientFactory.CreateClient();
            var jsoncontent =  new StringContent(JsonConvert.SerializeObject(user), System.Text.Encoding.UTF8, "application/json");
           // Console.WriteLine(jsoncontent);
            var response = await client.PostAsync(apiurl, jsoncontent);
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registration Failed");
                return View(user);
            }
            
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
		public async Task<IActionResult> Login(User loginmodel)
		{
			var apiurl = "https://localhost:7225/api/Token";

			using var client = _httpClientFactory.CreateClient();
			var jsoncontent = new StringContent(JsonConvert.SerializeObject(loginmodel), System.Text.Encoding.UTF8, "application/json");
			// Console.WriteLine(jsoncontent);
			var response = await client.PostAsync(apiurl, jsoncontent);
            if (response.IsSuccessStatusCode)
            {
                var responsecontent = await response.Content.ReadAsStringAsync();
                var tokenresponse = JsonConvert.DeserializeObject<JObject>(responsecontent);

                if (tokenresponse.TryGetValue("token", out var tokenValue) && tokenValue.Type == JTokenType.String)
                {
                    var jwttoken = tokenValue.Value<string>();
                    //Store in the coookie
                    Response.Cookies.Append("jwttoken", jwttoken, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(10)
                    });

                }
                return RedirectToAction("Index", "Books");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Failed");
            }
			return View(loginmodel);
		}
	}
}
