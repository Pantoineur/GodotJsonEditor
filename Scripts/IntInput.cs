using Godot;
using System;

[Tool]
public class IntInput : DataClassInput
{
    SpinBox spinBox;
    public override void Init(string propName)
    {
        base.Init(propName);

        spinBox = GetNode<SpinBox>("Value");

        if (!spinBox.IsConnected("value_changed", this, nameof(SpinValueChanged)))
        {
            spinBox.Connect("value_changed", this, nameof(SpinValueChanged));
        }

        value = 0;
    }

    private void SpinValueChanged(float value)
    {
        this.value = (int)value;
    }

    public override void SetValue(object value)
    {
        base.SetValue(value);

        if (value is long valAsInt)
        {
            spinBox.Value = valAsInt;
            spinBox._Draw();
        }
    }
}
