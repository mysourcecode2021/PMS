using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;

namespace WebApp.Controllers
{
    public class AttendanceOnlineController : Controller
    {
        AttendanceRepository _repo = new AttendanceRepository();

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SendProcess([FromBody] dynamic param)
        {
            var data = await _repo.SendOTP(param, GenerateOTP());
            return Json(data);
        }

        public async Task<IActionResult> SubmitOTP([FromBody] dynamic param)
        {
            var data = await _repo.SubmitOTP(param);
            return Json(data);
        }

        protected string GenerateOTP()
        {
            string characters = "1234567890";
            string otp = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
    }
}
