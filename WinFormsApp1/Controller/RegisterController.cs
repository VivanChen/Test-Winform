using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Function;
using WinFormsApp1.Model;

namespace WinFormsApp1.Controller
{
    public class RegisterController : Register
    {
        public static void CreateFolder(string foldername)
        {
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
        }

        public byte[] CommonExport_Word(string SrcPath, string file_name,DataRow row)
        {
             var docxBytes = Function_Word.GenerateDocx(File.ReadAllBytes(Path.Combine(SrcPath, file_name)),
                 new Dictionary<string, string>()
                 {
                     ["Type"] = row["Type"].ToString(),
                     ["Number"] = row["Number"].ToString(),
                     ["Testdate"] = row["Testdate"].ToString(),
                     ["Testtime"] = row["Testtime"].ToString(),
                     ["Unit"] = row["Unit"].ToString(),
                     ["Year"] = row["Year"].ToString(),
                     ["Region"] = row["Region"].ToString(),
                     ["Name"] = row["Name"].ToString(),
                     ["TestField"] = row["TestField"].ToString(),
                     ["Birthday"] = row["Birthday"].ToString(),
                     ["SeatNumber"] = row["SeatNumber"].ToString(),
                     ["TextId"] = row["TextId"].ToString(),
                     ["CompanyCode"] = row["CompanyCode"].ToString(),
                     ["Location"] = row["Location"].ToString(),
                     ["Address"] = row["Address"].ToString(),
                 });
            return docxBytes;
        }
        public static void DeleteFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectory in di.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }
            foreach (DirectoryInfo subDirectory in di.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }
        }




    }
}
