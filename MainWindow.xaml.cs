using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public DataTable datatable;

        private void Button_Click(object send, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog =
                new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Excel|*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                fileName.Content = dialog.FileName;
            }
        }

        private void Btn_InsertDistributorRows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                datatable = new DataTable();
                datatable.Columns.Add("Channel", typeof(string));
                datatable.Columns.Add("Percentage", typeof(int));
                if (dgDistributor.Items.Count > 0)
                {
                    datatable = ((DataView)dgDistributor.ItemsSource).Table;
                }

                var row = datatable.NewRow();
                //row["Channel"] = dgDistributor.Items.Count + 1;
                row["Percentage"] = 0;
                datatable.Rows.Add(row);

                dgDistributor.ItemsSource = datatable.DefaultView;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //删除条码表格行
        private void Btn_DeleteDistributorRows_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("Delete the last row?", "Notice", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)
                if (dgDistributor.Items.Count > 0)
                {
                    datatable = ((DataView)dgDistributor.ItemsSource).Table;
                    datatable.Rows.RemoveAt(dgDistributor.Items.Count - 1);
                }
            dgDistributor.ItemsSource = datatable.DefaultView;
        }

        private void Btn_InsertD2CRows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                datatable = new DataTable();
                datatable.Columns.Add("Channel", typeof(string));
                datatable.Columns.Add("Percentage", typeof(int));
                if (dgInsertD2C.Items.Count > 0)
                {
                    datatable = ((DataView)dgInsertD2C.ItemsSource).Table;
                }

                var row = datatable.NewRow();
                //row["Channel"] = dgInsertD2C.Items.Count + 1;
                row["Percentage"] = 0;
                datatable.Rows.Add(row);

                dgInsertD2C.ItemsSource = datatable.DefaultView;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //删除条码表格行
        private void Btn_DeleteD2CRows_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("Delete the last row?", "Notice", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)
                if (dgInsertD2C.Items.Count > 0)
                {
                    datatable = ((DataView)dgInsertD2C.ItemsSource).Table;
                    datatable.Rows.RemoveAt(dgInsertD2C.Items.Count - 1);
                }
            dgInsertD2C.ItemsSource = datatable.DefaultView;
        }

        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

        private void Btn_Execute_Click(object sender, RoutedEventArgs e)
        {
            string DistributorJSONString = string.Empty;
            string D2CJSONString = string.Empty;
            string DistributorJSON = string.Empty;
            string D2CJSON = string.Empty;
            bool DistributorOK = false;
            bool D2COK = false;
            
            if (dgDistributor.Items.Count > 0)
            {
                datatable = ((DataView)dgDistributor.ItemsSource).Table;
                var list = datatable.AsEnumerable().Select(c => c.Field<int>("Percentage")).ToList();
                int percentage = 0;
                foreach (var item in list)
                {
                    percentage += item;
                }

                if (percentage != 100)
                {
                    MessageBox.Show("Percentage is not 100% for Distributor!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }
                DistributorJSONString = DataTableToJSONWithJSONNet(datatable);
                DistributorJSON = DistributorJSONString.Replace("\"", "\\\"");
                //MessageBoxResult drDistributor = MessageBox.Show(DistributorJSON, "Distributor", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                DistributorOK = true;
            }
            else
            {
                MessageBox.Show("Please add channel for Distributor!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return;
            }

            if (dgInsertD2C.Items.Count > 0)
            {
                datatable = ((DataView)dgInsertD2C.ItemsSource).Table;
                var list = datatable.AsEnumerable().Select(c => c.Field<int>("Percentage")).ToList();
                int percentage = 0;
                foreach (var item in list)
                {
                    percentage += item;
                }

                if (percentage != 100)
                {
                    MessageBox.Show("Percentage is not 100% for D2C!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }
                D2CJSONString = DataTableToJSONWithJSONNet(datatable);
                D2CJSON = D2CJSONString.Replace("\"", "\\\"");
                //MessageBoxResult drD2C = MessageBox.Show(D2CJSON, "D2C", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                D2COK = true;
            }
            else
            {
                MessageBox.Show("Please add channel for D2C!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return;
            }

            if (DistributorOK && D2COK)
            {
                string tempFile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".csv";
                Label executeFileName = fileName;
                //MessageBoxResult dr2 = MessageBox.Show(executeFileName.Content + "", "JSON", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                //ProcessStartInfo startInfo = new ProcessStartInfo("D:\\processor.exe");
                //startInfo.Arguments = "/k -in \"" + executeFileName.Content + "\"  -out \"" + tempFile + "\"  -distributor '" + DistributorJSON + "' -d2c '" + D2CJSON + "'";
                Process process = new Process();
                process.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "processor.exe";
                process.StartInfo.Arguments = "-in \"" + executeFileName.Content + "\"  -out \"" + tempFile + "\"  -distributor " + DistributorJSON + " -d2c " + D2CJSON;
                //process.StartInfo.RedirectStandardInput = true;
                //process.StartInfo.RedirectStandardOutput = true;
                //process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit();

                Stream myStream;
                Microsoft.Win32.SaveFileDialog saveFileDialog =
                    new Microsoft.Win32.SaveFileDialog();

                saveFileDialog.FileName = "rename";
                saveFileDialog.Filter = "CSV|*.csv";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    if ((myStream = saveFileDialog.OpenFile()) != null)
                    {
                        using (var fileStream = new FileStream(tempFile, FileMode.Open))
                        {
                            fileStream.CopyTo(myStream);
                        }
                        myStream.Close();
                    }
                }
            }
        }
    }
}
