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

    private bool isInit = false;
    public bool IsInit { get { return isInit; } }

    public virtual void Init(string propName)
    {
        this.propName = propName;
        GetNode<Label>("Name").Text = propName;
        isInit = true;
    }

    public virtual void SetValue(object value, Type type = null)
    {
        this.value = value;
    }

    public virtual void Clear()
    {

    }
}
