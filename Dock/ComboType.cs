using Godot;
using System;
using System.Xml.Linq;

[Tool]
public class ComboType : OptionButton
{
    public override void _EnterTree()
    {
        GD.Print($"BLABLABLOU");
        foreach (string name in Enum.GetNames(typeof(DataType)))
        {
            GD.Print($"Found type {name}");
            AddItem(name);
        }
    }
}
