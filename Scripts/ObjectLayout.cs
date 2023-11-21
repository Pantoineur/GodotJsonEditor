using Godot;
using System;
using System.Collections.Generic;

public class ObjectLayout : DataClassInput
{
    Button btnAddProp;

    ConfirmationDialog cdAddProp;

    PackedScene intInputScene;
    PackedScene floatInputScene;
    PackedScene stringInputScene;

    public override void Init(string propName)
    {
        intInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/IntInput.tscn");
        floatInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/FloatInput.tscn");
        stringInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/StringInput.tscn");

        btnAddProp = GetNode<Button>("VBProps/AddProp");
        btnAddProp.Connect("pressed", this, nameof(ShowAddPropPanel));

        cdAddProp = GetNode<ConfirmationDialog>("AddPropP/AddProp");
        cdAddProp.Connect("confirmed", this, nameof(AddPropDialogConfirmed));
        cdAddProp.Connect("custom_action", this, nameof(TestCa));
    }

    public void TestCa(string action)
    {
        GD.Print($"Custom action is {action}");
    }

    public void InstantiateDataInput(DataObject dataObject)
    {
        Node inputNode = null;
        DataClassInput inputInstance = null;
        switch ((int)dataObject.DataType)
        {
            case (int)DataType.Int:
                inputNode = intInputScene.Instance();
                inputInstance = inputNode.GetNode<IntInput>("IntInput");
                break;
            case (int)DataType.Float:
                inputNode = floatInputScene.Instance();
                inputInstance = inputNode.GetNode<FloatInput>("FloatInput");
                break;
            case (int)DataType.String:
                inputNode = stringInputScene.Instance();
                inputInstance = inputNode.GetNode<StringInput>("StringInput");
                break;
        }

        inputInstance.Init(dataObject.PropName);

        if (dataObject.Value != null)
        {
            inputInstance.SetValue(dataObject.Value);
        }

        ActiveNodes.Add(inputNode);
        Properties.Add(inputInstance);
        AddChild(inputNode);
    }

    public void ShowAddPropPanel()
    {

    }

    public void AddPropDialogConfirmed()
    {

    }

    public List<DataClassInput> Properties { get; set; } = new List<DataClassInput>();
    public List<Node> ActiveNodes { get; set; } = new List<Node>();
}
