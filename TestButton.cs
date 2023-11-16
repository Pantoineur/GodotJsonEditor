using Godot;
using System;

namespace PluginSandbox.addons.GodotJsonEditor
{
    [Tool]
    public class TestButton : Button
    {
        public override void _EnterTree()
        {
            Connect("pressed", this, "Clicked");
        }

        public void Clicked()
        {
            GD.Print("You clicked me!");
        }
    }
}