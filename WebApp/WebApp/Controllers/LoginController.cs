using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        CommonRepository _comm = new CommonRepository();

        public IActionResult Index(string msg = "")
        {
            TempData["Message"] = msg;

            return View();
        }

        public async Task<IActionResult> Auth(string email, string password)
        {
            var auth = await _comm.GetAuth(email, password);

            if (auth.Count > 0)
            {
                string name = auth[0].Name;
                HttpContext.Session.SetString("UserId", email);
                HttpContext.Session.SetString("Name", name);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var msg = "User Not Registered";
                return RedirectToAction("Index", "Login", new { msg = msg });
            }
        }
    }
}
