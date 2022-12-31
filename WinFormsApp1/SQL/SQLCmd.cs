using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1.SQL
{
    public class SQLCmd
    {
		protected SqlConnection sqlConn { get; set; }

		/// <summary>
		/// 把SQL Command結果
		/// 放置DataTable
		/// </summary>
		/// <param name="cmd">SQL command.</param>
		/// <param name="message">回傳錯誤訊息</param>
		/// <returns>DataTable</returns>
		public static DataTable ExecuteDataRows(string sqlscript, Dictionary<string, string> parameterlist, out string message)
		{
			message = string.Empty;
			SqlCommand cmd = SetSqlCommand(sqlscript, parameterlist, out message);

			if (message != string.Empty)
			{
				cmd.Connection.Close();
				cmd.Connection.Dispose();
				return null;
			}

			return ExecuteDataRows(cmd, out message);
		}
		public static DataTable ExecuteDataRows(string sqlscript, out string message)
		{
			message = string.Empty;
			SqlCommand cmd = SetSqlCommand(sqlscript, out message);

			if (message != string.Empty)
			{
				cmd.Connection.Close();
				cmd.Connection.Dispose();
				return null;
			}

			return ExecuteDataRows(cmd, out message);
		}
		private static SqlCommand SetSqlCommand(string sqlscript,  out string message)
		{
			message = string.Empty;

			SqlCommand cmd = new SqlCommand();

			try
			{
				SQLConn conn = new SQLConn(Commons.DataBase, Commons.Server);
				cmd.Connection = conn.conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 18000;
				cmd.CommandText = sqlscript;
			}
			catch (SqlException e)
			{
				message = e.Message;
			}

			return cmd;
		}
		private static SqlCommand SetSqlCommand(string sqlscript, Dictionary<string, string> parameterlist, out string message)
		{
			message = string.Empty;

			SqlCommand cmd = new SqlCommand();

			try
			{
				SQLConn conn = new SQLConn(Commons.DataBase, Commons.Server);
				cmd.Connection = conn.conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = 18000;
				cmd.CommandText = sqlscript;

				foreach (KeyValuePair<string, string> item in parameterlist)
				{
					cmd.Parameters.AddWithValue(item.Key, item.Value);
				}
			}
			catch (SqlException e)
			{
				message = e.Message;
			}

			return cmd;
		}
		public static DataTable ExecuteDataRows(SqlCommand cmd, out string message)
		{
			message = string.Empty;
			DataTable dt = new DataTable();

			using (SqlDataAdapter da = new SqlDataAdapter(cmd))
			{
				try
				{
					if (dt != null)
					{
						da.Fill(dt);
					}
				}
				catch (SqlException ex)
				{
					message = ex.Message;
				}
			}

			cmd.Connection.Close();
			cmd.Connection.Dispose();
			return dt;
		}
		public static bool SQLBulkInsert(string DestinationTbl, DataTable dtInsertRows, out string message)
		{
			bool blresult = false;
			message = string.Empty;
			SQLConn conn = new SQLConn(Commons.DataBase, Commons.Server);

			try
			{
				using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn.conn))
				{

					sqlBC.BulkCopyTimeout = 0;

					sqlBC.DestinationTableName = DestinationTbl;

					// Number of records to be processed in one go
					sqlBC.BatchSize = 2000;

					// Add your column mappings here
					foreach (DataColumn c in dtInsertRows.Columns)
					{
						sqlBC.ColumnMappings.Add(c.ColumnName, c.ColumnName);
					}

					// Finally write to server
					sqlBC.WriteToServer(dtInsertRows);

					blresult = true;
				}
			}
			catch (SqlException ex)
			{
				message = ex.Message;
			}
			finally
			{
				conn.conn.Close();
				conn.conn.Dispose();
			}

			return blresult;
		}
	}
}
