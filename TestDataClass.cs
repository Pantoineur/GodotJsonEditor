using Godot;
using System;

namespace PluginSandbox.addons.GodotJsonEditor
{
    [DataClass]
    public class TestDataClass
    {
        public OtherDataClass Other { get; set; }
    }

    public class OtherDataClass
    {
        public int Val1 { get; set; }
        public string Val2 { get; set; }
    }
}