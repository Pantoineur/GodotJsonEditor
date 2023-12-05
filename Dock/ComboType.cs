using Godot;
using System;
using System.Xml.Linq;

[Tool]
public class ComboType : OptionButton
{
    private bool filled = false;
    public override void _EnterTree()
    {
        if(!filled)
        {
            foreach (string name in Enum.GetNames(typeof(DataType)))
            {
                GD.Print($"Found type {name}");
                AddItem(name);
            }

            filled = true;
        }
    }
}
