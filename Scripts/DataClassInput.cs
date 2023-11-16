using Godot;
using System;

[Tool]
public abstract class DataClassInput : Control
{
    protected string propName;
    public string PropName
    {
        get
        {
            return propName;
        }
        set
        {
            propName = value;
        }
    }

    protected object value;
    public object Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }

    public virtual void Init(string propName)
    {
        this.propName = propName;
        GetNode<Label>("Name").Text = propName;
    }

    public virtual void SetValue(object value)
    {
        this.value = value;
    }

    public DataType TypeToDataType()
    {
        switch(value.GetType().Name)
        {
            case nameof(Int16):
            case nameof(Int32):
            case nameof(Int64):
                return DataType.Int;
            case nameof(UInt16):
            case nameof(UInt32):
            case nameof(UInt64):
                // TODO
                break;
            case nameof(Double):
            case nameof(Single):
                return DataType.Float;
            case nameof(String):
                return DataType.String;
        }

        return DataType.Int;
    }
}
