using Microsoft.Win32;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
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

namespace ArgKmlEditorNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KMLFeatureTreeViewItem selectedKMLFeatureTreeViewItem = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Google Earth (.kml .kmz)|*.kml;*.kmz";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                KmlFile kmlFile = null;
                string fileName = openFileDialog1.FileName;

                string fileExtension = System.IO.Path.GetExtension(fileName);
                if (fileExtension.Equals(".kmz", StringComparison.OrdinalIgnoreCase)) {
                    try {
                        using(Stream fileStream = File.OpenRead(fileName)) {
                            KmzFile kmzFile = KmzFile.Open(fileStream);
                            string kmlFileString = kmzFile.ReadKml();
                            using(StringReader stringReader = new StringReader(kmlFileString)) 
                            {
                                kmlFile = KmlFile.Load(stringReader);
                            }
                        };
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Failed to open a KMZ file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                } else {
                    try {
                        using(Stream fileStream = File.OpenRead(fileName)) {
                            kmlFile = KmlFile.Load(fileStream);
                        };
                    } catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Failed to open a KML file", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                KmlTreeViewController kmlTreeViewController = new KmlTreeViewController();
                kmlTreeViewController.SetTreeView(this.KmlItemsTreeView);
                kmlTreeViewController.SetKML(kmlFile);
                kmlTreeViewController.ProcessKml();
                kmlTreeViewController.TreeViewSelectionChanged += c_TreeViewSelectionChanged;

                KmlSchemaComboBoxController kmlSchemaComboBoxController = new KmlSchemaComboBoxController();
                kmlSchemaComboBoxController.SetComboBox(this.SchemaSchemaListComboBox);
                kmlSchemaComboBoxController.SetKML(kmlFile);
                kmlSchemaComboBoxController.ProcessKml();
            }
        }

        void c_TreeViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedKMLFeatureTreeViewItem = e.kmlFeatureTreeViewItem;
            Feature feature = selectedKMLFeatureTreeViewItem.Feature;   

            NameTextBox.Text = feature.Name;
            DescriptionTextBox.Text = feature.Description != null ? feature.Description.Text : "";
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedKMLFeatureTreeViewItem != null)
            {
                Feature feature = selectedKMLFeatureTreeViewItem.Feature; 

                feature.Name = ((TextBox)sender).Text;
                selectedKMLFeatureTreeViewItem.Header = feature.Name;
            }
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedKMLFeatureTreeViewItem != null)
            {
                Feature feature = selectedKMLFeatureTreeViewItem.Feature;
                
                Description description = new Description();
                description.Text = ((TextBox)sender).Text;
                feature.Description = description;
            }
        }
    }
}
