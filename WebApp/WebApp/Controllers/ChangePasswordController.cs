using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using WebApp.Repository;

namespace WebApp.Controllers
{
    public class ChangePasswordController : Controller
    {
        CommonRepository _comm = new CommonRepository();

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ChangePass([FromBody] dynamic param)
        {
            var data = await _comm.ChangePass(param);
            return Json(data);
        }
    }
}
