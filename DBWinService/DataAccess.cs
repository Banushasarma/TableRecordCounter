using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.IO;

namespace DBWinService
{
    public class DataAccess
    {
        public void GetResults()
        {
            DataSet dataSet = new DataSet("TableCount");
            string TableNameString = ConfigurationManager.AppSettings["TableNames"];
            string[] TableNameArr = TableNameString.Split(',');


            foreach (string tabName in TableNameArr)
            {
                DataTable dataTableServer1 = new DataTable(tabName);
                DataTable dataTableServer2 = new DataTable(tabName);
                DataTable mergedTable = new DataTable(tabName);
                dataTableServer1 = GetRecordCount(tabName, "Server1");
                dataTableServer2 = GetRecordCount(tabName, "Server2");

                mergedTable = CombineDataTable(dataTableServer1, dataTableServer2);

                if (ConfigurationManager.AppSettings["ExportFileFormat"] == "csv")
                {
                    string Destination = ConfigurationManager.AppSettings["DestinationToExportExcel"];
                    string fileName = String.Format(@"{0}\{1}_RecordCount_{2}.csv", Destination, tabName, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_'));
                    ExportDataTabletoFile(mergedTable, ",", true, fileName);
                }
                if (ConfigurationManager.AppSettings["ExportFileFormat"] == "xls")
                {
                    dataSet.Tables.Add(mergedTable);
                }
            }
            if (ConfigurationManager.AppSettings["ExportFileFormat"] == "xls")
            {
                ExportDataSetToExcel(dataSet);
            }
        }

        public DataTable GetRecordCount(string TableName, string ServerName)
        {
            DataTable dt = new DataTable(TableName);
            SqlConnection sqlConn = new DBConnectivity().GetSqlConnection(ServerName);
            string cmdStr = "spGetRowCount";
            SqlCommand sqlCommand = new SqlCommand(cmdStr, sqlConn);
            sqlCommand.Parameters.AddWithValue("@TableName", TableName);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            using (var da = new SqlDataAdapter(sqlCommand))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                da.Fill(dt);
            }

            return dt;
        }

        private void ExportDataSetToExcel(DataSet ds)
        {
            string Destination = ConfigurationManager.AppSettings["DestinationToExportExcel"];
            ExcelLibrary.DataSetHelper.CreateWorkbook(String.Format(@"{0}\RecordCount_{1}.xls", Destination, DateTime.Now.ToString().Replace('/', '_').Replace(':', '_')), ds);
        }

        public DataTable CombineDataTable(DataTable dt1, DataTable dt2)
        {
            DataTable dtNew = new DataTable();
            foreach (DataColumn col in dt1.Columns)
            {
                dtNew.Columns.Add(col.ColumnName);
            }
            foreach (DataColumn col in dt2.Columns)
            {
                if (col.ColumnName != "Schema" && col.ColumnName != "Table")
                {
                    dtNew.Columns.Add(col.ColumnName);
                }
            }

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow item = dtNew.NewRow();
                foreach (DataColumn col in dt1.Columns)
                {
                    item[col.ColumnName] = dt1.Rows[i][col.ColumnName];
                }
                foreach (DataColumn col in dt2.Columns)
                {
                    item[col.ColumnName] = dt2.Rows[i][col.ColumnName];
                }
                dtNew.Rows.Add(item);
                dtNew.AcceptChanges();
            }
            return dtNew;
        }

        public void ExportDataTabletoFile(DataTable datatable, string delimited, bool exportcolumnsheader, string file)
        {
            StreamWriter str = new StreamWriter(file, false, System.Text.Encoding.Default);
            if (exportcolumnsheader)
            {
                string Columns = string.Empty;
                foreach (DataColumn column in datatable.Columns)
                {
                    Columns += column.ColumnName + delimited;
                }

                str.WriteLine(Columns.Remove(Columns.Length - 1, 1));
            }

            foreach (DataRow datarow in datatable.Rows)
            {
                string row = string.Empty;
                foreach (object items in datarow.ItemArray)
                {
                    row += items.ToString() + delimited;
                }
                str.WriteLine(row.Remove(row.Length - 1, 1));
            }
            str.Flush();
            str.Close();
        }
    }
}
