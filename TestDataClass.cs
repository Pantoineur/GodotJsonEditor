using Godot;
using System;

namespace PluginSandbox.addons.GodotJsonEditor
{
    [DataClass]
    public class TestDataClass
    {
        [DataClass]
        public OtherDataClass Other { get; set; }

        public TestDataClass Other2 { get; set; }
    }

    public class OtherDataClass
    {
        public int Val1 { get; set; }
        public string Val2 { get; set; }
    }
}