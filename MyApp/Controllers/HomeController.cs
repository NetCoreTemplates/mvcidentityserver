using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using ServiceStack;
using ServiceStack.Text;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }

        [Authorize]
        public IActionResult RequiresAuth()
        {
            return View();
        }

        [Authorize(Policy = "ProfileScope")]
        public async Task<IActionResult> RequiresScope()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult RequiresRole()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RequiresAdmin()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> CallWebApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:5001/webapi-identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

        public async Task<IActionResult> CallServiceStack()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.Json)); 
            var json = await client.GetStringAsync("https://localhost:5001/servicestack-identity");

            ViewBag.Json = json.IndentJson();
            return View("json");
        }

        public async Task<IActionResult> CallServiceClient()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new JsonServiceClient("https://localhost:5001/") {
                BearerToken = accessToken
            };
            var response = await client.GetAsync(new GetIdentity());

            ViewBag.Json = response.ToJson().IndentJson();
            return View("json");
        }
    }
}