using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace OrderDataBase
{
    /// <summary>
    /// Interaktionslogik für KundeAnlegen.xaml
    /// </summary>
    public partial class KundeAnlegen : Window
    {
        OleDbConnection con;
        DataTable dt;
        DispatcherTimer timer = new DispatcherTimer();

        public KundeAnlegen()
        {
            InitializeComponent();
        }

        private void BindGrid()
        {
            OleDbCommand cmd = new OleDbCommand();
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            cmd.CommandText = "select * from KundenTab";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dgvKunde.ItemsSource = dt.AsDataView();
            con.Close();

            if (dt.Rows.Count > 0)
            {
                dgvKunde.Visibility = System.Windows.Visibility.Visible;
                dgvKunde.DataContext = dt;
                con.Close();
            }
            else
            {
                dgvKunde.Visibility = System.Windows.Visibility.Hidden;
                con.Close();
            }
            countRows();           
        }

        private void countRows()
        {
            int num = dgvKunde.Items.Count - 1;
            lblCount.Content = "Rows: " + num.ToString() + " | ";
            lblCount.Visibility = Visibility.Visible;
        }

        public void usageCPU()
        {
            var cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            Thread.Sleep(100);
            var firstCall = cpuUsage.NextValue();

            Thread.Sleep(100);
            lblCPU.Content = Math.Round(cpuUsage.NextValue(), 2) + "%";
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            usageCPU();
        }

        public void GPU()
        {
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string cmbInfoText = cmbInfo.SelectedItem.ToString().Substring(cmbInfo.SelectedItem.ToString().LastIndexOf(" "));
                    lblGraKa.Content = obj[cmbInfoText.TrimStart()];

                    #region CharArrToString
                    //char[] cmbInfoArr = cmbInfoText.ToArray();
                    //MessageBox.Show(cmbInfoText.TrimStart());

                    //for (int i = 0; i < cmbInfoArr.Length; i++)
                    //{
                    //    MessageBox.Show(cmbInfoArr[i] + "");
                    //}

                    //input.Substring(input.LastIndexOf("/"));
                    //Convert.ToString(cmbInfo.SelectedItem); 
                    #endregion
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            BindGrid();


            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            con.Open();

            OleDbCommand cmdK = new OleDbCommand();
            cmdK.CommandText = "INSERT INTO [KundenTab](VorName, NachName, Strasse, PLZ, AlterPerson)Values(@VorName, @NachName, @Strasse, @PLZ, @AlterPerson)";
            cmdK.Parameters.AddWithValue("@VorName", txbVorname.Text);
            cmdK.Parameters.AddWithValue("@NachName", txbNachname.Text);
            cmdK.Parameters.AddWithValue("@Strasse", txbStrasse.Text);
            cmdK.Parameters.AddWithValue("@PLZ", txbPLZ.Text);
            cmdK.Parameters.AddWithValue("@AlterPerson", txbAlter.Text);

            cmdK.Connection = con;
            int b = cmdK.ExecuteNonQuery();
            if (b > 0)
            {
                //MessageBox.Show("Inserted");
                con.Close();
            }
            con.Close();

            con.Open();
            OleDbCommand cmdL = new OleDbCommand();
            cmdL.CommandText = "INSERT INTO [LogInDB](Mail, PassWort)Values(@Mail, @PassWort)";
            var str = txbPassword.Text;
            var password = "DKCryPty";
            var strEncryptred = Cipher.Encrypt(str, password);
            //var strDecrypted = Cipher.Decrypt(strEncryptred, password);
            cmdL.Parameters.AddWithValue("@Mail", txbMail.Text);
            cmdL.Parameters.AddWithValue("@PassWort", strEncryptred);



            cmdL.Connection = con;
            int a = cmdL.ExecuteNonQuery();
            if (a > 0)
            {
                MessageBox.Show("Inserted");
                con.Close();
            }
            con.Close();
            BindGrid();
        }

        #region GotFocus_TXB
        #region GotFocus_VornameTXB
        private void txbVorname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbVorname.Text == "Vorname")
            {
                txbVorname.Text = string.Empty;
            }  
        }

        private void txbVorname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbVorname.Text == "")
            {
                txbVorname.Text = "Vorname";
            }        
        }
        #endregion

        #region GotFocus_NachnameTXB
        private void txbNachname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbNachname.Text == "Nachname")
            {
                txbNachname.Text = string.Empty;
            }
        }

        private void txbNachname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbNachname.Text == "")
            {
                txbNachname.Text = "Nachname";
            }     
        }
        #endregion

        #region GotFocus_AlterTXB
        private void txbAlter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbAlter.Text == "Alter")
            {
                txbAlter.Text = string.Empty;
            }            
        }

        private void txbAlter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbAlter.Text == "")
            {
                txbAlter.Text = "Alter";
            }          
        }
        #endregion

        #region GotFocus_StrasseTXB
        private void txbStrasse_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbStrasse.Text == "Strasse")
            {
                txbStrasse.Text = string.Empty;
            }

        }

        private void txbStrasse_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbStrasse.Text == "")
            {
                txbStrasse.Text = "Strasse";
            }
            
        }
        #endregion

        #region GotFocus_PLZTXB
        private void txbPLZ_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbPLZ.Text == "PLZ")
            {
                txbPLZ.Text = string.Empty;
            }
            
        }

        private void txbPLZ_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbPLZ.Text == "")
            {
                txbPLZ.Text = "PLZ";
            }
            
        }
        #endregion

        #region GotFocus_MailTXB
        private void txbMail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbMail.Text == "Mail")
            {
                txbMail.Text = string.Empty;
            }
            
        }

        private void txbMail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbMail.Text == "")
            {
                txbMail.Text = "Mail";
            }
            
        }
        #endregion

        #region GotFocus_PasswortTXB
        private void txbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbPassword.Text == "Password")
            {
                txbPassword.Text = string.Empty;
            }
            
        }

        private void txbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbPassword.Text == "")
            {
                txbPassword.Text = "Password";
            }
            
        }
        #endregion

        #region GotFocus_SucheTXT
        private void txtSuche_GotFocus(object sender, RoutedEventArgs e)
        {
            lblSuche.Visibility = Visibility.Hidden;
        }

        private void txtSuche_LostFocus(object sender, RoutedEventArgs e)
        {
            lblSuche.Visibility = Visibility.Visible;
        } 
        #endregion

        #endregion

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)dgvKunde;
            DataRowView row_select = grid.SelectedItem as DataRowView;

            OleDbCommand cmd = new OleDbCommand();
            con = new OleDbConnection();
            con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";
            con.Open();
            cmd.Connection = con;

            cmd.CommandText = "update KundenTab set " +
                "VorName = '" + txbVorname.Text + "', " +
                "NachName = '" + txbNachname.Text + "', " +
                "Strasse = '" + txbStrasse.Text + "', " +
                "PLZ = '" + txbPLZ.Text + "', " +
                "AlterPerson = '" + txbAlter.Text + "' " +
                "where ID_Kunde = " + row_select["ID_Kunde"].ToString() + "";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = "update LoginDB set " +
            //    "Mail = '" + txbMail.Text + "', " +
            //    "PassWort = '" + txbPassword.Text + "', " +
            //    "where ID_Kunde = " + row_select["ID_Kunde"].ToString() + "";
            //cmd.ExecuteNonQuery();
            con.Close();
            BindGrid();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgvKunde.SelectedItems.Count > 0)
            {
                DataRowView row = (DataRowView)dgvKunde.SelectedItems[0];

                OleDbCommand cmd = new OleDbCommand();
                if (con.State != ConnectionState.Open)
                    con.Open();
                cmd.Connection = con;
                cmd.CommandText = "delete from KundenTab where ID_Kunde=" + row["ID_Kunde"].ToString();
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

        private void dgvKunde_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            DataRowView row_select = grid.SelectedItem as DataRowView;

            try
            {
                if (row_select != null)
                {
                    txtID.Text = row_select["ID_Kunde"].ToString();
                    txbVorname.Text = row_select["VorName"].ToString();
                    txbNachname.Text = row_select["NachName"].ToString();
                    txbStrasse.Text = row_select["Strasse"].ToString();
                    txbPLZ.Text = row_select["PLZ"].ToString();
                    txbAlter.Text = row_select["AlterPerson"].ToString();
                }
                //lblCountRows.Content = "|Rowcount: " + datagrid.Items.Count.ToString() + "|";
            }
            catch (Exception ex)
            {
                MessageBox.Show("null-Feld entdeckt\nBitte immer alle Werte eintragen\n\n" + ex);
            }
        }

        private void btnHauptfenster_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainW = new MainWindow();
            mainW.Show();
            this.Close();
        }

        private void dgvKunde_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        #region FilterFunktion
        private Dictionary<string, string> _conditions = new Dictionary<string, string>();

        private void UpdateFilter()
        {
            var activeConditions = _conditions.Where(c => c.Value != null).Select(c => "(" + c.Value + ")");
            DataView dv = dgvKunde.ItemsSource as DataView;
            dv.RowFilter = string.Join(" AND ", activeConditions);
        }

        private void txtSuche_TextChanged(object sender, TextChangedEventArgs e)
        {

            string filter = txtSuche.Text;
            if (string.IsNullOrEmpty(filter))
                _conditions["KundenTab"] = null;
            else
                _conditions["KundenTab"] = string.Format("VorName Like '%{0}%' " +
                                                       " OR NachName LIKE '%{0}%' " +
                                                       " OR Strasse LIKE '%{0}%' " +
                                                       " OR PLZ LIKE '%{0}%' " +
                                                       " OR AlterPerson LIKE '%{0}%'", filter);
            UpdateFilter();
            countRows();
        }


        #endregion

        #region Professionellere Variante Filtern Datagrid
        /* Data-Grid-Echtzeit-Filterfunktion:
* Das Filtern einer 'DataGrid'-Instanz in WPF ist keinesfalls leicht,
* sofern man es programmiertechnisch lösen möchte; wie du es hier wolltest.
* Denn normalerweise müsste man zunächst im WPF-Sil eine Klasse deiner KundenTab erstellen,
* die jedem Element eine Kunden_ID, VorName, ..., zuweist, analog zu deiner Access-DB.
* Das würde doppelt Arbeit bedeuten und ist dennoch die WPF-Lösung (in WinForm wäre dies deutlich einfacher).
* Unabhänig davon habe ich es versucht, eine Lösung mit meinem eingen Stil, nämlich rein programmiertechn., zu finden;
* was mir mit unterem Code gelungen ist und einem viel Arbeit mit dem WPF-DB-Analogon spart. 
* Falls das Kompilieren bei dir nicht funktioniert, stelle oben statt "x64" auf "Any CPU"; dann funktioniert es wieder.
*/
        //private void txtSuche_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    // Fenster bei jeder Textänderung leeren
        //    dgvKunde.ItemsSource = null;

        //    // DB durch erneutes Einladen aktualisieren
        //    con = new OleDbConnection();
        //    con.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\ShopDB.accdb";

        //    con.Open();

        //    OleDbCommand search_cmd = new OleDbCommand();
        //    string cmd_search_str = "select * from [KundenTab] WHERE "
        //                            + "(ID_Kunde Like ('" + txtSuche.Text + "')) "
        //                            + "OR (VorName LIKE ('" + txtSuche.Text + "%')) "
        //                            + "OR (NachName LIKE ('" + txtSuche.Text + "%')) "
        //                            + "OR (VorName + ' ' + NachName LIKE ('" + txtSuche.Text + "%')) "
        //                            + "OR (Strasse LIKE ('" + txtSuche.Text + "%')) "
        //                            + "OR (PLZ LIKE ('" + txtSuche.Text + "%')) "
        //                            + "OR (AlterPerson LIKE ('" + txtSuche.Text + "%'))";

        //    // Parameter übernehmen und ItemsSource mit gefilterten Daten festlegen 
        //    search_cmd.CommandText = cmd_search_str;
        //    search_cmd.Connection = con;
        //    OleDbDataReader db_rd = search_cmd.ExecuteReader();
        //    dgvKunde.ItemsSource = db_rd;
        //} // txtSuche_TextChanged

        //private void txtSuche_OnFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        //{
        //    if (txtSuche.Text == "Filter-Text hier eingeben....")
        //        txtSuche.Text = string.Empty;
        //} // txtSuche_OnFocus  
        #endregion

        private void cmbInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GPU();
        }
    }
}
