using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WebApp.Utils
{
    public static class NPOIUtil
    {
        public static void CopyImage(HSSFSheet sheet, HSSFPictureData picData, int row, int col)
        {

            var pictureIdx = sheet.Workbook.AddPicture(picData.Data, (NPOI.SS.UserModel.PictureType)picData.Format);
            var drawing = sheet.CreateDrawingPatriarch();
            ICreationHelper helper = sheet.Workbook.GetCreationHelper();
            var anchor = helper.CreateClientAnchor();
            anchor.Col1 = col;
            anchor.Row1 = row;
            var pict = drawing.CreatePicture(anchor, pictureIdx);
            pict.Resize();
        }
        public static void AddCellValue(HSSFSheet sheet, int row, int cell, object Value)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(sheet.GetRow(row).GetCell(cell).ToString() + (Value == null ? "" : Value.ToString()));
        }
        public static void SetCellValue(HSSFSheet sheet, int row, int cell, object Value)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? "" : Value.ToString());
        }
        public static void AddCellValueX(XSSFSheet sheet, int row, int cell, object Value)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(sheet.GetRow(row).GetCell(cell).ToString() + (Value == null ? "" : Value.ToString()));
        }
        public static void SetCellValueX(XSSFSheet sheet, int row, int cell, object Value)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? "" : Value.ToString());
        }

        public static string CellName(int row, int column)
        {
            string cellName = "A1";
            try
            {
                string letter = "";
                string number = "";

                if (column <= 25)
                    letter = Convert.ToString((char)('A' + column));
                else if (column > 25 && column <= 51)
                {
                    int columnT = column % (26 * 1);
                    letter = "A";
                    string secondLetter = Convert.ToString((char)('A' + columnT));
                    letter += secondLetter;
                }
                else if (column > 51 && column <= 77)
                {
                    int columnT = column % (26 * 1);
                    letter = "B";
                    string secondLetter = Convert.ToString((char)('A' + columnT));
                    letter += secondLetter;
                }

                number = Convert.ToString(row + 1);

                cellName = letter + number;
            }
            catch
            {

            }
            return cellName;
        }
        /// <summary>
        /// set numeric cell
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        /// <param name="cell"></param>
        /// <param name="Value"></param>
        public static void SetCellValueNumeric(HSSFSheet sheet, int row, int cell, object Value)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            Double a = 0;
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? a : Convert.ToDouble(Value));
        }

        public static void SetCellValueWithStyle(HSSFSheet sheet, int row, int cell, object Value, ICellStyle cellStyle)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? "" : Value.ToString());
            sheet.GetRow(row).GetCell(cell).CellStyle = cellStyle;
        }
        public static void SetCellValueWithStyle(XSSFSheet sheet, int row, int cell, object Value, ICellStyle cellStyle)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? "" : Value.ToString());
            sheet.GetRow(row).GetCell(cell).CellStyle = cellStyle;
        }
        public static void SetCellValueNumericWithStyle(HSSFSheet sheet, int row, int cell, object Value, ICellStyle cellStyle)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            Double a = 0;
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? a : Convert.ToDouble(Value));
            sheet.GetRow(row).GetCell(cell).CellStyle = cellStyle;
        }
        public static void SetCellValueNumericWithStyle(XSSFSheet sheet, int row, int cell, object Value, ICellStyle cellStyle)
        {
            if (sheet.GetRow(row) == null)
            {
                sheet.CreateRow(row);
            }
            if (sheet.GetRow(row).GetCell(cell) == null)
            {
                sheet.GetRow(row).CreateCell(cell);
            }
            Double a = 0;
            sheet.GetRow(row).GetCell(cell).SetCellValue(Value == null ? a : Convert.ToDouble(Value));
            sheet.GetRow(row).GetCell(cell).CellStyle = cellStyle;
        }
    }
}
