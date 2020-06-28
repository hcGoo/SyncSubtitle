using System;
using System.IO;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SyncSubtitleHC
{

    public interface SyncSubtitle
    {
        void AdjustSync(String filepath, double syncTime);
    }

    public class SyncSubtitleSMI : SyncSubtitle
    {
        private const string searchText = "SYNC Start=";

        public void AdjustSync(string filepath, double syncTime)
        {

            string[] textLines = File.ReadAllLines(filepath);
            StringBuilder sb = new StringBuilder();
            
            string pattern = searchText + "\\d+";

            foreach (string line in textLines) {

                double value = 0.0;

                foreach (Match match in Regex.Matches(line, pattern)) {
                    string valueStr = match.Value;
                    valueStr = valueStr.Remove(0, searchText.Count());
                    value = int.Parse(valueStr);
                    value += (syncTime * 1000); // msec
                    if (value < 0)
                        value = 0;
                }

                sb.Append(Regex.Replace(line, pattern, searchText + value.ToString()));
                sb.Append("\n");
            }

            //File.WriteAllText(filepath + "__", sb.ToString());
            File.WriteAllText(filepath, sb.ToString());
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String[] files;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EventDropFile(object sender, DragEventArgs e)
        {
            files = e.Data.GetData(DataFormats.FileDrop) as String[];

            fileList.Items.Clear();
            foreach (var item in files) {
                fileList.Items.Add(item);
            }
        }

        private void Button_Click_Excute(object sender, RoutedEventArgs e)
        {
            if (files == null) {
                MessageBox.Show("Error : Drag and drop files that you want to modify");
                return;
            }

            double value;
            if(Double.TryParse(syncTime.Text, out value)) {

            }
            else {
                MessageBox.Show("Error : sync intput value");
                return;
            }

            // if.. smi
            var syncSubTitle = new SyncSubtitleSMI();
            foreach (var item in files) {
                syncSubTitle.AdjustSync(item, value);
            }

        }
    }
}
