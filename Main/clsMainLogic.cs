using Accessibility;
using Group_Project.Common;
using Group_Project.Items;
using Group_Project.Search;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Group_Project.Main
{
    class clsMainLogic
    {
        #region Class Attributes
        /// <summary>
        /// Access to run commands on the database
        /// </summary>
        clsDBAccess DBAccess;
        /// <summary>
        /// Access to SQL statements
        /// </summary>
        clsMainSQL mainSQL;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public clsMainLogic()
        {
            DBAccess = new clsDBAccess();
            mainSQL = new clsMainSQL();
        }
        #endregion

        #region Public Class Functions        
        /// <summary>
        /// Checks if the items were changed 
        /// </summary>
        /// <returns></returns>
        public bool IsItemsChanged()
        {
            try
            {
                return clsItemsLogic.IsItemsChanged; // Using the static boolean in Item Logic class to pass information to the main screen
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
            
        }
        /// <summary>
        /// Checks to see if the user selected an invoice
        /// </summary>
        /// <returns>Boolean showing is the search window selected an invoice to load</returns>
        public bool IsSearchChanged()
        {
            try
            {
                return clsSearchLogic.IsInvioceSelected; // Using the static boolean in Search Logic class to pass information to the main screen
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Gets a list of all items from the database
        /// </summary>
        /// <returns>A list of all items</returns>
        /// <exception cref="Exception"></exception>
        public List<clsItem> GetItemsList()
        {
            try
            {
                List<clsItem> items = new List<clsItem>();
                DataSet ds = new DataSet();
                int iRows = 0;

                ds = DBAccess.ExecuteSQLStatement(mainSQL.GetItems(), ref iRows);

                for(int i=0; i<iRows; i++)
                {
                    decimal dCost;
                    string sItemCode = ds.Tables[0].Rows[i][0].ToString();
                    string sItemDesc = ds.Tables[0].Rows[i][1].ToString();
                    decimal.TryParse(ds.Tables[0].Rows[i][2].ToString(), out dCost);

                    items.Add(new clsItem(sItemCode, sItemDesc, dCost));
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Gets all items in a specified invoice
        /// </summary>
        /// <param name="iInvoiceNum">Invoice Number</param>
        /// <returns>List of items from the invoice given</returns>
        /// <exception cref="Exception"></exception>
        public List<clsItem> GetItemsInInvoice(int iInvoiceNum)
        {
            try
            {
                List<clsItem> items = new List<clsItem>();
                DataSet ds = new DataSet();
                int iRows = 0;

                ds = DBAccess.ExecuteSQLStatement(mainSQL.GetInvoiceItems(iInvoiceNum.ToString()), ref iRows);

                for(int i=0; i<iRows; i++)
                {
                    decimal dCost;
                    string sItemCode = ds.Tables[0].Rows[i][0].ToString();
                    string sItemDesc = ds.Tables[0].Rows[i][1].ToString();
                    decimal.TryParse(ds.Tables[0].Rows[i][2].ToString(), out dCost);

                    items.Add(new clsItem(sItemCode, sItemDesc, dCost));
                }
                
                return items;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Creates a new invoice in the database
        /// </summary>
        /// <param name="sInvoiceDate">Invocice Date</param>
        /// <param name="dTotalCost">Total Cost</param>
        /// <exception cref="Exception"></exception>
        public void CreateNewInvoice(string sInvoiceDate, decimal dTotalCost)
        {
            try
            {
                DBAccess.ExecuteNonQuery(mainSQL.CreateNewInvoice(sInvoiceDate, dTotalCost.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Updates the total price of an invoice
        /// </summary>
        /// <param name="iInvoiceNum">Invoice number you want to update</param>
        /// <param name="dTotalCost">New total price</param>
        /// <exception cref="Exception"></exception>
        public void EditInvoicePrice(int iInvoiceNum, decimal dTotalCost)
        {
            try
            {
                DBAccess.ExecuteNonQuery(mainSQL.UpdateInvoiceCost(iInvoiceNum.ToString(), dTotalCost.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Adds line items to the database
        /// </summary>
        /// <param name="items">A List of clsItems</param>
        /// <exception cref="Exception"></exception>
        public void AddLineItems(List<clsItem> items)
        {
            try
            {
                for(int i=0; i<items.Count; i++)
                {
                    DBAccess.ExecuteNonQuery(mainSQL.CreateNewLineItem(GetNewInvoiceNum().ToString(), (i + 1).ToString(), items[i].ItemCode));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Deletes all line items then adds the new line items to the specified invoice number
        /// </summary>
        /// <param name="iInvoiceNum">Invoice number</param>
        /// <param name="items">New list of items</param>
        /// <exception cref="Exception"></exception>
        public void EditLineItems(int iInvoiceNum, List<clsItem> items)
        {
            try
            {
                DBAccess.ExecuteNonQuery(mainSQL.DeleteAllLineItems(iInvoiceNum.ToString()));

                for (int i = 0; i < items.Count; i++)
                {
                    DBAccess.ExecuteNonQuery(mainSQL.CreateNewLineItem(iInvoiceNum.ToString(), (i + 1).ToString(), items[i].ItemCode));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Gets the most recently added invoice number
        /// </summary>
        /// <returns>Most recent invoice number</returns>
        /// <exception cref="Exception"></exception>
        public int GetNewInvoiceNum()
        {
            try
            {
                int iNewInvoiceNum;
                Int32.TryParse((DBAccess.ExecuteScalarSQL(mainSQL.GetNewInvoiceNum())), out iNewInvoiceNum);
                return iNewInvoiceNum;
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
