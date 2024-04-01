using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.DirectoryServices;

namespace WebApp.Controllers
{
    public class LoginADController : Controller
    {
        public IActionResult Index(string msg = "")
        {
            TempData["Message"] = msg;

            return View();
        }

        public IActionResult AuthenticateUser(string email, string password)
        {
            IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
            string msg = "";
            string domainName = conf["AppSettings:DomainName"].ToString();
            
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + domainName, email, password);
                DirectorySearcher dsearch = new DirectorySearcher(de);
                SearchResult results = null;

                results = dsearch.FindOne();

                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }

            if (msg == "Success")
            {
                return RedirectToAction("Index", "AttendanceOnline", new { u = email });
            }
            else
            {
                return RedirectToAction("Index", "LoginAD", new { msg = msg });
            }
        }
    }
}
