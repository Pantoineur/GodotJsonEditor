using Godot;
using System;

[Tool]
public class FloatInput : DataClassInput
{
    SpinBox spinBox;
    public override void Init(string propName)
    {
        base.Init(propName);

        spinBox = GetNode<SpinBox>("Value");
        spinBox.Step = 0.1; // TODO Moduler

        if (!spinBox.IsConnected("value_changed", this, nameof(SpinValueChanged)))
        {
            spinBox.Connect("value_changed", this, nameof(SpinValueChanged));
        }

        value = 0;
    }

    private void SpinValueChanged(float value)
    {
        this.value = value;
    }

    public override void SetValue(object value)
    {
        base.SetValue(value);

        if (value is double valueAsDouble)
        {
            spinBox.Value = valueAsDouble;
        }
    }
}
