using System;
using System.Collections.Generic;
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
using FolderLoader.ViewModel;

namespace FolderLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables and Properties

        private object dummyNode = null;

        private long value = 0;

        public string SelectedImagePath { get; set; }


        static readonly string[] FolderSuffixes =
                  { "bytes", "KB", "MB", "GB", "TB" };

        #endregion

        #region Methods and Events

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Store the size specified by the user and disable button
        /// </summary>      
        private void ShowBasedOnSizeBtn_Click(object sender, RoutedEventArgs e)
        {

            value = long.Parse(FileSize.Text);
            btnShow.IsEnabled = false;

        }

        /// <summary>
        /// Convert from bytes to appropriate size
        /// </summary>
        static string FolderSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + FolderSuffix(-value); }

            int i = 0;
            decimal decValue = (decimal)value;
            while (Math.Round(decValue, decimalPlaces) >= 1000)
            {
                decValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", decValue, FolderSuffixes[i]);
        }


        /// <summary>
        /// Load the Drives in the system when application loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (string s in Directory.GetLogicalDrives())
            {
                try
                {
                    TreeViewItem item = new TreeViewItem();
                    DriveInfo info = new DriveInfo(s);
                    long usedSize = info.TotalFreeSpace;
                    String sizeSuffix = FolderSuffix(usedSize);
                    item.Header = s + "          " +  sizeSuffix;
                    item.Tag = s;
                    item.FontWeight = FontWeights.Normal;
                    item.Items.Add(dummyNode);
                    item.Expanded += new RoutedEventHandler(folder_Expanded);
                    foldersItem.Items.Add(item);
                }
                catch (Exception) { }

            }

        }

        /// <summary>
        /// Async event that displays subfolders when folders expanded
        /// </summary>
        async void folder_Expanded(object sender, RoutedEventArgs e)
        {
            btnShow.IsEnabled = false;
            TreeViewItem item = (TreeViewItem)sender;

            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        await Task.Run(() => FolderViewModel.CalculateFolderSize(s));
                        try
                        {
                            TreeViewItem subitem = new TreeViewItem();

                            string subTemp = s.Substring(s.LastIndexOf("\\") + 1);
                            FileInfo info = new FileInfo(s);

                            long usedSize = FolderViewModel.CalculateFolderSize(s);
                            String sizeSuffix = FolderSuffix(usedSize);
                            if (usedSize > value)
                            {
                                subitem.Header = subTemp + "          " + sizeSuffix;
                                subitem.Tag = s;
                                subitem.FontWeight = FontWeights.Normal;
                                subitem.Items.Add(dummyNode);
                                subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                                item.Items.Add(subitem);
                            }
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }
            }
        }


        /// <summary>
        /// Loads the image from header when item selection is changed
        /// </summary>
        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }


        }

        #endregion

    }
}
