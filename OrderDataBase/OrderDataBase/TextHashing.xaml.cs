using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Window = System.Windows.Window;

namespace OrderDataBase
{
    /// <summary>
    /// Interaktionslogik für TextHashing.xaml
    /// </summary>
    public partial class TextHashing : Window
    {
        public TextHashing()
        {
            InitializeComponent();
        }

        private void btnHashProcess_Click(object sender, RoutedEventArgs e)
        {
            var str = txtInHash.Text;
            var password = "DKCryPty";
            var strEncryptred = Cipher.Encrypt(str, password);
            var strDecrypted = Cipher.Decrypt(strEncryptred, password);
            lblOutHash.Content = strEncryptred.ToString();
            lblOutText.Content = strDecrypted.ToString();
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DateTime time = DateTime.Now;
            double neueZahl = time.Millisecond;

            if (neueZahl > 0 && neueZahl <= 500)
            {
                double zufallUEins = ((neueZahl - 45) / Math.Pow(neueZahl, 2) + Math.Sqrt(neueZahl) * 2 / 75);

                MessageBox.Show("" + neueZahl + "  " + Math.Round(zufallUEins, 4));
            }else if (neueZahl > 500 && neueZahl <= 1000)
            {
                double zufallUEins = ((neueZahl + 87) / Math.Pow(neueZahl, 2) + Math.Sin(neueZahl) * 3 / 91);

                MessageBox.Show("" + neueZahl + "  " + Math.Round(zufallUEins, 2));
            }
            else
            {
                MessageBox.Show("NullFeld");
            }
        }

        private void btnTextToString_Click(object sender, RoutedEventArgs e)
        {
            // String to arraystring
            // String
            string str = txbTextToStringArr.Text;
            // Aus String einen String Array welcher bei ' ' getrennt wird
            string[] strarray = str.Split(' ');
            // Aus String Beispiel "Hello" wird ein char Array Beispiel {'H','e','l','l','o'}
            char[] chArr = str.ToCharArray();
            // Fügt das char Array wieder zu einem String zusammen
            string unitedChars = new string(chArr);
            string unitedChars1 = String.Concat(chArr);

            for (int i = 0; i < strarray.Length; i++)
            {
                lblStringToArray.Content += strarray[i] + "\n";
            }

            //lblStringToArray.Content += "\n\n" + strarray[1];
            // Durchsucht str nach 'x' - Zeichen und gibt Anzahl davon in MessageBox aus
            int errorCounter = 0;
            errorCounter = Regex.Matches(str, @"[x]").Count;
            MessageBox.Show("" + errorCounter.ToString());

            // Durchläuft das chararray einzeln durch und gibt jedes Zeichen einzeln und hintereinnader in einer Messagbox aus
            foreach (var item in chArr)
            {
                MessageBox.Show("" + item);
            }

            MessageBox.Show("UnitedChars: " + unitedChars1);
        }
    }
}
