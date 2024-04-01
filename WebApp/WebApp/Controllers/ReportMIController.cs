﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class ReportMIController : Controller
    {
        MedicalInsuranceRepository _repo = new MedicalInsuranceRepository();
        CommonRepository _comm = new CommonRepository();

        public async Task<IActionResult> Index()
        {
            ViewData["YEAR"] = await _comm.GetLookup("SystemMaster", "YEAR");

            return View();
        }

        public async Task<IActionResult> GetData([FromBody] dynamic param)
        {
            var data = await _repo.GetDataPaging(param);
            ViewData["DataList"] = data;

            return PartialView("_DataGrid");
        }

        public async Task<IActionResult> ExportExcel([FromBody] dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));

            MyExport util = new MyExport();
            List<dynamic> data = await _repo.GetDataExport(param);
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "List Medical Insurance.", d.ParamHeader.ToString().Replace(";", ""), "MI");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }
    }
}