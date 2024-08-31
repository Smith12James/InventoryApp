using Group_Project.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group_Project.Items
{
    class clsItemsSQL
    {

        #region Variables


        #endregion

        /// <summary>
        /// return string as SQL statement for all items in database.
        /// </summary>
        /// <returns></returns>
        public string getAllItems()
        {
            return "SELECT ItemCode, ItemDesc, Cost FROM ItemDesc ORDER BY ItemCode";

        }

        /// <summary>
        /// return string as SQL statement to check Invoices for item code that was updated,
        /// and return that Invoice number.
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        public string getInvoiceInfo(string sItemCode)
        {
            return $"SELECT DISTINCT(InvoiceNum) FROM LineItems WHERE ItemCode='{sItemCode}'";

        }

        /// <summary>
        /// return string as SQL statement to update item description
        /// </summary>
        /// <param name="sNewItemDesc"></param>
        /// <returns></returns>
        public string updateItemDesc(string sItemCode, string sItemDesc)
        {
            return $"UPDATE ItemDesc SET ItemDesc='{sItemDesc}' WHERE ItemCode ='{sItemCode}';";

        }

        /// <summary>
        /// return string as SQL statement to update item cost
        /// </summary>
        /// <param name="iItemCost"></param>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        public string updateItemCost(string sItemCode, int iItemCost)
        {
            //return $"UPDATE ItemDesc SET ItemCost={dItemCost} WHERE ItemCode ='{sItemCode}';";

            return $"UPDATE ItemDesc SET Cost={iItemCost} WHERE ItemCode='{sItemCode}';";

        }

        /// <summary>
        /// update item cost and description using provided item code
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="iItemCost"></param>
        /// <returns></returns>
        public string updateItemCostDesc(string sItemCode, string sItemDesc, int iItemCost)
        {
            return $"UPDATE ItemDesc SET ItemDesc='{sItemDesc}', Cost={iItemCost} WHERE ItemCode='{sItemCode}';";

        }

        /// <summary>
        /// return string as SQL statement to check if ItemCode is already being used
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        public string checkItemCode(string sItemCode)
        {
            return $"SELECT ItemCode FROM ItemDesc WHERE ItemCode='{sItemCode}'";

        }

        /// <summary>
        /// return string as SQL statement to add new item into the table.
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="iCost"></param>
        /// <returns></returns>
        public string insertNewItem(string sItemCode, string sItemDesc, decimal dCost)
        {
            return $"INSERT INTO ItemDesc (ItemCode, ItemDesc, Cost) VALUES ('{sItemCode}', '{sItemDesc}', {dCost})";

        }

        /// <summary>
        /// return string as SQL statement to delete item from table
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        public string deleteItem(string sItemCode)
        {
            return $"DELETE FROM ItemDesc WHERE ItemCode='{sItemCode}'";

        }

    }
}
