using Group_Project.Common;
using Group_Project.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Group_Project.Items
{
    /// <summary>
    /// Interaction logic for WndItems.xaml
    /// </summary>
    public partial class WndItems : Window
    {

        #region Variables
        /// <summary>
        /// initialize logic class to query data
        /// </summary>
        clsItemsLogic clsItemLogic = new clsItemsLogic();

        /// <summary>
        /// used to send info back to main window
        /// </summary>
        private WndMain _WndMain;

        /// <summary>
        /// if an item is updated, this is changed to true and sent back to the main window
        /// </summary>
        //static bool bItemUpdated = false;

        #endregion

        /// <summary>
        /// initialize window, must accept mainWnd as parameter to pass data back to main if an item was updated.
        /// </summary>
        /// <param name="mainWnd"></param>
        /// <exception cref="Exception"></exception>
        public WndItems()
        {
            try
            {
                InitializeComponent();

                //_WndMain = mainWnd;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        /// <summary>
        /// Used to call methods after the Window has loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                PopulateDataGrid();

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured starting the Items window{ex.Message}");

            }


        }

        /// <summary>
        /// This checks if an item was updated, and if an item was updated, pass it to the main window, otherwise
        /// close the window as normal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                //if (bItemUpdated)
                //{
                //    _WndMain.bIsItemUpdated = true;

                //}

            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending data to main window: {ex.Message}");

            }

        }

        /// <summary>
        /// method used to get data from database and populate datagrid with this data
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void PopulateDataGrid()
        {
            try
            {
                List<clsItem> items = clsItemLogic.getItemsList();

                dtgrdItemsList.ItemsSource = items;

                SetDataGridHeaders();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }


        /// <summary>
        /// method used to add an item to the database should the 'Add Item' button be clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            int iItemCost;

            try
            {
                if (txtbxCode.Text.Length < 1 || txtbxDescription.Text.Length < 1 || txtbxCost.Text.Length < 1)
                {
                    throw new Exception("You must fill in all fields to add an item. Please ensure the Item Code is unique, and cost is a numeric value");

                }
                else if (int.TryParse(txtbxCost.Text, out iItemCost))
                {
                    clsItemsLogic.IsItemsChanged = clsItemLogic.addItem(txtbxCode.Text, txtbxDescription.Text, iItemCost);

                    txtbxCode.Clear();
                    txtbxDescription.Clear();
                    txtbxCost.Clear();

                    PopulateDataGrid();

                }

            }
            catch (Exception ex)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show(ex.Message, "Error has Occured", button);

            }

        }

        /// <summary>
        /// used to edit items in the database if this button is clicked.
        /// if the user leaves the Item Code box empty and exception will be thrown
        /// if the user leaves one of the other two boxes empty, the empty field will not be updated and only the field with
        /// new data to save.
        /// Should the user leave all 3 fields blank, the a different exception will be throw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtbxCode.Text.Length < 1)
                {
                    // throw exception if user did not enter item code
                    throw new Exception("You must enter an Item code in the text box.");

                }
                else if (txtbxCode.Text.Length < 1 && txtbxDescription.Text.Length < 1 && txtbxCost.Text.Length < 1)
                {
                    // throw exception if all fields are empty
                    throw new Exception("Please enter the Item Code and which fields you would like to update");

                }
                else if (txtbxCode.Text.Length >= 1 && txtbxCost.Text.Length >= 1 && txtbxDescription.Text.Length > 1)
                {
                    string sItemCode = txtbxCode.Text;
                    string sItemDescription = txtbxDescription.Text;
                    int iItemCost;

                    bool bIsNumEntered = int.TryParse(txtbxCost.Text, out iItemCost);

                    if (!bIsNumEntered)
                    {
                        throw new Exception("Please enter a number for the Item Cost");
                        
                    }

                    Debug.WriteLine($"Code: {sItemCode} - Desc: {sItemDescription} - Cost: {iItemCost}");

                    clsItemsLogic.IsItemsChanged = clsItemLogic.editCurrentItemCostDesc(sItemCode, sItemDescription, iItemCost);

                }
                else if (txtbxCode.Text.Length >= 1 && txtbxCost.Text.Length >= 1 && txtbxDescription.Text.Length < 1)
                {
                    // edit item cost if item code and item cost fields have correct data
                    string sItemCode = txtbxCode.Text;
                    int iItemCost;

                    if (int.TryParse(txtbxCost.Text, out iItemCost))
                    {
                        clsItemsLogic.IsItemsChanged = clsItemLogic.editCurrentItemCost(sItemCode, iItemCost);

                    }
                    else
                    {
                        throw new Exception("Please enter a number for the Item Cost");

                    }

                }
                else if (txtbxCode.Text.Length >= 1 && txtbxCost.Text.Length < 1 && txtbxDescription.Text.Length > 1)
                {
                    // edit item description if Item code and item description have correct data
                    string sItemCode = txtbxCode.Text;
                    string sItemDescription = txtbxDescription.Text;

                    clsItemsLogic.IsItemsChanged = clsItemLogic.editCurrentItemDesc(sItemCode, sItemDescription);

                }
                else
                {
                    throw new Exception("An unknown error has occured");

                }

                txtbxCode.Clear();
                txtbxDescription.Clear();
                txtbxCost.Clear();

                PopulateDataGrid();

            }
            catch (Exception ex)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show(ex.Message, "Error has Occured", button);

            }

        }

        /// <summary>
        /// button logic used to delete an item based on the Item Code user has provided
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtbxCode.Text.Length > 0)
                {

                    clsItemsLogic.IsItemsChanged = clsItemLogic.deleteItem(txtbxCode.Text);
                    PopulateDataGrid();

                    txtbxCode.Clear();
                    txtbxDescription.Clear();
                    txtbxCost.Clear();

                }
                else
                {
                    throw new Exception("To delete an item, please confirm and enter the Item Code");

                }

            }
            catch (Exception ex)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show(ex.Message, "Error has Occured", button);

            }

        }

        /// <summary>
        /// update the datagrid headers after each query
        /// </summary>
        private void SetDataGridHeaders()
        {
            dtgrdItemsList.Columns[0].Header = "Item Code";
            dtgrdItemsList.Columns[1].Header = "Item Description";
            dtgrdItemsList.Columns[2].Header = "Item Cost";

        }
    }
}