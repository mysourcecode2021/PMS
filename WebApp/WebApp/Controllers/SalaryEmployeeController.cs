using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Dynamic;
using WebApp.Repository;
using WebApp.Utils;

namespace WebApp.Controllers
{
    public class SalaryEmployeeController : Controller
    {
        GenerateRepository _repo = new GenerateRepository();
        CommonRepository _comm = new CommonRepository();

        public async Task<IActionResult> Index()
        {
            ViewData["YEAR"] = await _comm.GetLookup("SystemMaster", "YEAR");

            return View();
        }

        public async Task<IActionResult> GetData([FromBody] dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var parHeader = d.ParamHeader.ToString().Split(";");
            ViewBag.PaymentType = parHeader[0];

            var data = await _repo.GetDataPaging(param);
            ViewData["DataList"] = data;

            return PartialView("_DataGrid");
        }

        public async Task<IActionResult> GetDataByKey([FromBody] dynamic param)
        {
            var data = await _repo.GetDataByKey(param);
            return Json(data);
        }

        public async Task<IActionResult> ExportExcel([FromBody] dynamic param)
        {
            MyExport util = new MyExport();
            List<dynamic> data = await _repo.GetDataExport(param);
            byte[] result = util.ExportListtoExcel<dynamic>(data, "Data", "Payment");
            return Json(Convert.ToBase64String(result.ToArray(), 0, result.ToArray().Length));
        }

        public async Task<IActionResult> GenerateSlip([FromBody] dynamic param)
        {
            string userId = HttpContext.Session.GetString("UserId")!;
            var data = await _repo.GenerateSlip(param, userId);
            return Json(data[0].Msg);
        }

