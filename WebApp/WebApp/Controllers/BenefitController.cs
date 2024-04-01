using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class BenefitController : Controller
    {
        BenefitRepository _repo = new BenefitRepository();
        CommonRepository _comm = new CommonRepository();

        public async Task<IActionResult> Index()
        {
            ViewData["NIKName"] = await _comm.GetLookup("NIKName");

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
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "Benefit");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }

        public async Task<IActionResult> UploadFile(IFormFile fileupload)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = fileupload.FileName;
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                fileupload.CopyTo(stream);
            }
            string pathfile = Path.Combine(path, fileName);
            MyExcel exc = new MyExcel(pathfile, true, "Sheet1", null, null, null, null);
            var dataList = exc.Table;
            dataList.RemoveRange(0, 12);
            var categoryId = dataList[0][0];
            var categoryName = dataList[0][1];
            dataList.RemoveAt(0);
            string userId = HttpContext.Session.GetString("UserId")!;
            await _repo.SaveBenefitUpload(dataList, categoryId, categoryName, userId);

            return Json(new { Status = "Done" });
        }
    }
}
