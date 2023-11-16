using Godot;
using System;

[Tool]
public class StringInput : DataClassInput
{
    LineEdit lineEdit;
    public override void Init(string propName)
    {
        base.Init(propName);

        lineEdit = GetNode<LineEdit>("Value");

        if (!lineEdit.IsConnected("text_changed", this, nameof(TextValueChanged)))
        {
            lineEdit.Connect("text_changed", this, nameof(TextValueChanged));
        }

        value = "";
    }

    private void TextValueChanged(string value)
    {
        this.value = value;
    }

    public override void SetValue(object value)
    {
        base.SetValue(value);
        lineEdit.Text = (string)value;
    }
}
