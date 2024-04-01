using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WebApp.Utils
{
    public class MyExcel
    {
        private const char SEGMENT_DELIMITER = ',';
        private const char DOUBLE_QUOTE = '"';
        private const char CARRIAGE_RETURN = '\r';
        private const char NEW_LINE = '\n';

        private List<String[]> _table = new List<String[]>();
        String SheetName = "";
        String BreakRow = "";
        Nullable<int> SheetIdx = null;
        Nullable<int> StartRow = null;
        Nullable<int> EndRow = null;
        Nullable<int> StartColumn = null;
        Nullable<int> EndColumn = null;
        public String ErrorMessage = "";

        public List<String[]> Table
        {
            get
            {
                return _table;
            }
        }

        public MyExcel(byte[] bytes, Boolean isXLSX)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            if (isXLSX)
            {
                ReadX(ms);
            }
            else
            {
                Read(ms);
            }

        }
        public MyExcel(byte[] bytes, Boolean isXLSX,
            int SheetIdx,
            Nullable<int> StartRow,
            Nullable<int> EndRow,
            Nullable<int> StartColumn,
            Nullable<int> EndColumn)
        {
            this.SheetIdx = SheetIdx;
            this.StartRow = StartRow;
            this.EndRow = EndRow;
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            if (isXLSX)
            {
                ReadX(ms);
            }
            else
            {
                Read(ms);
            }

        }

        public MyExcel(string Path, Boolean isXLSX,
            string SheetName,
            Nullable<int> StartRow,
            Nullable<int> EndRow,
            Nullable<int> StartColumn,
            Nullable<int> EndColumn)
        {
            if (Path == null)
            {
                throw new ArgumentNullException("path");
            }
            this.SheetName = SheetName;
            this.StartRow = StartRow;
            this.EndRow = EndRow;
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;

            FileStream fs = new FileStream(Path, FileMode.Open);
            if (isXLSX)
            {
                ReadX(fs);
            }
            else
            {
                Read(fs);
            }
        }

        public MyExcel(Stream stream, Boolean isXLSX,
            int SheetIdx,
            Nullable<int> StartRow,
            String BreakRow,
            Nullable<int> StartColumn,
            Nullable<int> EndColumn)
        {
            this.SheetIdx = SheetIdx;
            this.StartRow = StartRow;
            this.BreakRow = BreakRow;
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;

            if (isXLSX)
            {
                ReadX(stream);
            }
            else
            {
                Read(stream);
            }
        }

        public MyExcel(Stream stream, Boolean isXLSX,
            int SheetIdx,
            Nullable<int> StartRow,
            Nullable<int> EndRow,
            Nullable<int> StartColumn,
            Nullable<int> EndColumn)
        {
            this.SheetIdx = SheetIdx;
            this.StartRow = StartRow;
            this.EndRow = EndRow;
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;

            if (isXLSX)
            {
                ReadX(stream);
            }
            else
            {
                Read(stream);
            }
        }

        public MyExcel(Stream stream, Boolean isXLSX,
            string SheetName,
            Nullable<int> StartRow,
            Nullable<int> EndRow,
            Nullable<int> StartColumn,
            Nullable<int> EndColumn)
        {
            this.SheetName = SheetName;
            this.StartRow = StartRow;
            this.EndRow = EndRow;
            this.StartColumn = StartColumn;
            this.EndColumn = EndColumn;

            if (isXLSX)
            {
                ReadX(stream);
            }
            else
            {
                Read(stream);
            }
        }
        public MyExcel(string path, Boolean isXLSX)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            FileStream fs = new FileStream(path, FileMode.Open);
            if (isXLSX)
            {
                ReadX(fs);
            }
            else
            {
                Read(fs);
            }
        }

        public MyExcel(Stream stream, Boolean isXLSX)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (isXLSX)
            {
                ReadX(stream);
            }
            else
            {
                Read(stream);
            }
        }

        private void ReadX(Stream s)
        {
            XSSFWorkbook hssfworkbook;
            try
            {
                hssfworkbook = new XSSFWorkbook(s);
                ISheet sheet = SheetIdx == null ? hssfworkbook.GetSheet(SheetName) : hssfworkbook.GetSheetAt((int)SheetIdx);
                Read(sheet);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                Console.WriteLine(
                    e.Message);
            }
        }

        private void Read(Stream s)
        {
            HSSFWorkbook hssfworkbook;
            try
            {
                hssfworkbook = new HSSFWorkbook(s);
                ISheet sheet = SheetIdx == null ? hssfworkbook.GetSheet(SheetName) : hssfworkbook.GetSheetAt((int)SheetIdx);
                if (sheet == null)
                {
                    ErrorMessage = " sheet : [" + (SheetIdx == null ? SheetName : "" + SheetIdx) + "] not exists in excel file.";
                }
                else
                {
                    Read(sheet);
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                Console.WriteLine(
                    e.Message);
            }
        }

        private void Read(ISheet sheet)
        {
            int i = 0;
            int j = 0;
            try
            {
                int IndexReadRow = (int)(StartRow == null ? 0 : StartRow);
                Boolean ReadNext = true;
                while (ReadNext)
                {
                    if (IndexReadRow > sheet.LastRowNum)
                    {
                        break;
                    }
                    IRow row = sheet.GetRow(IndexReadRow);
                    if (!String.IsNullOrEmpty(BreakRow))
                    {
                        if (BreakRow.Equals(getCellValue(row.GetCell(0))))
                        {
                            ReadNext = false;
                        }
                    }
                    else
                    {
                        if (IndexReadRow == (int)(EndRow == null ? sheet.LastRowNum + 1 : EndRow))
                        {
                            ReadNext = false;
                        }
                    }
                    if (ReadNext)
                    {
                        List<String> parsed = new List<String>();
                        string Data = string.Empty;
                        for (j = (int)(StartColumn == null ? 0 : StartColumn); j < (int)(EndColumn == null ? row.LastCellNum : EndColumn); j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                Data = getCellValue(row.GetCell(j));
                            }
                            else
                            {
                                Data = String.Empty;
                            }

                            parsed.Add(Data);
                        }
                        _table.Add(parsed.ToArray());
                    }
                    IndexReadRow++;
                    i = IndexReadRow;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                Console.WriteLine(
                    e.Message + "\n" +
                    "Row : " + i + "\n" +
                    "Col : " + j);
            }
        }

        private String getCellValue(ICell cell)
        {
            if (cell.CellType.Equals(CellType.Boolean))
            {
                return cell.BooleanCellValue ? "true" : "false";
            }
            if (cell.CellType.Equals(CellType.String))
            {
                return cell.StringCellValue;
            }
            if (cell.CellType.Equals(CellType.Numeric))
            {
                if (HSSFDateUtil.IsCellDateFormatted(cell))
                {
                    return cell.DateCellValue.ToShortDateString();
                }
                else
                {
                    return cell.NumericCellValue.ToString();
                }
            }
            if (cell.CellType.Equals(CellType.Formula))
            {
                if (cell.CachedFormulaResultType.Equals(CellType.Boolean))
                {
                    return cell.BooleanCellValue ? "true" : "false";
                }
                if (cell.CachedFormulaResultType.Equals(CellType.String))
                {
                    return cell.StringCellValue;
                }
                if (cell.CachedFormulaResultType.Equals(CellType.Numeric))
                {
                    return cell.NumericCellValue.ToString();
                }
                if (cell.CachedFormulaResultType.Equals(CellType.Formula))
                {
                    return cell.CachedFormulaResultType.ToString();
                }
                if (cell.CachedFormulaResultType.Equals(CellType.Blank))
                {
                    return "";
                }
                if (cell.CachedFormulaResultType.Equals(CellType.Error))
                {
                    return "#ERROR#";
                }
                if (cell.CachedFormulaResultType.Equals(CellType.Unknown))
                {
                    return "#Unknown#";
                }
            }
            //    }
            if (cell.CellType.Equals(CellType.Blank))
            {
                return "";
            }
            if (cell.CellType.Equals(CellType.Error))
            {
                return "#ERROR#";
            }
            if (cell.CellType.Equals(CellType.Unknown))
            {
                return "#Unknown#";
            }
            return "";
        }
    }
}
