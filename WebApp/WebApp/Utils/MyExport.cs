using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System.Data;
using WebApp.Repository;

namespace WebApp.Utils
{
    public class MyExport
    {
        CommonRepository _comm = new CommonRepository();

        #region Export List To Excel
        public byte[] ExportListtoExcel<T>(List<T> data, string sheetName, string rptName, string periode = "", string type = "")
        {
            string json = JsonConvert.SerializeObject(data);
            var table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)))!;
            var memoryStream = new MemoryStream();

            using (var workbook = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                workbook.AddWorkbookPart();

                workbook.WorkbookPart!.Workbook = new Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                WorkbookStylesPart stylePart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylePart.Stylesheet = GenerateStylesheet();
                stylePart.Stylesheet.Save();

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                sheets!.Append(new Sheet { Id = relationshipId, SheetId = 1, Name = sheetName });

                if (type == "BPJSTK")
                {
                    var titleRow1 = new Row();
                    titleRow1.AppendChild(new Cell() { CellReference = "B1", CellValue = new CellValue(_comm.GetSysVal("SystemMaster", "Apps|CompanyName")), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow1);
                    var titlespace1 = new Row();
                    sheetData.AppendChild(titlespace1);
                    var titleRow2 = new Row();
                    titleRow2.AppendChild(new Cell() { CellReference = "B3", CellValue = new CellValue("Form 2A ( Rincian Iuran Tenaga Kerja )"), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow2);
                    var titleRow3 = new Row();
                    titleRow3.AppendChild(new Cell() { CellReference = "B4", CellValue = new CellValue("No. NPP"), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "F4", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Default|NoNPP")), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "L4", CellValue = new CellValue("Unit Kerja"), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "M4", CellValue = new CellValue(": Pusat"), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow3);
                    var titleRow4 = new Row();
                    titleRow4.AppendChild(new Cell() { CellReference = "B5", CellValue = new CellValue("Nama Perusahaan"), DataType = CellValues.String });
                    titleRow4.AppendChild(new Cell() { CellReference = "F5", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Apps|CompanyName")), DataType = CellValues.String });
                    titleRow4.AppendChild(new Cell() { CellReference = "L5", CellValue = new CellValue("Bln-Thn"), DataType = CellValues.String });
                    titleRow4.AppendChild(new Cell() { CellReference = "M5", CellValue = new CellValue(": " + periode.Substring(4, 2) + "-" + periode.Substring(0,4)), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow4);
                }
                else if (type == "BPJSKes")
                {
                    var titleRow1 = new Row();
                    titleRow1.AppendChild(new Cell() { CellReference = "J1", CellValue = new CellValue("Form"), DataType = CellValues.String });
                    titleRow1.AppendChild(new Cell() { CellReference = "K1", CellValue = new CellValue(": PPU-1"), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow1);
                    var titlespace1 = new Row();
                    sheetData.AppendChild(titlespace1);
                    var titleRow2 = new Row();
                    titleRow2.AppendChild(new Cell() { CellReference = "A3", CellValue = new CellValue("PERHITUNGAN IURAN BPJS KESEHATAN"), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow2);
                    MergeCells mergeCellsRow2 = new MergeCells();
                    mergeCellsRow2.Append(new MergeCell() { Reference = new StringValue("A3:M3") });
                    sheetPart.Worksheet.InsertAfter(mergeCellsRow2, sheetPart.Worksheet.Elements<SheetData>().First());
                    var titlespace2 = new Row();
                    sheetData.AppendChild(titlespace2);
                    var titleRow3 = new Row();
                    titleRow3.AppendChild(new Cell() { CellReference = "A5", CellValue = new CellValue("Nama Perusahaan"), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "D5", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Apps|CompanyName")), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "I5", CellValue = new CellValue("Bulan"), DataType = CellValues.String });
                    titleRow3.AppendChild(new Cell() { CellReference = "J5", CellValue = new CellValue(": " + periode.Substring(4, 2) + "-" + periode.Substring(0, 4)), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow3);
                    var titleRow4 = new Row();
                    titleRow4.AppendChild(new Cell() { CellReference = "A6", CellValue = new CellValue("Kode Entitas"), DataType = CellValues.String });
                    titleRow4.AppendChild(new Cell() { CellReference = "D6", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Default|KodeEntitas")), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow4);
                    var titleRow5 = new Row();
                    titleRow5.AppendChild(new Cell() { CellReference = "A7", CellValue = new CellValue("Alamat Perusahaan"), DataType = CellValues.String });
                    titleRow5.AppendChild(new Cell() { CellReference = "D7", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Default|AddressCompany")), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow5);
                    var titleRow6 = new Row();
                    titleRow6.AppendChild(new Cell() { CellReference = "A8", CellValue = new CellValue("Wilayah"), DataType = CellValues.String });
                    titleRow6.AppendChild(new Cell() { CellReference = "D8", CellValue = new CellValue(": " + _comm.GetSysVal("SystemMaster", "Default|Wilayah")), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow6);
                }
                else if (type == "Ebupot Month" || type == "Ebupot Year")
                {
                    var titlespace = new Row();
                    sheetData.AppendChild(titlespace);
                }
                else
                {
                    var titleRow1 = new Row();
                    titleRow1.AppendChild(new Cell() { CellReference = "A1", CellValue = new CellValue(_comm.GetSysVal("SystemMaster", "Apps|CompanyName")), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow1);
                    var titleRow2 = new Row();
                    titleRow2.AppendChild(new Cell() { CellReference = "A2", CellValue = new CellValue(rptName), DataType = CellValues.String });
                    sheetData.AppendChild(titleRow2);
                    if (periode != "")
                    {
                        var titleRowP1 = new Row();
                        titleRowP1.AppendChild(new Cell() { CellReference = "A3", CellValue = new CellValue(((type == "MI") ? "Jan - Dec " : "Periode ") + periode), DataType = CellValues.String });
                        sheetData.AppendChild(titleRowP1);
                        var titleRowP2 = new Row();
                        titleRowP2.AppendChild(new Cell() { CellReference = "A4", CellValue = new CellValue("Printed as of " + DateTime.Now.ToString("dd MMM yyyy")), DataType = CellValues.String });
                        sheetData.AppendChild(titleRowP2);
                    }
                    else
                    {
                        var titleRow3 = new Row();
                        titleRow3.AppendChild(new Cell() { CellReference = "A3", CellValue = new CellValue("Printed as of " + DateTime.Now.ToString("dd MMM yyyy")), DataType = CellValues.String });
                        sheetData.AppendChild(titleRow3);
                    }
                    var titleRow4 = new Row();
                    sheetData.AppendChild(titleRow4);
                }

                var headerRow = new Row();
                var columns = new List<string>();
                foreach (DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);

                    var cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    cell.StyleIndex = 2;
                    headerRow.AppendChild(cell);
                }
                
                sheetData.AppendChild(headerRow);

                int seqBPJSKes = 9;
                foreach (DataRow dsrow in table.Rows)
                {
                    seqBPJSKes++;
                    var newRow = new Row();
                    foreach (var col in columns)
                    {
                        var cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(dsrow[col].ToString()!);
                        cell.StyleIndex = 1;
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                if (type == "BPJSKes")
                {
                    sheetData.AppendChild(new Row());
                    seqBPJSKes = seqBPJSKes + 2;
                    var rowttd1 = new Row();
                    rowttd1.AppendChild(new Cell() { CellReference = "H" + seqBPJSKes.ToString(), CellValue = new CellValue(_comm.GetSysVal("SystemMaster", "Default|Area") + ", " + DateTime.Now.ToString("dd MMM yyyy")), DataType = CellValues.String });
                    sheetData.AppendChild(rowttd1);
                    seqBPJSKes++;
                    var rowttd2 = new Row();
                    rowttd2.AppendChild(new Cell() { CellReference = "H" + seqBPJSKes.ToString(), CellValue = new CellValue("Asst. General Manager"), DataType = CellValues.String });
                    sheetData.AppendChild(rowttd2);
                    sheetData.AppendChild(new Row());
                    sheetData.AppendChild(new Row());
                    seqBPJSKes = seqBPJSKes + 3;
                    var rowttd3 = new Row();
                    rowttd3.AppendChild(new Cell() { CellReference = "H" + seqBPJSKes.ToString(), CellValue = new CellValue(_comm.GetSysVal("SystemMaster", "Default|GM_Name")), DataType = CellValues.String });
                    sheetData.AppendChild(rowttd3);
                }

                workbook.Save();
                workbook.Dispose();
            }

            return memoryStream.ToArray();
        }

        public byte[] ExportReportToBank<T>(List<T> data)
        {
            string json = JsonConvert.SerializeObject(data);
            var table = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)))!;
            var memoryStream = new MemoryStream();

            using (var workbook = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                workbook.AddWorkbookPart();

                workbook.WorkbookPart!.Workbook = new Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                WorkbookStylesPart stylePart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                stylePart.Stylesheet = GenerateStylesheet();
                stylePart.Stylesheet.Save();

                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                sheets!.Append(new Sheet { Id = relationshipId, SheetId = 1, Name = "Report" });

                var titlespace1 = new Row();
                sheetData.AppendChild(titlespace1);
                var titlespace2 = new Row();
                sheetData.AppendChild(titlespace2);
                var titlespace3 = new Row();
                sheetData.AppendChild(titlespace3);
                var titlespace4 = new Row();
                sheetData.AppendChild(titlespace4);

                List<dynamic> dataRow = JsonConvert.DeserializeObject<List<dynamic>>(json);
                var titleRow1 = new Row();
                titleRow1.AppendChild(new Cell() { CellReference = "A5", CellValue = new CellValue("File Creation(auto)"), DataType = CellValues.String });
                titleRow1.AppendChild(new Cell() { CellReference = "B5", CellValue = new CellValue("Total Baris(auto)"), DataType = CellValues.String });
                titleRow1.AppendChild(new Cell() { CellReference = "C5", CellValue = new CellValue("Nama File"), DataType = CellValues.String });
                sheetData.AppendChild(titleRow1);
                var titleRow2 = new Row();
                titleRow2.AppendChild(new Cell() { CellReference = "A6", CellValue = new CellValue(dataRow[0].FileCreation.ToString()), DataType = CellValues.String });
                titleRow2.AppendChild(new Cell() { CellReference = "B6", CellValue = new CellValue(dataRow[0].TotalBaris.ToString()), DataType = CellValues.String });
                titleRow2.AppendChild(new Cell() { CellReference = "C6", CellValue = new CellValue(dataRow[0].NamaFile.ToString()), DataType = CellValues.String });
                sheetData.AppendChild(titleRow2);
                var titleRow3 = new Row();
                titleRow3.AppendChild(new Cell() { CellReference = "A7", CellValue = new CellValue("P(auto)"), DataType = CellValues.String });
                titleRow3.AppendChild(new Cell() { CellReference = "B7", CellValue = new CellValue("Tgl Transaksi"), DataType = CellValues.String });
                titleRow3.AppendChild(new Cell() { CellReference = "C7", CellValue = new CellValue("Rek. Debet(16)"), DataType = CellValues.String });
                titleRow3.AppendChild(new Cell() { CellReference = "D7", CellValue = new CellValue("Total Record(auto)"), DataType = CellValues.String });
                titleRow3.AppendChild(new Cell() { CellReference = "E7", CellValue = new CellValue("Total Amount(auto)"), DataType = CellValues.String });
                sheetData.AppendChild(titleRow3);
                var titleRow4 = new Row();
                titleRow4.AppendChild(new Cell() { CellReference = "A8", CellValue = new CellValue(dataRow[0].P.ToString()), DataType = CellValues.String });
                titleRow4.AppendChild(new Cell() { CellReference = "B8", CellValue = new CellValue(dataRow[0].FileCreation.ToString()), DataType = CellValues.String });
                titleRow4.AppendChild(new Cell() { CellReference = "C8", CellValue = new CellValue(dataRow[0].RekDebet.ToString()), DataType = CellValues.String });
                titleRow4.AppendChild(new Cell() { CellReference = "D8", CellValue = new CellValue(dataRow[0].TotalRecord.ToString()), DataType = CellValues.String });
                titleRow4.AppendChild(new Cell() { CellReference = "E8", CellValue = new CellValue(dataRow[0].AmountAll.ToString()), DataType = CellValues.String });
                sheetData.AppendChild(titleRow4);

                var headerRow = new Row();
                var columns = new List<string>();
                foreach (DataColumn column in table.Columns)
                {
                    if (column.ColumnName != "FileCreation" && column.ColumnName != "TotalBaris" && column.ColumnName != "NamaFile" && column.ColumnName != "P" && column.ColumnName != "RekDebet" && column.ColumnName != "TotalRecord" && column.ColumnName != "AmountAll")
                    {
                        columns.Add(column.ColumnName);

                        var cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        cell.StyleIndex = 2;
                        headerRow.AppendChild(cell);
                    }
                }

                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    var newRow = new Row();
                    foreach (var col in columns)
                    {
                        if (col != "FileCreation" && col != "TotalBaris" && col != "NamaFile" && col != "P" && col != "RekDebet" && col != "TotalRecord" && col != "AmountAll")
                        {
                            var cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString()!);
                            cell.StyleIndex = 1;
                            newRow.AppendChild(cell);
                        }
                    }

                    sheetData.AppendChild(newRow);
                }

                workbook.Save();
                workbook.Dispose();
            }

            return memoryStream.ToArray();
        }

        private Stylesheet GenerateStylesheet()
        {
            Stylesheet? styleSheet = null;

            Fonts fonts = new Fonts(
                new Font(
                    new FontSize() { Val = 10 }

                ),
                new Font(
                    new FontSize() { Val = 10 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),
                new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),
                new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                { PatternType = PatternValues.Solid })
            );

            Borders borders = new Borders(
                new Border(),
                new Border(
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder())
            );

            CellFormats cellFormats = new CellFormats(
                new CellFormat(),
                new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },
                new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }
            );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
        #endregion
    }
}
