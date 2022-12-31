using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1.Function
{
     public class Function_Excel
    {
        #region  NPOI  導出 EXCEL
        /// <summary>
        /// NPOI  導出 EXCEL
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dgv"></param>
        /// <param name="fontname"></param>
        /// <param name="fontsize"></param>
        public void ExportExcel(string fileName, DataGridView dgv, string fontname, short fontsize)
        {
            IWorkbook workbook;
            ISheet sheet;
            Stopwatch sw = null;

            //判斷datagridview中內容是否為空
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("DataGridView中內容為空,請先導入數據!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //保存文件
            string saveFileName = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Excel文件保存路徑";
            saveFileDialog.FileName = fileName;
            MemoryStream ms = new MemoryStream(); //MemoryStream
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //**程序開始計時**//
                sw = new Stopwatch();
                sw.Start();

                saveFileName = saveFileDialog.FileName;

                //檢測文件是否被佔用
                if (!CheckFiles(saveFileName))
                {
                    MessageBox.Show("文件被佔用,請關閉文件" + saveFileName);
                    workbook = null;
                    ms.Close();
                    ms.Dispose();
                    return;
                }
            }
            else
            {
                workbook = null;
                ms.Close();
                ms.Dispose();
            }

            //*** 根據擴展名xls和xlsx來創建對象
            string fileExt = Path.GetExtension(saveFileName).ToLower();
            if (fileExt == ".xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileExt == ".xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = null;
            }
            //***

            //創建Sheet
            if (workbook != null)
            {
                sheet = workbook.CreateSheet("Sheet1");//Sheet的名稱  
            }
            else
            {
                return;
            }

            //設置單元格樣式
            ICellStyle cellStyle = workbook.CreateCellStyle();
            //水平居中對齊和垂直居中對齊
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            //設置字體
            IFont font = workbook.CreateFont();
            font.FontName = fontname;//字體名稱
            font.FontHeightInPoints = fontsize;//字號
            font.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;//字體顏色
            cellStyle.SetFont(font);

            //添加列名
            IRow headRow = sheet.CreateRow(0);
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                //隱藏行列不導出
                if (dgv.Columns[i].Visible == true)
                {
                    headRow.CreateCell(i).SetCellValue(dgv.Columns[i].HeaderText);
                    headRow.GetCell(i).CellStyle = cellStyle;
                }
            }

            //根據類型寫入內容
            for (int rowNum = 0; rowNum < dgv.Rows.Count; rowNum++)
            {
                ///跳過第一行,第一行為列名
                IRow dataRow = sheet.CreateRow(rowNum + 1);
                for (int columnNum = 0; columnNum < dgv.Columns.Count; columnNum++)
                {
                    int columnWidth = sheet.GetColumnWidth(columnNum) / 256; //列寬

                    //隱藏行列不導出
                    if (dgv.Rows[rowNum].Visible == true && dgv.Columns[columnNum].Visible == true)
                    {
                        //防止行列超出Excel限制
                        if (fileExt == ".xls")
                        {
                            //03版Excel最大行數是65536行,最大列數是256列
                            if (rowNum > 65536)
                            {
                                MessageBox.Show("行數超過Excel限制!");
                                return;
                            }
                            if (columnNum > 256)
                            {
                                MessageBox.Show("列數超過Excel限制!");
                                return;
                            }
                        }
                        else if (fileExt == ".xlsx")
                        {
                            //07版Excel最大行數是1048576行,最大列數是16384列
                            if (rowNum > 1048576)
                            {
                                MessageBox.Show("行數超過Excel限制!");
                                return;
                            }
                            if (columnNum > 16384)
                            {
                                MessageBox.Show("列數超過Excel限制!");
                                return;
                            }
                        }

                        ICell cell = dataRow.CreateCell(columnNum);
                        if (dgv.Rows[rowNum].Cells[columnNum].Value == null)
                        {
                            cell.SetCellType(CellType.Blank);
                        }
                        else
                        {
                            if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.Int32"))
                            {
                                cell.SetCellValue(Convert.ToInt32(dgv.Rows[rowNum].Cells[columnNum].Value));
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.String"))
                            {
                                cell.SetCellValue(dgv.Rows[rowNum].Cells[columnNum].Value.ToString());
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.Single"))
                            {
                                cell.SetCellValue(Convert.ToSingle(dgv.Rows[rowNum].Cells[columnNum].Value));
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.Double"))
                            {
                                cell.SetCellValue(Convert.ToDouble(dgv.Rows[rowNum].Cells[columnNum].Value));
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.Decimal"))
                            {
                                cell.SetCellValue(Convert.ToDouble(dgv.Rows[rowNum].Cells[columnNum].Value));
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.DateTime"))
                            {
                                cell.SetCellValue(Convert.ToDateTime(dgv.Rows[rowNum].Cells[columnNum].Value).ToString("yyyy-MM-dd"));
                            }
                            else if (dgv.Rows[rowNum].Cells[columnNum].ValueType.FullName.Contains("System.DBNull"))
                            {
                                cell.SetCellValue("");
                            }
                        }
                        //設置列寬
                        IRow currentRow;
                        if (sheet.GetRow(rowNum) == null)
                        {
                            currentRow = sheet.CreateRow(rowNum);
                        }
                        else
                        {
                            currentRow = sheet.GetRow(rowNum);
                        }

                        if (currentRow.GetCell(columnNum) != null)
                        {
                            ICell currentCell = currentRow.GetCell(columnNum);
                            int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;

                            if (columnWidth < length)
                            {
                                columnWidth = length + 10; //設置列寬數值
                            }
                        }
                        sheet.SetColumnWidth(columnNum, columnWidth * 256);

                        //單元格樣式
                        dataRow.GetCell(columnNum).CellStyle = cellStyle;
                    }
                }
            }

            //保存为Excel文件                  
            workbook.Write(ms);
            FileStream file = new FileStream(saveFileName, FileMode.Create);
            workbook.Write(file);
            file.Close();
            workbook = null;
            ms.Close();
            ms.Dispose();

            //**程序結束計時**//
            sw.Stop();
            double totalTime = sw.ElapsedMilliseconds / 1000.0;

            MessageBox.Show(fileName + " 導出成功\n耗時" + totalTime + "s", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        #region 檢測文件是否被佔用
        /// <summary>
        /// 判定文件是否打開
        /// </summary>  
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        /// <summary>
        /// 檢測文件被佔用
        /// </summary>
        /// <param name="FileNames">要檢測的文件路徑</param>
        /// <returns></returns>
        public bool CheckFiles(string FileNames)
        {
            if (!File.Exists(FileNames))
            {
                //文件不存在
                return true;
            }
            IntPtr vHandle = _lopen(FileNames, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                //文件被佔用
                return false;
            }
            //文件沒被佔用
            CloseHandle(vHandle);
            return true;
        }
        #endregion
        /// <summary>
        /// 讀取Excel 匯出Datatable
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="SaveFileName"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromExcelFile(string fileName, out string SaveFileName)
        {
            string saveFileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "xls";
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "Excel文件保存路徑";
            openFileDialog.FileName = fileName;
            MemoryStream ms = new MemoryStream(); //MemoryStream
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveFileName = openFileDialog.FileName;
                fileName = saveFileName;
            }
            SaveFileName = saveFileName;

            FileStream fs = null;
            DataTable dt = new DataTable();
            try
            {
                IWorkbook wb = null;

                //檢測文件是否被佔用
                Function_Excel exportDgvToExcel = new Function_Excel();
                if (!exportDgvToExcel.CheckFiles(saveFileName))
                {
                    MessageBox.Show("文件被佔用,請關閉文件" + saveFileName);
                    return dt;
                }
                fs = File.Open(fileName, FileMode.Open, FileAccess.Read);
                switch (Path.GetExtension(fileName).ToUpper())
                {
                    case ".XLS":
                        {
                            wb = new HSSFWorkbook(fs);
                        }
                        break;
                    case ".XLSX":
                        {
                            wb = new XSSFWorkbook(fs);
                        }
                        break;
                }
                if (wb.NumberOfSheets > 0)
                {
                    ISheet sheet = wb.GetSheetAt(0);
                    IRow headerRow = sheet.GetRow(0);
                    //IRow CheckRow = sheet.GetRow(1);
                    //var a = CheckRow.GetCell(22).ToString().ToUpper();
                    //if (a != ConfigurationManager.AppSettings["Ver"].ToUpper())
                    //{
                    //    MessageBox.Show("Excel版本有誤，錯誤版本為: " + a + " ，正確版本為: " + ConfigurationManager.AppSettings["Ver"]);
                    //    dt = null;
                    //    return dt;
                    //}



                    //處理標題列
                    for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
                    {
                        dt.Columns.Add(headerRow.GetCell(i).StringCellValue.Trim());
                    }
                    IRow row = null;
                    DataRow dr = null;
                    CellType ct = CellType.Unknown;
                    //標題列之後的資料
                    for (int i = sheet.FirstRowNum+1 ; i <= sheet.LastRowNum; i++)
                    {
                        dr = dt.NewRow();
                        row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (string.IsNullOrEmpty(row.Cells[0].ToString())) continue;
                       
                        for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                        {
                            if (row.GetCell(j) == null)
                            {
                                dr[j] = "";
                            }
                            else
                            {
                                ct = row.GetCell(j).CellType;
                                switch (ct.ToString())
                                {
                                    case "Unknown":
                                        dr[j] = row.GetCell(j).ToString().Replace("$", "");
                                        break;
                                    case "Numeric":
                                        if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                        {
                                            dr[j] = row.GetCell(j).DateCellValue.ToString("yyyy/MM/dd");
                                        }
                                        else
                                        {
                                            dr[j] = row.GetCell(j).NumericCellValue;
                                        }
                                        break;
                                    case "String":
                                        dr[j] = row.GetCell(j).ToString().Replace("$", "");
                                        break;
                                    case "Formula":
                                        ct = row.GetCell(j).CachedFormulaResultType;
                                        break;
                                    case "Blank":
                                        dr[j] = row.GetCell(j).ToString().Replace("$", "");
                                        break;
                                    case "Boolean":
                                        dr[j] = row.GetCell(j).ToString().Replace("$", "");
                                        break;
                                    case "Error":
                                        dr[j] = row.GetCell(j).ToString().Replace("$", "");
                                        break;
                                     
                                }                                
                            }
                        }
                        dt.Rows.Add(dr);
                    }

                    //新增ID
                    dt.Columns.Add("ID");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["ID"].ToString())) { dt.Rows[i]["ID"] = DateTime.Now.ToString("yyMMddHHmmssff") + i+1.ToString().PadLeft(5, '0'); };
                    }

                }
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
           }
            finally
            {
                if (fs != null) fs.Dispose();
            }
            return dt;
        }
    }
}
