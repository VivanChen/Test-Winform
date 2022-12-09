using System.Configuration;

namespace WinFormsApp1.SQL
{
    public class Commons
    {
		/// <summary>
		/// 連線資料庫名稱.
		/// </summary>
		/// <value>
		/// 連線資料庫名稱.
		/// </value>
		public static string DataBase
		{
			get
			{				
				return ConfigurationManager.AppSettings["DataBase"];
			}
		}

		/// <summary>
		/// 連線資料庫主機名稱.
		/// </summary>
		/// <value>
		/// 連線資料庫主機名稱.
		/// </value>
		public static string Server
		{
			get
			{
				return ConfigurationManager.AppSettings["Server"];
			}
		}

	}
}
