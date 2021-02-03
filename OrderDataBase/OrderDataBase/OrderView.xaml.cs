/*
    Class: OrderView
    Workingdes.: By not failed login is it able to click products and order them on the account which is logged in
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OrderDataBase
{
    /// <summary>
    /// Interaktionslogik für OrderView.xaml
    /// </summary>
    public partial class OrderView : Window
    {
        // Connectionstring and DataTable Instances
        // Instance of a decimal variable with value 0 and string where the file goes saved in
        OleDbConnection con;
        DataTable dt;
        decimal totalPrice = 0;
        String filesaveLocation = ConfigurationSettings.AppSettings["FileLocation"];

        // Constructor from this class
        public OrderView()
        {
            InitializeComponent();
        }

        // Button to go back to the MainWindow
        private void btnHauptfenster_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainW = new MainWindow();
            mainW.Show();
            this.Close();
        }

        // EventHandler button which is checking the input logdata
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string mail = txbMailLogin.Text;
            string passwort = pswBox.Password;
            //var str = txbPasswortLogin.Text;
            var str = pswBox.Password;
            var password = "DKCryPty";
            var strEncryptred = Cipher.Encrypt(str, password);

            string commandString = "SELECT COUNT(*) from LoginDB WHERE Mail = '" + mail + "' AND PassWort = '" + strEncryptred + "'";

            OleDbCommand command = new OleDbCommand(commandString, con);
            con.Open();
            OleDbDataReader reader = command.ExecuteReader();

            Boolean foundUser = false;

            while (reader.Read())
            {
                if (reader.GetInt32(0) > 0)
                {
                    dgvOrderProducts.IsEnabled = true;
                    foundUser = true;
                }
            }

            if (!foundUser)
            {
                MessageBox.Show("Login fehlgeschlagen Username oder Paswort falsch...");
            }

            reader.Close();
            con.Close();
            
        }

        // Method to empty the strings from the textboxes
        private void EmptyBoxes()
        {
            txbMailLogin.Text = "";
            pswBox.Password = "";
        }
        
        //Loaded Event from the Class Grid with the connection from the Access DB
        private void Orderview_Loaded(object sender, RoutedEventArgs e)
        {
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            BindGrid();
           
            //MessageBox.Show("" + filesaveLocation);
        }

        // Method which is calling the Access DB from an path and mirrors in the DataGrid
        private void BindGrid()
        {
            OleDbCommand cmd = new OleDbCommand();
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            cmd.CommandText = "select * from ProduktTab";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dgvOrderProducts.ItemsSource = dt.AsDataView();
            con.Close();

            if (dt.Rows.Count > 0)
            {
                dgvOrderProducts.Visibility = System.Windows.Visibility.Visible;
                con.Close();
            }
            else
            {
                dgvOrderProducts.Visibility = System.Windows.Visibility.Hidden;
                con.Close();
            }
            dgvOrderProducts.Columns[0].Visibility = Visibility.Collapsed;
        }

        // MouseDoubleClick Event which is put in the clicked dataset into the richtextbox
        private void dgvOrderProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            #region Alternative
            //var rows = GetDataGridRows(dgvOrderProducts);

            //foreach (DataGridRow r in rows)
            //{
            //    //   DataRowView rv = (DataRowView)r.Item;
            //    foreach (DataGridColumn column in dgvOrderProducts.Columns)
            //    {
            //        if (column.GetCellContent(r) is TextBlock)
            //        {
            //            TextBlock cellContent = column.GetCellContent(r) as TextBlock;
            //            //MessageBox.Show(cellContent.Text);

            //            rtbBestellungen.Selection.Text += cellContent.Text +  "\n";
            //        }
            //    }
            //}

            //rtbBestellungen.Selection.Text +=  this.dgvOrderProducts.CurrentCell.Column.ToString(); 
            #endregion
            DataGrid dataGrid = sender as DataGrid;
            DataRowView rowView = dataGrid.SelectedItem as DataRowView;
            string myCellValue1 = rowView.Row[0].ToString();
            string myCellValue2 = rowView.Row[1].ToString();
            string myCellValue3 = rowView.Row[2].ToString();
            string myCellValue4 = rowView.Row[3].ToString();

            decimal myPrice = (decimal)rowView.Row[4];

            rtbBestellungen.Selection.Text += "" + myCellValue1 + " | "
                + myCellValue2 + " | "
                + myCellValue3 + " | "
                + myCellValue4 + " | "
                + myPrice + " | \n";

            totalPrice += myPrice;
            updateTotPreis();
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }
    
        // Method which puts in the price into the Label
        public void updateTotPreis()
        {
            lblPreis.Content = " " + totalPrice.ToString() + " €";
        }

        // Button EventHandler to create a text fileand  write the order in it
        private void btnBestellen_Click(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory +  filesaveLocation;
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Bestellung von Kunde: " + txbMailLogin.Text + "\n\n");
                    sw.WriteLine("Artikelnummer | Bezeichnung | Beschreibung | €\n");
                    sw.WriteLine(rtbBestellungen.Selection.Text);
                    sw.WriteLine("\n\n\n" + lblPreis.Content);
                }
            }
            Process.Start(path);
            EmptyBoxes();
        }
        
        #region TXB_SearchElements
        private Dictionary<string, string> _conditions = new Dictionary<string, string>();

        private void UpdateFilter()
        {
            var activeConditions = _conditions.Where(c => c.Value != null).Select(c => "(" + c.Value + ")");
            DataView dv = dgvOrderProducts.ItemsSource as DataView;
            dv.RowFilter = string.Join(" AND ", activeConditions);
        }

        private void txtSuche_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtSuche.Text;
            if (string.IsNullOrEmpty(filter))
                _conditions["ProduktTab"] = null;
            else
                _conditions["ProduktTab"] = string.Format("WarenNummer Like '%{0}%' " +
                                                       " OR Bezeichnung LIKE '%{0}%' " +
                                                       " OR Beschreibung LIKE '%{0}%' ", filter);
            UpdateFilter();
        }
        #endregion

        #region GotFocus_LostFocus_Elements
        private void txtSuche_GotFocus(object sender, RoutedEventArgs e)
        {
            lblSuche.Visibility = Visibility.Hidden;
        }

        private void txtSuche_LostFocus(object sender, RoutedEventArgs e)
        {
            lblSuche.Visibility = Visibility.Visible;
        }

        private void txbMailLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            lblMail.Visibility = Visibility.Hidden;
        }

        private void txbMailLogin_LostFocus(object sender, RoutedEventArgs e)
        {
            txbMailLogin.Text = txbMailLogin.Text;
            lblMail.Visibility = Visibility.Visible;
        }

        private void btnBestellungloeschen_Click(object sender, RoutedEventArgs e)
        {
            if (rtbBestellungen.Selection.Text.Length > 0 || rtbBestellungen.Selection.Text != "Bestellung")
            {
                rtbBestellungen.Selection.Text = String.Empty;
                lblPreis.Content = String.Empty;
                totalPrice = 0;
            }
            else
            {
                MessageBox.Show("Bitte Bestellung überprüfen..");
            }

        }

        private void pswBox_GotFocus(object sender, RoutedEventArgs e)
        {
            lblPasswort.Visibility = Visibility.Hidden;
        }

        private void pswBox_LostFocus(object sender, RoutedEventArgs e)
        {
            lblPasswort.Visibility = Visibility.Visible;
        } 
        #endregion

        private void btnBestelungspeichern_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
