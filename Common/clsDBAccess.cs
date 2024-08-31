using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Group_Project.Common
{
    internal class clsDBAccess
    {
        #region Class Attributes
        /// <summary>
        /// String to hold the address of the database
        /// </summary>
        private string sConnectionString;
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor that creates the link to the database
        /// </summary>
        public clsDBAccess()
        {
            try
            {
                string dbFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
                string dbFilePPath = Path.Combine(dbFolderPath, "Invoice.mdb");

                sConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbFilePPath}";
            }
            catch (Exception ex)
            {
                throw new Exception(ConstructorInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    ConstructorInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
            
        }
        #endregion

        #region Public Class Functions
        /// <summary>
        /// Allows a user to retrieve a table from a database
        /// </summary>
        /// <param name="sSQL">SQL command</param>
        /// <param name="iRetVal">The number of rows</param>
        /// <returns>A dataset with the retrieved table's informations</returns>
        /// <exception cref="Exception"></exception>
        public DataSet ExecuteSQLStatement(string sSQL, ref int iRetVal)
        {
            try
            {
                DataSet ds = new DataSet();

                using(OleDbConnection conn = new OleDbConnection(sConnectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                    {
                        // Open the connection to the database
                        conn.Open();

                        // Add the information for the SelectCommand using the SQL statement and the connection object
                        adapter.SelectCommand = new OleDbCommand(sSQL, conn);
                        adapter.SelectCommand.CommandTimeout = 0;

                        // Fill up the DataSet with data
                        adapter.Fill(ds);
                    }
                }

                iRetVal = ds.Tables[0].Rows.Count;

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }

        /// <summary>
        /// method for sql query, that only takes a string as a parameter, and returns a dataset regardless of how many rows.
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet SelectSQLQuery(string sSQL)
        {
            try
            {
                DataSet ds = new DataSet();

                using (OleDbConnection conn = new OleDbConnection(sConnectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                    {
                        // Open the connection to the database
                        conn.Open();

                        // Add the information for the SelectCommand using the SQL statement and the connection object
                        adapter.SelectCommand = new OleDbCommand(sSQL, conn);
                        adapter.SelectCommand.CommandTimeout = 0;

                        // Fill up the DataSet with data
                        adapter.Fill(ds);
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }

        }

        /// <summary>
        /// Allows a user to retrieve a single value from a database
        /// </summary>
        /// <param name="sSQL">The SQL Query statement</param>
        /// <returns>A single value retrived from the database</returns>
        /// <exception cref="Exception"></exception>
        public string ExecuteScalarSQL(string sSQL)
        {
            try
            {
                object obj;

                using (OleDbConnection conn = new OleDbConnection(sConnectionString))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                    {
                        // Open the connection to the database
                        conn.Open();

                        // Add the information for the SelectCommand using the SQL statement and the connection object
                        adapter.SelectCommand = new OleDbCommand(sSQL, conn);
                        adapter.SelectCommand.CommandTimeout = 0;

                        // Fill up the DataSet with data
                        obj = adapter.SelectCommand.ExecuteScalar();
                    }
                }

                if(obj == null)
                {
                    return "";
                }
                else
                {
                    return obj.ToString();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }

        /// <summary>
        /// Used when a user wants to interact with a database but does not want to query it
        /// </summary>
        /// <param name="sSQL">The SQL statement for the database</param>
        /// <returns>The number of rows that were affected</returns>
        /// <exception cref="Exception"></exception>
        public int ExecuteNonQuery(string sSQL)
        {
            try
            {
                //Number of rows affected
                int iNumRows;

                using (OleDbConnection conn = new OleDbConnection(sConnectionString))
                {
                    // Open the connection to the database
                    conn.Open();

                    // Add the information for the SelectCommand using the SQL statement and the connection object
                    OleDbCommand cmd = new OleDbCommand(sSQL, conn);
                    cmd.CommandTimeout = 0;

                    //Execute the non query SQL statement
                    iNumRows = cmd.ExecuteNonQuery();
                }

                // return the number of rows affected
                return iNumRows;

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        #endregion
    }
}
