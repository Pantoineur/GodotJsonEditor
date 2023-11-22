using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginSandbox.addons.GodotJsonEditor.Extensions
{
    public static class TypeExtensions
    {
        public static DataType ToDataType(this Type type)
        {
            if(type.GetCustomAttribute<DataClassAttribute>() != null)
            {
                return DataType.Object;
            }

            switch (type.Name)
            {
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                    return DataType.Int;
                case nameof(UInt16):
                case nameof(UInt32):
                case nameof(UInt64):
                    // TODO handle uint correctly
                    return DataType.Int;
                case nameof(Double):
                case nameof(Single):
                    return DataType.Float;
                case nameof(String):
                    return DataType.String;
                default:
                    return DataType.Int;
            }
        }
    }
}
