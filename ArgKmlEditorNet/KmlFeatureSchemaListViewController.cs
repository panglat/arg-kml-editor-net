using ArgKmlEditorNet.DTO;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ArgKmlEditorNet
{
    public class KmlFeatureSchemaListViewController
    {
        ListView _listView = null;
        Feature _feature = null;

        public void SetListView(ListView listView)
        {
            _listView = listView;
        }

        public void SetFeature(Feature feature)
        {
            _feature = feature;
            ProcessSchema();
        }

        void ProcessSchema()
        {
            _listView.Items.Clear();
            if (_feature.ExtendedData != null)
            {
                if (_feature.ExtendedData.SchemaData != null)
                {
                    
                    SchemaData schemaData = _feature.ExtendedData.SchemaData.FirstOrDefault();
                    if(schemaData != null) { 
                        string originalName = _feature.ExtendedData.SchemaData.First().SchemaUrl.OriginalString;
                        if (originalName.StartsWith("#"))
                        {
                            originalName = originalName.Substring(1);
                        }
                        schemaData.SimpleData.ToList().ForEach(d =>
                        {
                            _listView.Items.Add(new KmlSchemaWithValueDto()
                            {
                                Name = d.Name,
                                Value = d.Text
                            });
                        });
                    }
                }
            }
        }
    }
}
