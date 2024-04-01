using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class BPJSEmploymentController : Controller
    {
        BPJSEmploymentRepository _repo = new BPJSEmploymentRepository();
        CommonRepository _comm = new CommonRepository();

        public async Task<IActionResult> Index()
        {
            ViewData["NIKName"] = await _comm.GetLookup("NIKName");
            ViewBag.IuranJKK = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJKK");
            ViewBag.IuranJKM = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJKM");
            ViewBag.IuranJHTTK = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJHTTK");
            ViewBag.IuranJHTCompany = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJHTCompany");
            ViewBag.IuranJPTK = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJPTK");
            ViewBag.IuranJPCompany = await _comm.GetSystemValue("SystemMaster", "Rate|IuranJPCompany");
            ViewBag.BaseCalculation = await _comm.GetSystemValue("SystemMaster", "BaseCalculation|Amount");
            ViewData["YEAR"] = await _comm.GetLookup("SystemMaster", "YEAR");

            return View();
        }

        public async Task<IActionResult> GetData([FromBody] dynamic param)
        {
            var data = await _repo.GetDataPaging(param);
            ViewData["DataList"] = data;

            return PartialView("_DataGrid");
        }

        public async Task<IActionResult> GetDataByKey([FromBody] dynamic param)
        {
            var data = await _repo.GetDataByKey(param);
            return Json(data);
        }

        public async Task<IActionResult> SaveProcess([FromBody] dynamic param)
        {
            string userId = HttpContext.Session.GetString("UserId")!;
            var data = await _repo.SaveProcess(param, userId);
            return Json(data[0].Msg);
        }

        public async Task<IActionResult> ExportExcel([FromBody] dynamic param)
        {
            MyExport util = new MyExport();
            List<dynamic> data = await _repo.GetDataExport(param);
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "BPJS Employment");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }

        public async Task<IActionResult> GenerateBPJS([FromBody] dynamic param)
        {
            var data = await _repo.GenerateBPJS(param);
            return Json(data[0].Msg);
        }
    }
}
