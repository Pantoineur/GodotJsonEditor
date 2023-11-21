using Godot;
using System;

public class ObjectLayout : VBoxContainer
{
    VBoxContainer vbProps;

    Button btnAddProp;

    ConfirmationDialog cdAddProp;

    PackedScene intInputScene;
    PackedScene floatInputScene;
    PackedScene stringInputScene;

    public override void _EnterTree()
    {
        intInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/IntInput.tscn");
        floatInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/FloatInput.tscn");
        stringInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/StringInput.tscn");

        vbProps = GetNode<VBoxContainer>("VBProps");

        btnAddProp = GetNode<Button>("VBProps/AddProp");
        btnAddProp.Connect("pressed", this, "ShowAddPropPanel");

        cdAddProp = GetNode<ConfirmationDialog>("AddPropP/AddProp");
        cdAddProp.Connect("confirmed", this, "AddPropDialogConfirmed");
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

        activeNodes.Add(inputNode);
        properties.Add(inputInstance);
        vBox.AddChild(inputNode);
    }

    public void ShowAddPropPanel()
    {

    }

    public void AddPropDialogConfirmed()
    {

    }
}
