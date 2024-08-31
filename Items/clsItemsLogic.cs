using Group_Project.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group_Project.Items
{
    class clsItemsLogic
    {

        #region Variables
        /// <summary>
        /// initialize dbAccess class
        /// </summary>
        clsDBAccess dbConn = new clsDBAccess();

        /// <summary>
        /// initialize item sql class, which holds all SQL statements to be used in the Items window
        /// </summary>
        clsItemsSQL itemsSQL = new clsItemsSQL();

        static bool bIsItemsChanged;

        #endregion

        public static bool IsItemsChanged
        {
            get { return  bIsItemsChanged; }
            set { bIsItemsChanged = value; }
        }

        /// <summary>
        /// connect to database, and get sql string to return list to use in datagrid
        /// </summary>
        /// <returns></returns>
        public List<clsItem> getItemsList()
        {
            try
            {
                List<clsItem> lstItemsList = new List<clsItem>();
                DataSet dsItems = new DataSet();
                string sSQL = itemsSQL.getAllItems();

                dsItems = dbConn.SelectSQLQuery(sSQL);

                foreach (DataRow dr in dsItems.Tables[0].Rows)
                {
                    //lstItemsList.Add(new clsItem { SItemCode = $"{dr["ItemCode"]}", SItemDesc = $"{dr["ItemDesc"]}", ICost = (decimal)dr["Cost"] });
                    lstItemsList.Add(new clsItem($"{dr["ItemCode"]}", $"{dr["ItemDesc"]}", (decimal)dr["Cost"]));
                }

                return lstItemsList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// update current item if all 3 fields have new info
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="dItemCost"></param>
        public bool editCurrentItemCostDesc(string sItemCode, string sItemDesc, int iItemCost)
        {
            try
            {
                Debug.WriteLine(checkItemExists(sItemCode));

                if (checkItemExists(sItemCode))
                {
                    
                    string sSQL = itemsSQL.updateItemCostDesc(sItemCode, sItemDesc, iItemCost);
                    dbConn.ExecuteNonQuery(sSQL);

                    bIsItemsChanged = true;

                    Debug.WriteLine(bIsItemsChanged);

                    //dbConn.ExecuteNonQuery(sSQL);

                    return true;

                }
                else
                {
                    bIsItemsChanged = false;

                    Debug.WriteLine(bIsItemsChanged);

                    return false;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// update item cost if Item Description field is left empty
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="dItemCost"></param>
        public bool editCurrentItemCost(string sItemCode, int iItemCost)
        {
            try
            {
                if (checkItemExists(sItemCode))
                {
                    string sSQL = itemsSQL.updateItemCost(sItemCode, iItemCost);

                    bIsItemsChanged = true;

                    Debug.WriteLine(bIsItemsChanged);

                    dbConn.ExecuteNonQuery(sSQL);

                    return true;
                }
                else
                {
                    bIsItemsChanged = false;

                    Debug.WriteLine(bIsItemsChanged);

                    return false;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// update item description if the item cost field is left empty
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        public bool editCurrentItemDesc(string sItemCode, string sItemDesc)
        {
            try
            {
                if (checkItemExists(sItemCode))
                {
                    string sSQL = itemsSQL.updateItemDesc(sItemCode, sItemDesc);

                    dbConn.ExecuteNonQuery(sSQL);

                    bIsItemsChanged = true;

                    Debug.WriteLine(bIsItemsChanged);

                    Debug.WriteLine(sSQL);

                    //dbConn.ExecuteNonQuery(sSQL);

                    return true;

                }
                else
                {
                    bIsItemsChanged = false;

                    Debug.WriteLine(bIsItemsChanged);

                    return false;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// used to add item to db. Method will first check if Item code is currently being used, and if so throw and exception
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="iItemCost"></param>
        /// <returns></returns>
        public bool addItem(string sItemCode, string sItemDesc, int iItemCost)
        {
            try
            {
                if (!checkItemExists(sItemCode))
                {
                    string sSQL = itemsSQL.insertNewItem(sItemCode, sItemDesc, iItemCost);

                    dbConn.ExecuteNonQuery(sSQL);

                    bIsItemsChanged = true;

                    Debug.WriteLine(bIsItemsChanged);

                    Debug.WriteLine(checkItemExists(sItemCode));
                    Debug.WriteLine(sSQL);

                }
                else
                {
                    bIsItemsChanged = false;

                    Debug.WriteLine(bIsItemsChanged);

                    throw new Exception("Item Code must be unique");

                }

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        /// <summary>
        /// logic to delete item, but to delete an item, the item code must exist in the database 
        /// and the item cannot be used to any current invoices.
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        public bool deleteItem(string sItemCode)
        {
            try
            {
                if (checkItemExists(sItemCode) && !checkInvoiceForItem(sItemCode))
                {
                    string sSQL = itemsSQL.deleteItem(sItemCode);

                    dbConn.ExecuteNonQuery(sSQL);

                    bIsItemsChanged = true;

                    Debug.WriteLine(bIsItemsChanged);

                    Debug.WriteLine(sSQL);

                    return true;

                }

                bIsItemsChanged = false;

                return false;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// query database for list of all unique invoice numbers that have the provided item code.
        /// if the data base returns a list of items and exception will be thrown,
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool checkInvoiceForItem(string sItemCode)
        {
            try
            {
                string sSQL = itemsSQL.getInvoiceInfo(sItemCode);
                bool b = false;

                DataSet ds = dbConn.SelectSQLQuery(sSQL);

                StringBuilder sb = new StringBuilder();

                if (ds.Tables.Count > 0)
                {

                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 1)
                    {
                        b = true;
                        sb.AppendLine($"{dt.Columns[0].ColumnName}\n");

                        foreach (DataRow dr in dt.Rows)
                        {
                            sb.Append($"{dr[0].ToString()}\n");

                        }
                        throw new Exception(sb.ToString());

                    }

                }

                return b;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// method will query dataset for Item code to check if item is in database
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool checkItemExists(string sItemCode)
        {
            try
            {
                string sSQL = itemsSQL.checkItemCode(sItemCode);
                DataSet ds = dbConn.SelectSQLQuery(sSQL);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return false;

                }
                else if (ds.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if ((string)row["ItemCode"] == sItemCode)
                        {
                            return true;

                        }
                        else
                        {
                            return false;

                        }

                    }

                    return false;

                }
                else
                {
                    return true;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

    }
}
