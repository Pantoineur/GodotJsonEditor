using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataObject
{
    public string PropName { get; set; }
    public object Value { get; set; }
    public DataType DataType { get; set; }
    public bool FromExistingType { get; set; }
    public Type BaseType { get; set; }
}
