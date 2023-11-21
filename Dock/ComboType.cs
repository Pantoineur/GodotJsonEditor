using Godot;
using System;

public class ComboType : OptionButton
{
    public override void _EnterTree()
    {
        foreach (string name in Enum.GetValues(typeof(DataType)))
        {
            AddItem(name);
        }
    }
}
