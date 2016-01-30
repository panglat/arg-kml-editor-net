using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ArgKmlEditorNet
{

    public class KmlSchemaComboBoxSelectionChangedEventArgs : EventArgs
    {
        public Schema schemaItem { get; set; }
    }

    class KmlSchemaNameComboBoxController
    {
        KmlFile _kmlFile = null;
        ComboBox _comboBox = null;

        public event EventHandler<KmlSchemaComboBoxSelectionChangedEventArgs> ComboBoxSelectionChanged;

        public void SetKML(KmlFile kmlFile)
        {
            _kmlFile = kmlFile;
        }

        public void SetComboBox(ComboBox comboBox)
        {
            _comboBox = comboBox;
            comboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
        }

        public void ProcessKml()
        {
            if (_kmlFile == null) /*|| _treeView == null)*/ return;
            Kml kml = _kmlFile.Root as Kml;
            if (kml != null)
            {
                Feature feature = kml.Feature;
                _comboBox.Items.Clear();
                ProcessFeature(feature);
            }
        }

        void ProcessFeature(Feature feature)
        {
            if (feature is Document)
            {
                ProcessDocument(feature as Document);
            }
        }

        void ProcessDocument(Document document)
        {
            if (document.Schemas != null)
            {
                document.Schemas.ToList().ForEach(s => {
                    _comboBox.Items.Add(new KmlSchemaComboBoxItem()
                    {
                        Schema = s,
                        Content = s.Name
                    });
                });
            }
        }

        void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            KmlSchemaComboBoxItem kmlSchemaComboBoxItem = (KmlSchemaComboBoxItem)((ComboBox)sender).SelectedItem;
            Schema schema = kmlSchemaComboBoxItem.Schema;

            EventHandler<KmlSchemaComboBoxSelectionChangedEventArgs> handler = ComboBoxSelectionChanged;
            if (handler != null)
            {
                handler(this, new KmlSchemaComboBoxSelectionChangedEventArgs() { schemaItem = schema });
            }
        }
    }
}
