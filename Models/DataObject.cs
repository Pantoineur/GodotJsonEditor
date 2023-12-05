using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataObject
{
    public string PropName { get; set; }
    public DataType DataType { get; set; }
    public object Value { get; set; }

    [JsonIgnore]
    public Type BaseType { get; set; }
}
