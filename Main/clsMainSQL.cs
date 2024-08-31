using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Group_Project.Main
{
    class clsMainSQL
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <exception cref="Exception"></exception>
        public clsMainSQL() {   }
        #endregion

        #region Public Class Functions
        /// <summary>
        /// Updates the total cost of a given invoice
        /// </summary>
        /// <param name="sInvoiceNum">The invoice number that needs to change</param>
        /// <param name="sTotalCost">The new total cost of the invoice</param>
        /// <returns>SQL Update string</returns>
        /// <exception cref="Exception"></exception>
        public string UpdateInvoiceCost(string sInvoiceNum, string sTotalCost)
        {
            try
            {
                return "UPDATE Invoices SET TotalCost = " + sTotalCost + " WHERE InvoiceNum = " + sInvoiceNum;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
            
        }
        /// <summary>
        /// Inserts a new Invoice into the database
        /// </summary>
        /// <param name="sTotalCost">The total cost of the invoice</param>
        /// <param name="sDate">The date of the invoice</param>
        /// <returns>SQL insert string</returns>
        /// <exception cref="Exception"></exception>
        public string CreateNewInvoice(string sDate, string sTotalCost)
        {
            try
            {
                return "INSERT INTO Invoices (InvoiceDate, TotalCost) VALUES (#" + sDate + "#," + sTotalCost + ")";
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Inserts a new LineItem into the database
        /// </summary>
        /// <param name="sInvoiceNum">The invoice number</param>
        /// <param name="sItemCode">The item code of the item</param>
        /// <param name="LineItemNum">Which line the item is on</param>
        /// <returns>SQL Insert String</returns>
        /// <exception cref="Exception"></exception>
        public string CreateNewLineItem(string sInvoiceNum, string sLineItemNum, string sItemCode)
        {
            try
            {
                return "INSERT INTO LineItems VALUES (" + sInvoiceNum + "," + sLineItemNum + ",'" + sItemCode + "')";
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Queries all information in the ItemDesc table
        /// </summary>
        /// <returns>SQL query string</returns>
        /// <exception cref="Exception"></exception>
        public string GetItems()
        {
            try
            {
                return "SELECT * FROM ItemDesc";
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Queries all line items of a given invoice
        /// </summary>
        /// <param name="sInvoiceNum">Invoce number to query</param>
        /// <returns>SQL query string</returns>
        /// <exception cref="Exception"></exception>
        public string GetInvoiceItems(string sInvoiceNum)
        {
            try
            {
                return "SELECT l.ItemCode, i.ItemDesc, i.Cost FROM LineItems l, ItemDesc i WHERE l.ItemCode = i.ItemCode AND l.InvoiceNum = " + sInvoiceNum;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Gets the max invoice number which is the most recently added invoice
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetNewInvoiceNum()
        {
            try
            {
                return "SELECT MAX(InvoiceNum) FROM Invoices";
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Deletes all line items given an invoice number
        /// </summary>
        /// <param name="sInvoiceNum">The invoice numer of line items to delete</param>
        /// <returns>SQL delete string</returns>
        /// <exception cref="Exception"></exception>
        public string DeleteAllLineItems(string sInvoiceNum)
        {
            try
            {
                return "DELETE FROM LineItems WHERE InvoiceNum = " + sInvoiceNum;
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
