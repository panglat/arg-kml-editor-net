using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgKmlEditorNet.DTO
{
    public class KmlSchemaWithValueDto
    {
        public string Name { get; set; }
        public SchemaTypeEnum Type { get; set; }
        public string Value { get; set; }
    }
}
