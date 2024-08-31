using Group_Project.Common;
using Group_Project.Items;
using Group_Project.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

namespace Group_Project.Main
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        /// <summary>
        /// Enumeration to track the status of the program
        /// </summary>
        enum Status
        {
            waiting,
            editing,
            creating
        }

        #region Class Attributes
        /// <summary>
        /// Class to connect this window to other classes
        /// </summary>
        clsMainLogic mainLogic;
        /// <summary>
        /// Items Window
        /// </summary>
        WndItems wWndItems;
        /// <summary>
        /// Search Window
        /// </summary>
        WndSearch wWndSearch;
        /// <summary>
        /// Enum to track what the user is doing in the window
        /// </summary>
        Status eStatus = new Status();
        /// <summary>
        /// Tracks the current Invoice items the user is making/looking at
        /// </summary>
        List<clsItem> newInvoice;
        /// <summary>
        /// Tracks the current invoice number being looked at
        /// </summary>
        private int iCurrentInvoiceNum;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor, binds the items and disables all but one button, resets prices
        /// </summary>
        public WndMain()
        {
            InitializeComponent();
            mainLogic = new clsMainLogic();
            newInvoice = new List<clsItem>();

            eStatus = Status.waiting;

            DisplayItems();
            CreateButtonActive();
            ResetPrice();
        }
        #endregion

        #region Private Class Functions
        /// <summary>
        /// Opens the Items Window when the items button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsWnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the user is currently editing an invoice
                if (eStatus == Status.creating || eStatus == Status.editing)
                {
                    // Warn user of chance to lose all data
                    MessageBoxResult userAns = MessageBox.Show("Are you sure you want to go to the items window? All unsaved Data will be lost",
                                                               "All data will be lost", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (userAns != MessageBoxResult.Yes)
                    {
                        // return without loading the window
                        return;
                    }
                }
                wWndItems = new WndItems();
                this.Hide();
                wWndItems.ShowDialog();

                //Check if the items were updated
                if (mainLogic.IsItemsChanged())
                {
                    DisplayItems();
                }

                ReturnFromWindow();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        } 
        /// <summary>
        /// Opens the Search Window when the search button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchWnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the user is currently editing an invoice
                if(eStatus == Status.creating || eStatus == Status.editing)
                {
                    // Warn user of chance to lose all data
                    MessageBoxResult userAns = MessageBox.Show("Are you sure you want to go to the search window? All unsaved Data will be lost",
                                                               "All data will be lost", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if(userAns != MessageBoxResult.Yes)
                    {
                        // return without loading the window
                        return;
                    }
                }
                wWndSearch = new WndSearch();
                this.Hide();
                wWndSearch.ShowDialog();

                // Check if an invoice was selected
                if (mainLogic.IsSearchChanged())
                {
                    btnEdit.IsEnabled = true;
                    iCurrentInvoiceNum = mainLogic.GetNewInvoiceNum();
                }
                else
                {
                    ReturnFromWindow();
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Activates the buttons to create an invoice, clears the items tracker
        /// Updates the status of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                eStatus = Status.creating;
                EditCreateButtonsDeactive();
                newInvoice.Clear();
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Activates the buttons to edit an invoice
        /// Updates the status of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                eStatus = Status.editing;
                EditCreateButtonsDeactive();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Saves the currently displayed invoice to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblInvoiceDateError.Visibility = Visibility.Collapsed; // Hide error label

                if (ValidDate()) // Verify the date provided is valid
                {
                    if(eStatus == Status.creating) // Creating an invoice
                    {
                        // Create Invoice
                        mainLogic.CreateNewInvoice(txtInvoiceDate.Text, UpdateTotal());
                        // Store New Invoice Number
                        iCurrentInvoiceNum = mainLogic.GetNewInvoiceNum();
                        // Add all line items
                        mainLogic.AddLineItems(newInvoice);
                        // Show the new invoice number to the user
                        lblInvoiceNum.Content = iCurrentInvoiceNum;
                    }
                    else if(eStatus == Status.editing) // Editing an invoice
                    {
                        // Updates the invoice price of the invoice
                        mainLogic.EditInvoicePrice(iCurrentInvoiceNum, UpdateTotal());
                        // Edits the lines of the invoice to match to user's change(s)
                        mainLogic.EditLineItems(iCurrentInvoiceNum, newInvoice);
                    }
                    else
                    {
                        // Throw error if status is waiting
                        HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                                    MethodInfo.GetCurrentMethod().Name, "Invalid Status when saving Invoice");
                    }
                    // Set combobox to no selection
                    comboItems.SelectedIndex = -1;
                    // Set only the create and edit buttons active
                    CreateEditButtonActive();
                    // Set status to waiting
                    eStatus = Status.waiting;
                }
                else
                {
                    // Show error if date is invalid
                    lblInvoiceDateError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Cancels the change(s) made by the user to the invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(eStatus == Status.editing)
                {
                    DisplayInvoice(); // Display the invoice with no changes
                    CreateEditButtonActive(); // Only have create and edit buttons active
                }
                else
                {
                    newInvoice.Clear(); // Clear the items list
                    UpdateDataGrid(); // Update the data grid
                    CreateButtonActive(); // Only have the create button active
                }
                eStatus = Status.waiting; // Set status to waiting
                comboItems.SelectedIndex = -1; // Set combobox to no selection
                UpdateDataGrid(); // Update datagrid to show last loaded invoice or blank invoice
                UpdateTotal(); // Show the new total
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Changes the price of items when the combobox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(comboItems.SelectedIndex >= 0) // If an item is selected show the price
                {
                    clsItem selectedItem = (clsItem)comboItems.SelectedItem;

                    lblPrice.Content = "$" + selectedItem.Cost.ToString();

                    btnRemoveItem.IsEnabled = true;
                    btnAddItem.IsEnabled = true;
                }
                else // If no item is selected show $0.00
                {
                    btnAddItem.IsEnabled = false;
                    btnRemoveItem.IsEnabled = false;
                    lblPrice.Content = "$0.00";
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Adds Item to the item list and shows it on the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clsItem selectedItem = (clsItem)comboItems.SelectedItem; // Create item with selected item

                newInvoice.Add(selectedItem); // Add item to the items list
                UpdateDataGrid(); // Udate the datagrid to reflect new added item
                UpdateTotal(); // Update the total price of the invoice
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Removes the selected item from the item list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clsItem selectedItem = (clsItem)comboItems.SelectedItem; // Create item of selected item

                if (dgInvoiceItems.Items.Contains(selectedItem)) // If the item is on the invoice
                {
                    newInvoice.Remove(selectedItem); // Remove the item from the invoice
                    UpdateDataGrid(); // Update the datagrid to reflect changes
                    UpdateTotal(); // Update the total
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Sets the combobox to the item selected from the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgInvoiceItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            clsItem selectedItem = (clsItem)dgInvoiceItems.SelectedItem;
            dgInvoiceItems.SelectedItem = null;
            comboItems.SelectedItem = selectedItem;
        }
        /// <summary>
        /// Removes default string if the textbox gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInvoiceDate_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtInvoiceDate.Text == "MM/DD/YYYY") // If the default value is in the textbox
                {
                    txtInvoiceDate.Text = String.Empty; // delete the default string
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                            MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Displays all database items in a combobox for the user to interact with
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void DisplayItems()
        {
            try
            {
                comboItems.ItemsSource = mainLogic.GetItemsList(); // Displays all available items in the database
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Get invoice from teh database then display it to the user
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void DisplayInvoice()
        {
            try
            {
                // Get the invoice items from the database
                newInvoice = mainLogic.GetItemsInInvoice(iCurrentInvoiceNum);
                UpdateDataGrid();
                UpdateTotal();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Deactivates the create and edit buttons, activates all user modifiable containers
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void EditCreateButtonsDeactive()
        {
            try
            {
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;
                comboItems.IsEnabled = true;
                txtInvoiceDate.IsEnabled = true;

                btnAddItem.IsEnabled = false;
                btnRemoveItem.IsEnabled = false;
                btnCreate.IsEnabled = false;
                btnEdit.IsEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Loops through the list of invoice items and calculates a total cost
        /// </summary>
        /// <returns>The total cost</returns>
        /// <exception cref="Exception"></exception>
        private decimal UpdateTotal()
        {
            try
            {
                decimal iTotal = 0;

                for (int i = 0; i < newInvoice.Count; i++) // Loop and add
                {
                    iTotal += newInvoice[i].Cost;
                }

                lblTotal.Content = "$" + iTotal.ToString(); // Display total
                
                return iTotal; // Return total
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Sets the item and total price to 0
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ResetPrice()
        {
            try
            {
                lblPrice.Content = "$0.00";
                lblTotal.Content = "$0.00";
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Bind datagrid to item list and refresh datagrid
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void UpdateDataGrid()
        {
            try
            {
                dgInvoiceItems.ItemsSource = newInvoice;
                dgInvoiceItems.Items.Refresh();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Enables only the Create and Edit buttons
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CreateEditButtonActive()
        {
            try
            {
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnAddItem.IsEnabled = false;
                btnRemoveItem.IsEnabled = false;
                comboItems.IsEnabled = false;
                txtInvoiceDate.IsEnabled = false;

                btnEdit.IsEnabled = true;
                btnCreate.IsEnabled = true;

            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Enables only the Create button
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CreateButtonActive()
        {
            try
            {
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnAddItem.IsEnabled = false;
                btnRemoveItem.IsEnabled = false;
                comboItems.IsEnabled = false;
                txtInvoiceDate.IsEnabled = false;
                btnEdit.IsEnabled = false;

                btnCreate.IsEnabled = true;
                
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Checks if the user's date input is a valid date
        /// </summary>
        /// <returns>Boolean if the provided date is valid</returns>
        /// <exception cref="Exception"></exception>
        private bool ValidDate()
        {
            try
            {
                DateTime dateTime = new DateTime();

                string[] acceptedFormats =
                {
                    "M/d/yyyy",
                    "MM/d/yyyy",
                    "MM/dd/yyyy"
                };

                return (DateTime.TryParseExact(txtInvoiceDate.Text, acceptedFormats, new CultureInfo("en-US"), DateTimeStyles.None, out dateTime) && txtInvoiceDate.Text != String.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Behavior for finding the last invoice after returning from a different window
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ReturnFromWindow()
        {
            try
            {
                if(eStatus == Status.editing)
                {
                    CreateEditButtonActive();
                    DisplayInvoice();
                }
                else
                {
                    newInvoice.Clear();
                    CreateButtonActive();
                }

                eStatus = Status.waiting;
                comboItems.SelectedIndex = -1;
                UpdateDataGrid();
                this.Show();
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + "->" + ex.Message);
            }
        }
        /// <summary>
        /// Ensures all windows close when main window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Error Handling
        /// <summary>
        /// exception handler that shows the error
        /// </summary>
        /// <param name="sClass">the class</param>
        /// <param name="sMethod">the method</param>
        /// <param name="sMessage">the error message</param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText("F:\\Weber\\Classes\\CS 3280 Object Oriented Windows Using C#\\Errors.txt",
                                             Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }


        #endregion

        
    }
}
