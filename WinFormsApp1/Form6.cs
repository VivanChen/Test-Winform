using Spire.Doc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
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
        SQLConn conn = new SQLConn(Commons.DataBase, Commons.Server);
        AccountsController AccountController = new AccountsController();
        public List<Register> registers { get; set; }
        public DataTable Data { get; set; }
        public Accounts Account { get; set; }
        public string SaveFileName { get; set; }
        public Form6(Accounts Accounts_souce)
        {
            Account = Accounts_souce;
            InitializeComponent();
        }
        /// <summary>
        /// 下載EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form6_Load(object sender, EventArgs e)
        {
            if (!AccountController.Validate_account(Account, out string message))
            {
                MessageBox.Show("帳號密碼錯誤，請重新登入");
                Form3 f = new Form3();
                this.Hide();
                f.ShowDialog();
                this.Dispose();
            }
            else label1.Text = "您好 : " + Account.Account;
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
                if (SQLCmd.SQLBulkInsert("Registration_master", Data, out string message)) { MessageBox.Show("上傳成功"); }
                else MessageBox.Show(message);
            }         
        }
        private void button6_Click(object sender, EventArgs e)
        {
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

        private void button2_Click(object sender, EventArgs e)
        {
            // 目錄不存在就要建立
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"])))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]));
            }
            var docxBytes = Function_Word.GenerateDocx(File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Mode"], "word1.docx")),
                new Dictionary<string, string>()
                {
                    ["Type"] = Data.Rows[0]["Type"].ToString(),
                    ["Number"] = Data.Rows[0]["Number"].ToString(),
                    ["Testdate"] = Data.Rows[0]["Testdate"].ToString(),
                    ["Testtime"] = Data.Rows[0]["Testtime"].ToString(),
                    ["Unit"] = Data.Rows[0]["Unit"].ToString(),
                    ["Year"] = Data.Rows[0]["Year"].ToString(),
                    ["Region"] = Data.Rows[0]["Region"].ToString(),
                    ["Name"] = Data.Rows[0]["Name"].ToString(),
                    ["TestField"] = Data.Rows[0]["TestField"].ToString(),
                    ["Birthday"] = Data.Rows[0]["Birthday"].ToString(),
                    ["SeatNumber"] = Data.Rows[0]["SeatNumber"].ToString(),
                    ["TextId"] = Data.Rows[0]["TextId"].ToString(),
                    ["CompanyCode"] = Data.Rows[0]["CompanyCode"].ToString(),
                    ["Location"] = Data.Rows[0]["Location"].ToString(),
                    ["Address"] = Data.Rows[0]["Address"].ToString(),


                }); ; ;
            File.WriteAllBytes(
                Path.Combine(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]), $"套表測試-{DateTime.Now:HHmmss}.docx"),
                docxBytes);

            Document doc = new Document();
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, ConfigurationManager.AppSettings["DIR_Temp"]));
            FileInfo[] files = info.GetFiles();
            foreach (FileInfo item in files)
            {
                doc.LoadFromFile(item.FullName);
                PrintDocument printDoc = doc.PrintDocument;
                printDoc.Print();
            }

        }
    }
}
