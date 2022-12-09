using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.Function
{
    public class Function_File
    {
        public static void copyFile(string saveFileName)
        {
            string fileName = "報名文件.xlsx";
            string sourcePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "//Model//";
            string targetPath = saveFileName;
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = targetPath;

            //if (!System.IO.Directory.Exists(targetPath))
            //{
            //    System.IO.Directory.CreateDirectory(targetPath);
            //}

            System.IO.File.Copy(sourceFile, destFile, true);
        }

    }
}
