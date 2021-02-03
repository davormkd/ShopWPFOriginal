/*
    Class: ProduktBestellungen
    Workingdes.: Able to insert, edit, delete or search datasets from or in the datagrid from this class
    
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OrderDataBase
{
    /// <summary>
    /// Interaktionslogik für ProdukteBestellungen.xaml
    /// </summary>
    public partial class ProdukteBestellungen : Window
    {
        // Instances of OleDbConnection and DataTable
        OleDbConnection con;
        DataTable dt;

        // Constructor
        public ProdukteBestellungen()
        {
            InitializeComponent();
        }

        // Button for go back to Mainwindow
        private void btnHauptfenster_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainW = new MainWindow();
            mainW.Show();
            this.Close();
        }

        // LoadEvent from the Grid. Sets the Connectionstring and call the Access DB from Path
        private void ProduktTabelle_Loaded(object sender, RoutedEventArgs e)
        {
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            BindGrid();
        }

        // Method to calculate the price of the inserted products
        private void CalcSumPrice()
        {
            decimal sum = 0m;
            foreach (DataRowView row in dgvProdukt.ItemsSource)
            {
                sum += (decimal)row["Preis"];
            }

            lblSumPreis.Content = "€ " +  Math.Round(sum, 2).ToString() + " | ";
        }

        // Method to call the Access DB from a path and mirrors in the DataGrid
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
            dgvProdukt.ItemsSource = dt.AsDataView();
            con.Close();

            if (dt.Rows.Count > 0)
            {
                dgvProdukt.Visibility = System.Windows.Visibility.Visible;
                con.Close();
            }
            else
            {
                dgvProdukt.Visibility = System.Windows.Visibility.Hidden;
                con.Close();
            }
            countRows();
            CalcSumPrice();
            dgvProdukt.Columns[0].Visibility = Visibility.Collapsed;
        }

        // Method to counts the rows from the DataGrid
        private void countRows()
        {
            int num = dgvProdukt.Items.Count - 1;
            lblCount.Content = "Rows: " + num.ToString() + " | ";
            lblCount.Visibility = Visibility.Visible;
        }

        // Clickevent from the Insert button to insert the dataset from the textboxes
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            con.Open();

            OleDbCommand cmdK = new OleDbCommand();
            cmdK.CommandText = "INSERT INTO [ProduktTab](WarenNummer, Bezeichnung, Beschreibung, Preis)Values(@WarenNummer, @Bezeichnung, @Beschreibung, @Preis)";
            cmdK.Parameters.AddWithValue("@WarenNummer", txbWarenNummer.Text);
            cmdK.Parameters.AddWithValue("@Bezeichnung", txbBezeichnung.Text);
            cmdK.Parameters.AddWithValue("@Beschreibung", txbBeschreibung.Text);
            cmdK.Parameters.AddWithValue("@Preis", txbPreis.Text);


            cmdK.Connection = con;
            int b = cmdK.ExecuteNonQuery();
            if (b > 0)
            {
                MessageBox.Show("Inserted");
                con.Close();
            }

            con.Close();
            BindGrid();
        }

        // ClickEvent from the Edit button to edit the dataset which is clicked and mirrored in the textboxes from the SelectedChange Event
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)dgvProdukt;
            DataRowView row_select = grid.SelectedItem as DataRowView;

            OleDbCommand cmd = new OleDbCommand();
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            con.Open();
            cmd.Connection = con;

            cmd.CommandText = "update ProduktTab set " +
                "WarenNummer = '" + txbWarenNummer.Text + "', " +
                "Bezeichnung = '" + txbBezeichnung.Text + "', " +
                "Beschreibung = '" + txbBeschreibung.Text + "', " +
                "Preis = '" + txbPreis.Text + "' " +
                "where ID_Produkt = " + row_select["ID_Produkt"].ToString() + "";
            cmd.ExecuteNonQuery();

            con.Close();
            BindGrid();
        }

        // ClickEvent from the Deletebutton. Deletes the selected dataset by checked ID
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgvProdukt.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)dgvProdukt.SelectedItems[0];

                OleDbCommand cmd = new OleDbCommand();
                if (con.State != ConnectionState.Open)
                    con.Open();
                cmd.Connection = con;
                cmd.CommandText = "delete from ProduktTab where ID_Produkt=" + row["ID_Produkt"].ToString();
                cmd.ExecuteNonQuery();
                BindGrid();
                MessageBox.Show("Löschen erfolgreich...");
                con.Close();
            }
            else
            {
                MessageBox.Show("Please Select Any Employee From List...");
                con.Close();
            }
        }

        // CleckEvent-Method to mirrors the dataset from the DataGrid to the Textboxes
        private void dgvProdukt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            DataRowView row_select = grid.SelectedItem as DataRowView;

            try
            {
                if (row_select != null)
                {
                    txb_ID_Produkt.Text = row_select["ID_Produkt"].ToString();
                    txbWarenNummer.Text = row_select["WarenNummer"].ToString();
                    txbBezeichnung.Text = row_select["Bezeichnung"].ToString();
                    txbBeschreibung.Text = row_select["Beschreibung"].ToString();
                    txbPreis.Text = row_select["Preis"].ToString();
                }
                //lblCountRows.Content = "|Rowcount: " + datagrid.Items.Count.ToString() + "|";
            }
            catch (Exception ex)
            {
                MessageBox.Show("null-Feld entdeckt\nBitte immer alle Werte eintragen\n\n" + ex);
            }
        }

        #region GotFocus_TXB
        #region GotFocus_WarenNummer
        private void txbWarenNummer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbWarenNummer.Text == "WarenNummer")
            {
                txbWarenNummer.Text = string.Empty;
            }
        }

        private void txbWarenNummer_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbWarenNummer.Text == "")
            {
                txbWarenNummer.Text = "WarenNummer";
            }
        }
        #endregion

        #region GotFocus_Bezeichnung
        private void txbBezeichnung_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbBezeichnung.Text == "Bezeichnung")
            {
                txbBezeichnung.Text = string.Empty;
            }
        }

        private void txbBezeichnung_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbBezeichnung.Text == "")
            {
                txbBezeichnung.Text = "Bezeichnung";
            }
        }
        #endregion

        #region GotFocus_Beschreibung
        private void txbBeschreibung_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbBeschreibung.Text == "Beschreibung")
            {
                txbBeschreibung.Text = string.Empty;
            }
        }

        private void txbBeschreibung_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbBeschreibung.Text == "")
            {
                txbBeschreibung.Text = "Beschreibung";
            }
        }
        #endregion

        #region GotFocus_Preis
        private void txbPreis_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbPreis.Text == "0,00")
            {
                txbPreis.Text = string.Empty;
            }
        }

        private void txbPreis_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbPreis.Text == "")
            {
                txbPreis.Text = "0,00";
            }
        }
        #endregion

        #region GotFocus_SearchTXB
        private void txtSucheProdukt_GotFocus(object sender, RoutedEventArgs e)
        {
            lblSucheProdukt.Visibility = Visibility.Hidden;
        }

        private void txtSucheProdukt_LostFocus(object sender, RoutedEventArgs e)
        {
            lblSucheProdukt.Visibility = Visibility.Visible;
        } 
        #endregion
        #endregion

        // Allows only digit and comma input to the price textbox
        private void txbPreis_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            bool approvedDecimalPoint = false;

            if (e.Text == ",")
            {
                if (!((TextBox)sender).Text.Contains(","))
                    approvedDecimalPoint = true;
            }

            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
                e.Handled = true;
        }

        #region FilterElements
        private Dictionary<string, string> _conditions = new Dictionary<string, string>();

        private void UpdateFilter()
        {
            var activeConditions = _conditions.Where(c => c.Value != null).Select(c => "(" + c.Value + ")");
            DataView dv = dgvProdukt.ItemsSource as DataView;
            dv.RowFilter = string.Join(" AND ", activeConditions);
        }

        private void txtSucheProdukt_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtSucheProdukt.Text;
            if (string.IsNullOrEmpty(filter))
                _conditions["ProduktTab"] = null;
            else
                _conditions["ProduktTab"] = string.Format("WarenNummer Like '%{0}%' " +
                                                       " OR Bezeichnung LIKE '%{0}%' " +
                                                       " OR Beschreibung LIKE '%{0}%' ", filter);
            UpdateFilter();
            countRows();
        }
        #endregion

    }
}
