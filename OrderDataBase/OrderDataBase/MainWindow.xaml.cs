/*
    Class: MainWindow
    Workingdes.: To navigate through the OrderMenues
 */

using System;
using System.Diagnostics;
using System.Windows;
using ConTest_WPF_CON_CS;

namespace OrderDataBase
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void KundenTab_Click(object sender, RoutedEventArgs e)
        {
            KundeAnlegen kTab = new KundeAnlegen();
            kTab.Show();
            this.Close();
        }

        private void ProdukteTab_Click(object sender, RoutedEventArgs e)
        {
            ProdukteBestellungen produkte = new ProdukteBestellungen();
            produkte.Show();
            this.Close();
        }

        private void OrderView_Click(object sender, RoutedEventArgs e)
        {
            OrderView Orderview = new OrderView();
            Orderview.Show();
            this.Close();
        }

        private void TextHashing_Click(object sender, RoutedEventArgs e)
        {
            TextHashing tH = new TextHashing();
            tH.Show();
            this.Close();
        }

        private void ExcelWorkbook_Click(object sender, RoutedEventArgs e)
        {
            TextHashing tH = new TextHashing();
            tH.Show();
            this.Close();
        }

        private void CMD_Click(object sender, RoutedEventArgs e)
        {
            //Program.Main(null);
            Process.Start(@"C:\Users\Davor\source\repos\OrderDataBase\ConTest_WPF_CON_CS\bin\Release\ConTest_WPF_CON_CS.exe");
        }
    }
}
