using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class ReportToBankController : Controller
    {
        SalaryRepository _repo = new SalaryRepository();
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ExportExcel([FromBody] dynamic param)
        {
            MyExport util = new MyExport();
            List<dynamic> data = await _repo.ReportToBank(param);
            byte[] result = util.ExportReportToBank<dynamic>(data);
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }
    }
}
