using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group_Project.Common
{
    internal class clsItem
    {
        #region Class Attributes
        /// <summary>
        /// The Unique item code of items
        /// </summary>
        string sItemCode;
        /// <summary>
        /// The description of the item
        /// </summary>
        string sItemDesc;
        /// <summary>
        /// The cost of the item
        /// </summary>
        decimal dCost;
        #endregion

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sItemCode">The Item Code</param>
        /// <param name="sItemDesc">The Item Description</param>
        /// <param name="dCost">The Item Cost</param>
        public clsItem(string sItemCode, string sItemDesc, decimal dCost)
        {
            this.sItemCode = sItemCode;
            this.sItemDesc = sItemDesc;
            this.dCost = dCost;
        }
        //public clsItem(string sItemCode, string sItemDesc, decimal dCost, int iLineItemNum)
        //{
        //    this.sItemCode = sItemCode;
        //    this.sItemDesc = sItemDesc;
        //    this.dCost = dCost;
        //    this.iLineItemNum = iLineItemNum;
        //}

        #endregion

        #region Properties
        /// <summary>
        /// Getter and Setter for sItemCode
        /// </summary>
        public string ItemCode { get { return sItemCode; } set {  sItemCode = value; } }
        /// <summary>
        /// Getter and Setter for sItemDesc
        /// </summary>
        public string ItemDesc { get {  return sItemDesc; } set { sItemDesc = value; } }
        /// <summary>
        /// Getter and Setter for dCost
        /// </summary>
        public decimal Cost { get { return dCost; } set { dCost = value; } }
        #endregion

        public override string ToString()
        {
            return sItemCode + " - " + sItemDesc; 
        }
    }
}
