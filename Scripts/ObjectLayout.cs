using Godot;
using PluginSandbox.addons.GodotJsonEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

[Tool]
public class ObjectLayout : DataClassInput
{
    Button btnAddProp;

    ConfirmationDialog cdAddProp;

    PackedScene intInputScene;
    PackedScene floatInputScene;
    PackedScene stringInputScene;
    PackedScene objectLayoutScene;

    VBoxContainer vbProps;

    HashSet<string> alreadyParsedTypes = new HashSet<string>();

    public override void Init(string propName)
    {
        this.propName = propName;
        GetNode<Label>("VB/Name").Text = propName;

        intInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/IntInput.tscn");
        floatInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/FloatInput.tscn");
        stringInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/StringInput.tscn");
        objectLayoutScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/ObjectLayout.tscn");

        btnAddProp = GetNode<Button>("VB/AddProp");
        btnAddProp.Connect("pressed", this, nameof(ShowAddPropPanel));

        vbProps = GetNode<VBoxContainer>("VB/VBProps");

        cdAddProp = GetNode<ConfirmationDialog>("../AddPropP/AddProp");
        cdAddProp.Connect("confirmed", this, nameof(AddPropDialogConfirmed));
        cdAddProp.Connect("custom_action", this, nameof(TestCa));

        IsInit = true;
    }

    public void TestCa(string action)
    {
        GD.Print($"Custom action is {action}");
    }

    public void InstantiateDataInput(DataObject dataObject)
    {
        Node inputNode = null;
        DataClassInput inputInstance = null;
        switch (dataObject.DataType)
        {
            case DataType.Int:
                inputNode = intInputScene.Instance();
                inputInstance = inputNode.GetNode<IntInput>(nameof(IntInput));
                break;
            case DataType.Float:
                inputNode = floatInputScene.Instance();
                inputInstance = inputNode.GetNode<FloatInput>(nameof(FloatInput));
                break;
            case DataType.String:
                inputNode = stringInputScene.Instance();
                inputInstance = inputNode.GetNode<StringInput>(nameof(StringInput));
                break;
            case DataType.Object:
                inputNode = objectLayoutScene.Instance();
                inputInstance = inputNode.GetNode<ObjectLayout>(nameof(ObjectLayout));
                inputInstance.Init(dataObject.PropName);

                inputInstance.MarginLeft = MarginLeft + 30;
                inputInstance.MarginRight = MarginRight + 30;

                if(dataObject.BaseType != null && !string.IsNullOrEmpty(dataObject.BaseType.Name))
                {
                    (inputInstance as ObjectLayout).InstantiateFromType(dataObject.BaseType);
                }

                break;
        }

        if(!inputInstance.IsInit)
        {
            inputInstance.Init(dataObject.PropName);
        }        

        if (dataObject.Value != null)
        {
            inputInstance.SetValue(dataObject.Value, dataObject.BaseType);
        }

        ActiveNodes.Add(inputNode);
        Properties.Add(inputInstance);
        vbProps.AddChild(inputNode);
    }

    public void InstantiateFromType(Type type)
    {
        foreach (PropertyInfo prop in type.GetProperties())
        {
            if(prop.PropertyType == type)
            {
                GD.PrintErr($"Type {type.Name} has a property of its own type. It is ignored for infinite loop reasons.");
            }
            else
            {
                if(alreadyParsedTypes.Add(prop.PropertyType.Name))
                {
                    InstantiateDataInput(new DataObject() { DataType = prop.PropertyType.ToDataType(), PropName = prop.Name, BaseType = prop.PropertyType });
                }
                else
                {
                    GD.PrintErr($"Type '{prop.PropertyType.Name}' was already parsed in this hierarchy. Ignoring it to avoid infinite loop.");
                }
            }            
        }
    }

    public override void SetValue(object value, Type type = null)
    {
        base.SetValue(value);

        GD.Print($"object value {value} base type {type}");

        if(value is List<DataClassInput> properties && type != null)
        {

        }
    }

    public void ShowAddPropPanel()
    {

    }

    public void AddPropDialogConfirmed()
    {

    }

    public override void Clear()
    {
        foreach (DataClassInput inputNode in Properties.ToArray())
        {
            inputNode.Clear();
            inputNode.QueueFree();
        }
        foreach (Node node in ActiveNodes.ToArray())
        {
            node.QueueFree();
        }

        Properties.Clear();
        ActiveNodes.Clear();
    }

    public List<DataClassInput> Properties { get; set; } = new List<DataClassInput>();
    public List<Node> ActiveNodes { get; set; } = new List<Node>();
}
