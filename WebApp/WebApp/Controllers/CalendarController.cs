﻿using Microsoft.AspNetCore.Mvc;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class CalendarController : Controller
    {
        CalendarRepository _repo = new CalendarRepository();

        public IActionResult Index()
        {
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
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "Calendar");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }
    }
}