        public async Task<IActionResult> DownloadSlip(string paymentId)
        {
            dynamic param = new ExpandoObject();
            param.Param = paymentId;
            param.Type = "GetDetailPayment";
            List<dynamic> row = await _repo.GetDataByKey(JsonConvert.SerializeObject(param));

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFFont myFont = (HSSFFont)workbook.CreateFont();
            myFont.IsBold = true;

            HSSFCellStyle centerCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            centerCellStyle.SetFont(myFont);
            centerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            centerCellStyle.Alignment = HorizontalAlignment.Center;

            HSSFCellStyle rightCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            rightCellStyle.VerticalAlignment = VerticalAlignment.Center;
            rightCellStyle.Alignment = HorizontalAlignment.Right;
            //rightCellStyle.BorderTop = BorderStyle.Medium;
            //rightCellStyle.BorderBottom = BorderStyle.Medium;
            //rightCellStyle.BorderLeft = BorderStyle.Medium;
            //rightCellStyle.BorderRight = BorderStyle.Medium;

            ISheet Sheet = workbook.CreateSheet("Slip");

            CreateCell(Sheet.CreateRow(3), 1, row[0].PTName, centerCellStyle);
            var mRow3 = new NPOI.SS.Util.CellRangeAddress(3, 3, 1, 9);
            Sheet.AddMergedRegion(mRow3);
            RegionUtil.SetBorderBottom(2, mRow3, Sheet);
            RegionUtil.SetBorderLeft(2, mRow3, Sheet);
            RegionUtil.SetBorderRight(2, mRow3, Sheet);
            RegionUtil.SetBorderTop(2, mRow3, Sheet);

            CreateCell(Sheet.CreateRow(4), 1, "Salary Slip / " + row[0].Periode, centerCellStyle);
            var mRow4 = new NPOI.SS.Util.CellRangeAddress(4, 4, 1, 9);
            Sheet.AddMergedRegion(mRow4);
            RegionUtil.SetBorderBottom(2, mRow4, Sheet);
            RegionUtil.SetBorderLeft(2, mRow4, Sheet);
            RegionUtil.SetBorderRight(2, mRow4, Sheet);
            RegionUtil.SetBorderTop(2, mRow4, Sheet);

            int nextRow = 5;
            IRow rowSheet = Sheet.CreateRow(nextRow);

            CreateCell(rowSheet, 1, "Name", null);
            CreateCell(rowSheet, 2, row[0].Name, null);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 2, 4));
            CreateCell(rowSheet, 6, "Dept", null);
            CreateCell(rowSheet, 7, row[0].DepartmentName, null);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 7, 9));
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 1), Sheet);
            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 9, 9), Sheet);

            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 6, "Position", null);
            CreateCell(rowSheet, 7, row[0].PositionName, null);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 7, 9));
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 1), Sheet);
            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 9, 9), Sheet);

            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 1, "Overtime", null);
            CreateCell(rowSheet, 2, row[0].TotalOT.ToString(), null);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 2, 4));
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 1), Sheet);
            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 9, 9), Sheet);

            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 1, "Overtime / Hour", null);
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 1), Sheet);
            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 9, 9), Sheet);

            int startIncome = 9;
            var totalIncome = 0;
            foreach (var r in row.Where(x => x.Category == "Income"))
            {
                IRow rowIncome = Sheet.CreateRow(startIncome);
                CreateCell(rowIncome, 1, r.Component, null);
                CreateCell(rowIncome, 2, "Rp.", null);
                CreateCell(rowIncome, 3, string.Format("{0:#,0.00}", r.Amount), rightCellStyle);
                SetBorderRow(startIncome, Sheet);
                totalIncome += Convert.ToDecimal(r.Amount.ToString());

                startIncome++;
            }

            int startDeduction = 9;
            var totalDeduction = 0;
            foreach (var r in row.Where(x => x.Category == "Deduction"))
            {
                IRow rowDeduction;
                if (startIncome > startDeduction)
                {
                    rowDeduction = Sheet.GetRow(startDeduction);
                    CreateCell(rowDeduction, 6, r.Component, null);
                    CreateCell(rowDeduction, 7, "Rp.", null);
                    CreateCell(rowDeduction, 9, string.Format("{0:#,0.00}", r.Amount), rightCellStyle);
                    SetBorderRow(startDeduction, Sheet);
                    totalDeduction += Convert.ToDecimal(r.Amount.ToString());
                }
                else
                {
                    rowDeduction = Sheet.CreateRow(startDeduction);
                    CreateCell(rowDeduction, 6, r.Component, null);
                    CreateCell(rowDeduction, 7, "Rp.", null);
                    CreateCell(rowDeduction, 9, string.Format("{0:#,0.00}", r.Amount), rightCellStyle);
                    SetBorderRow(startDeduction, Sheet);
                    if (r.Component != "Income Tax (BIK)")
                    {
                        totalDeduction += Convert.ToDecimal(r.Amount.ToString());
                    }
                }


                startDeduction++;
            }

            nextRow = startIncome;
            if ((startIncome - startDeduction) > 0)
            {
                nextRow = startIncome;
            }
            else
            {
                nextRow = startDeduction;
            }
            SetBorderRow(nextRow, Sheet);

            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 1, "Gross Income", null);
            CreateCell(rowSheet, 2, "Rp.", null);
            CreateCell(rowSheet, 3, string.Format("{0:#,0.00}", totalIncome), rightCellStyle);
            SetBorderRow(nextRow, Sheet);
            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 1, "Take Home Pay", null);
            CreateCell(rowSheet, 2, "Rp.", null);
            CreateCell(rowSheet, 3, string.Format("{0:#,0.00}", (totalIncome - totalDeduction)), rightCellStyle);
            CreateCell(rowSheet, 6, "Total Deduction", null);
            CreateCell(rowSheet, 7, "Rp.", null);
            CreateCell(rowSheet, 9, string.Format("{0:#,0.00}", totalDeduction), rightCellStyle);
            SetBorderRow(nextRow, Sheet);

            nextRow = nextRow + 2;
            rowSheet = Sheet.CreateRow(nextRow);
            var terbilang = await _repo.GetTextMoney((totalIncome - totalDeduction));
            CreateCell(rowSheet, 1, "Says       : " + terbilang + " rupiah.", null);
            var f1 = new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 9);
            Sheet.AddMergedRegion(f1);
            RegionUtil.SetBorderBottom(2, f1, Sheet);
            RegionUtil.SetBorderLeft(2, f1, Sheet);
            RegionUtil.SetBorderRight(2, f1, Sheet);
            RegionUtil.SetBorderTop(2, f1, Sheet);
            nextRow++;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 1, "Note : Tax Paid by Employee", null);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 3));
            CreateCell(rowSheet, 6, "Jakarta, " + DateTime.Now.ToString("dd MMM yyyy"), centerCellStyle);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 6, 9));

            nextRow = nextRow + 2;
            rowSheet = Sheet.CreateRow(nextRow);
            CreateCell(rowSheet, 6, "Prepared By", null);
            CreateCell(rowSheet, 9, "Checked By", null);
            nextRow = nextRow + 5;
            rowSheet = Sheet.CreateRow(nextRow);
            Sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(nextRow, nextRow, 1, 4));
            CreateCell(rowSheet, 1, "This payslip is private and confidential", null);

            Sheet.AutoSizeColumn(1);
            Sheet.AutoSizeColumn(3);
            Sheet.AutoSizeColumn(6);
            Sheet.AutoSizeColumn(9);

            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            return File(output.ToArray(),   //The binary data of the XLS file
             "application/vnd.ms-excel", //MIME type of Excel files
             "Slip_" + paymentId + ".xls");
        }

        private void CreateCell(IRow CurrentRow, int CellIndex, string Value, HSSFCellStyle Style)
        {
            ICell Cell = CurrentRow.CreateCell(CellIndex);
            Cell.SetCellValue(Value);
            if (Style != null)
            {
                Cell.CellStyle = Style;
            }
        }

        private void SetBorderRow(int row, ISheet Sheet)
        {
            RegionUtil.SetBorderBottom(2, new NPOI.SS.Util.CellRangeAddress(row, row, 1, 9), Sheet);
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(row, row, 1, 9), Sheet);
            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(row, row, 1, 9), Sheet);
            RegionUtil.SetBorderTop(2, new NPOI.SS.Util.CellRangeAddress(row, row, 1, 9), Sheet);

            RegionUtil.SetBorderRight(2, new NPOI.SS.Util.CellRangeAddress(row, row, 1, 1), Sheet);
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(row, row, 6, 6), Sheet);
            RegionUtil.SetBorderLeft(2, new NPOI.SS.Util.CellRangeAddress(row, row, 7, 7), Sheet);
        }
    }
}
