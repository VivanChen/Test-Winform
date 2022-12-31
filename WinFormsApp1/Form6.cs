using Spire.Doc;
using Spire.Pdf;
using Spire.Pdf.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.Controller;
using WinFormsApp1.Function;
using WinFormsApp1.Model;
using WinFormsApp1.SQL;

namespace WinFormsApp1
{
    public partial class Form6 : Form
    {
        AccountsController AccountController = new AccountsController();
        RegisterController RegisterController = new RegisterController();
        public List<Register> registers { get; set; }
        public DataTable Data { get; set; }
        public Accounts Account { get; set; }
        public string SaveFileName { get; set; }
        //public Form6(Accounts Accounts_souce)
        //{
        //    Account = Accounts_souce;
        //    InitializeComponent();
        //}

        public Form6()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 下載EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form6_Load(object sender, EventArgs e)
        {
            //if (!AccountController.Validate_account(Account, out string message))
            //{
            //    MessageBox.Show("帳號密碼錯誤，請重新登入");
            //    Form3 f = new Form3();
            //    this.Hide();
            //    f.ShowDialog();
            //    this.Dispose();
            //}
            //else label1.Text = "您好 : " + Account.Account;

        }
        /// <summary>
        /// 上傳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (Data == null || Data.Rows.Count == 0) { MessageBox.Show("請先執行匯入動作"); return; }
            else
            {
                DataTable Tamp = Data.Copy();

                if (SQLCmd.SQLBulkInsert("Registration_master", Data, out string message)) { MessageBox.Show("上傳成功"); }
                else MessageBox.Show(message);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            SetCheckColumn();
            Button_Close();
            Function_Excel function_Excel = new Function_Excel();
            DataTable dtt = function_Excel.GetDataTableFromExcelFile("", out string SaveFileName);
            this.SaveFileName = SaveFileName;
            if (dtt == null)
            {
                return;
            }
            Data = null;
            Data = dtt;
            Button_Open();
            dataGridView1.DataSource = Data;

        }

        private void SetCheckColumn()
        {
            DataGridViewCheckBoxColumn cbCol = new DataGridViewCheckBoxColumn();
            cbCol.Width = 50;   //設定寬度
            cbCol.HeaderText = "　全選";
            cbCol.Name = "Select";
            cbCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;   //置中
            dataGridView1.Columns.Insert(0, cbCol);
            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = dataGridView1.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 9;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 9);

            CheckBox cbHeader = new CheckBox();
            cbHeader.Name = "checkboxHeader";
            cbHeader.Size = new Size(18, 18);
            cbHeader.Location = rect.Location;
            //全選要設定的事件
            cbHeader.CheckedChanged += new EventHandler(cbHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dataGridView1.Controls.Add(cbHeader);
            dataGridView1.Columns[0].ReadOnly = false;
        }

        private void cbHeader_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridView1.Rows)
                dr.Cells[0].Value = ((CheckBox)dataGridView1.Controls.Find("checkboxHeader", true)[0]).Checked;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //保存文件
            Button_Close();
            Stopwatch sw = null;
            string saveFileName = "";
            string fileName = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xlsx";
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Excel文件保存路徑";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //**程序開始計時**//
                sw = new Stopwatch();
                sw.Start();

                saveFileName = saveFileDialog.FileName;
                Function_File.copyFile(saveFileName);
                //**程序結束計時**//
                sw.Stop();
                double totalTime = sw.ElapsedMilliseconds / 1000.0;

