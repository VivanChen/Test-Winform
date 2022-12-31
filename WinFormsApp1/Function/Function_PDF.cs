using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace WinFormsApp1.Function
{
    public class Function_PDF
    {
        /// <summary>

        /// 利用LibraOffice將Doc轉成PDF
        /// </summary>
        /// <param name=”openOfficePath”>soffice.exe的路徑</param>
        /// <param name=”workDir”>要被轉換檔案的資料夾位置</param>
        /// <param name=”docFileName”>要被轉換檔案的名稱</param>
        /// <returns></returns>
        private bool convertDocToPdf(String openOfficePath, String workDir, String docFileName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = openOfficePath;
            startInfo.WorkingDirectory = workDir;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "–headless –convert - to pdf "+docFileName;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
