using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.SQL
{
    public class SQLConn: IDisposable
    {

        #region variables

        private string _database;
        private string _servername;
        public SqlConnection conn;

        /// <summary>
        /// Gets or sets SQL連線字串
        /// </summary>
        /// <value>
        /// The SQL connection string.
        /// </value>
        private static string SQLConnString { get; set; }

        /// <summary>
        /// Gets 資料庫名稱
        /// </summary>
        /// <value>
        /// The data base.
        /// </value>
        public string DataBase
        {
            get
            {
                return _database;
            }
        }

        /// <summary>
        /// Gets Server名稱
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public string Server
        {
            get
            {
                return _servername;
            }
        }

        #endregion variables

        #region Constructors

        /// <summary>
        /// 建立SQL連線
        /// </summary>
        /// <param name="db">資料庫名稱</param>
        /// <param name="svr">SQL Server名稱</param>
        public SQLConn(string db, string svr)
        {
            this._database = db;
            this._servername = svr;
            SQLConnString = "DATABASE=" + this.DataBase +
                            "; SERVER=" + this.Server +
                            "; Trusted_Connection=True " +
                            "; MultipleActiveResultSets=true " +
                            "; Min Pool Size=" + System.Environment.ProcessorCount +
                            "; Connection Timeout=18000 ";

            if ((conn = _conn()) == null)
            {
                this.Dispose();
            }
        }

        #endregion Constructors



        #region Public Function

        /// <summary>
        /// 結束SQL連線
        /// </summary>
        public void Dispose()
        {
            if (conn != null)
            {
                conn.Dispose();
            }
        }

        #endregion Public Function



        #region Private Function

        /// <summary>
        /// SQL連線
        /// </summary>
        /// <returns></returns>
        private SqlConnection _conn()
        {
            SqlConnection cnn = null;

            string sConnectionString = SQLConnString;
            cnn = new SqlConnection(sConnectionString);
            cnn.Open();
            return cnn;
        }

        #endregion Private Function
    }
}
