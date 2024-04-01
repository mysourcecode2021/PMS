using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeRepository _repo = new EmployeeRepository();
        CommonRepository _comm = new CommonRepository();

        public async Task<IActionResult> Index()
        {
            ViewData["Department"] = await _comm.GetLookup("Department");
            ViewData["Position"] = await _comm.GetLookup("Position");
            ViewData["MaritalStatus"] = await _comm.GetLookup("SystemMaster", "MaritalStatus");
            ViewData["PKWTT"] = await _comm.GetLookup("SystemMaster", "PKWTT");
            ViewData["MaritalType"] = await _comm.GetLookup("SystemMaster", "MaritalType");

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
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "Employee");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }

        public async Task<IActionResult> GenerateNIK()
        {
            var data = await _repo.GenerateNIK();
            return Json(data[0].Msg);
        }
    }
}
