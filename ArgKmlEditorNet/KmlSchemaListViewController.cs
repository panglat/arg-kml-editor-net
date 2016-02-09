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
    public class KmlSchemaListViewController
    {
        ListView _listView = null;
        Schema _schema = null;

        public void SetListView(ListView listView)
        {
            _listView = listView;
        }

        public void SetSchema(Schema schema)
        {
            _schema = schema;
            ProcessSchema();
        }

        void ProcessSchema()
        {
            _listView.Items.Clear();
            if (_schema != null)
            {
                _schema.Fields.ToList().ForEach(s =>
               {
                   SchemaTypeEnum type = SchemaTypeEnum.unknown;
                   Enum.TryParse<SchemaTypeEnum>(s.FieldType, true, out type);

                   _listView.Items.Add(new KmlSchemaWithValueDto()
                   {
                       Name = s.Name,
                       Type = type
                   });
                });
            }
        }
    }
}