                MessageBox.Show(fileName + " 導出成功\n耗時" + totalTime + "s", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Button_Open();
            }
            else
            {
                Button_Open();
                return;
            }
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            string DIR_Temp = Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]);
            string DIR_ExportPDF = Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_ExportPDF"]);
            string DIR_Mode = Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Mode"]);
            SemaphoreSlim semaphore = new SemaphoreSlim(1);
            // 目錄不存在就要建立
            RegisterController.CreateFolder(DIR_Temp);
            RegisterController.CreateFolder(DIR_ExportPDF);

            Stopwatch sw = new Stopwatch();
            //**程序開始計時**//
            sw = new Stopwatch();
            sw.Start();
            int totalTasks = dataGridView1.Rows.Count;  // 總共要處理的任務數
            int completedTasks = 0;  // 已完成的任務數
            label2.Visible = true;
            label3.Visible = true;
            // 使用非同步的方式來執行迴圈
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() =>
            {
                // 使用多執行緒來加速轉檔
                ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 8 };
                Parallel.For(0, dataGridView1.Rows.Count, options, async i =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        if ((bool)dataGridView1.Rows[i].Cells[0].EditedFormattedValue == true)
                        {
                            if (dataGridView1.Rows[i].Cells[1].Value != null)
                            {
                                // 檢查 Data.Rows[i] 是否有值，若無值或為 null 則跳過該筆繼續執行
                                if (Data.Rows[i] == null || Data.Rows[i]["Type"] == null || Data.Rows[i]["TextId"] == null)
                                {
                                    this.Invoke(new MethodInvoker(() =>
                                    {
                                        MessageBox.Show("無法取得值，跳過該筆繼續執行。", "警告", MessageBoxButtons.OK);
                                    }));
                                    return;
                                }

                                // 使用非同步的方式來執行耗時的函式
                                var docxBytes = await Task.Run(() => RegisterController.CommonExport_Word(DIR_Mode, "word1.docx", Data.Rows[i]));
                                // 將 docxBytes 寫入檔案
                                string fileName = $"套表測試-{i}-{DateTime.Now:HHmmss}.docx";
                                string filePath = Path.Combine(DIR_Temp, fileName);
                                File.WriteAllBytes(filePath, docxBytes);

                                // 建立文件物件並載入檔案
                                Document doc = new Document();
                                doc.LoadFromFile(filePath);

                                // 儲存為 PDF 檔案
                                doc.SaveToFile(Path.Combine(DIR_Temp, "test.pdf"), Spire.Doc.FileFormat.PDF);

                                // 加密 PDF 檔案
                                EncryptPDF(Path.Combine(DIR_Temp, "test.pdf"),
                                           Path.Combine(DIR_ExportPDF, Data.Rows[i]["Type"] + "-" + Data.Rows[i]["TextId"] + $"-{ DateTime.Now:HHmmss}.pdf"),
                                           Data.Rows[i]["TextId"].ToString());

                                // 刪除臨時目錄
                                RegisterController.DeleteFolder(DIR_Temp);
                                // 計算已完成的任務數
                                Interlocked.Increment(ref completedTasks);
                                // 更新進度條的值
                                int progress = (int)((float)completedTasks / totalTasks * 100);
                                this.Invoke(new MethodInvoker(() =>
                                {
                                    progressBar1.Value = progress;
                                    label2.Text = progress + "%";
                                    label3.Text = "("+completedTasks+"/"+totalTasks+")";
                                }));
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                });
            }));

            if (completedTasks== totalTasks-1)
            {
                //**程序結束計時**//
                sw.Stop();
                // 顯示處理時間
                this.Invoke(new MethodInvoker(() =>
                {
                    MessageBox.Show($"共處理 {completedTasks} 筆資料，共花費 {sw.Elapsed.TotalSeconds} 秒。", "訊息", MessageBoxButtons.OK);
                }));
            }
            
        }


        //private void button2_Click(object sender, EventArgs e)
        //{
        //    // 目錄不存在就要建立
        //    RegisterController.CreateFolder(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]));
        //    RegisterController.CreateFolder(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_ExportPDF"]));
        //    Stopwatch sw = null;
        //    //**程序開始計時**//
        //    sw = new Stopwatch();
        //    sw.Start();
        //    for (int i = 0; i < dataGridView1.Rows.Count; i++)
        //    {
        //        if ((bool)dataGridView1.Rows[i].Cells[0].EditedFormattedValue == true)
        //        {
        //            if (dataGridView1.Rows[i].Cells[1].Value!=null)
        //            {
        //                File.WriteAllBytes(
        //                         Path.Combine(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]), $"套表測試-{DateTime.Now:HHmmss}.docx"),
        //                         RegisterController.CommonExport_Word(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Mode"]), "word1.docx", Data.Rows[i]));
        //                Document doc = new Document();
        //                DirectoryInfo info = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]));
        //                FileInfo[] files = info.GetFiles();
        //                foreach (FileInfo item in files)
        //                {
        //                    doc.LoadFromFile(item.FullName);
        //                    ToPdfParameterList topdf = new ToPdfParameterList();

        //                    doc.SaveToFile(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"], "test.pdf"), Spire.Doc.FileFormat.PDF);
        //                    EncryptPDF(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"], "test.pdf"),
        //                               Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_ExportPDF"], Data.Rows[i]["Type"] + "-" + Data.Rows[i]["TextId"] + $"-{ DateTime.Now:HHmmss}.pdf"),
        //                               Data.Rows[i]["TextId"].ToString());
        //                    RegisterController.DeleteFolder(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]));
        //                }
        //            }

        //        }
        //    }
        //    //**程序結束計時**//
        //    sw.Stop();
        //    double totalTime = sw.ElapsedMilliseconds / 1000.0;
        //    MessageBox.Show(" 導出成功\n耗時" + totalTime + "s", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}
        private void button3_Click(object sender, EventArgs e)
        {
            // 取得資料夾下的所有 PDF 檔案清單
            string[] pdfFilePaths = Directory.GetFiles(ConfigurationManager.AppSettings["DIR_ExportPDF"], "*.pdf");
            foreach (var item in pdfFilePaths)
            {
                //載入PDF文件
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(item, item.Substring(21, 10));
                //使用預設印表機列印文件所有頁面
                doc.Print();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataTable dtt = new DataTable();
            string script = "select  * from  Registration_master";
            dtt = SQLCmd.ExecuteDataRows(script, out string message);
            dataGridView1.DataSource = dtt;
        }
        /// <summary>
        /// pdf加密
        /// </summary>
        /// <param name="SrcPath">來源</param>
        /// <param name="OutPath">輸出</param>
        /// <param name="UserPw">user密碼</param>
        /// <param name="pmss">權限(ex. PdfWriter.ALLOW_SCREENREADERS)</param>
        public static void EncryptPDF(string SrcPath, string OutPath, string UserPw)
        {
            //建立PdfDocument類的物件，並載入測試文件
            using (PdfDocument pdf = new PdfDocument())
            {
                pdf.LoadFromFile(SrcPath);
                //設定開啟文件的口令以及許可口令
                pdf.Security.Encrypt("open", UserPw, PdfPermissionsFlags.Print | PdfPermissionsFlags.CopyContent, PdfEncryptionKeySize.Key128Bit);
                //儲存加密後的文件
                pdf.SaveToFile(OutPath, Spire.Pdf.FileFormat.PDF);
            }
        }
        /// <summary>
        /// 關閉按鈕
        /// </summary>
        private void Button_Close()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
        }
        /// <summary>
        /// 打開按鈕
        /// </summary>
        private void Button_Open()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void 保險業務人員報名系統ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 台北市各級學校查詢ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 台灣商家黃頁ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 天氣查詢ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 旅遊景點查詢ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 登出ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 退出程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
