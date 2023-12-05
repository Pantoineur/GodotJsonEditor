using Godot;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PluginSandbox.addons.GodotJsonEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

[Tool]
public class ObjectLayout : DataClassInput
{
    Button btnAddProp;
    Button btnExpand;

    ConfirmationDialog cdAddProp;
    LineEdit txtNewPropName;
    OptionButton optNewPropType;

    PackedScene intInputScene;
    PackedScene unsignedIntInputScene;
    PackedScene floatInputScene;
    PackedScene stringInputScene;
    PackedScene objectLayoutScene;

    MarginContainer mProps;

    VBoxContainer vbChildren;

    Control mainContainer;

    Texture iconArrowUp;
    Texture iconArrowDown;

    bool isExpanded = false;

    HashSet<string> propNames = new HashSet<string>();

    const string iconsPath = "res://addons/GodotJsonEditor/Icons";
    
    public override void Init(string propName)
    {
        this.propName = propName;
        GetNode<Label>("../VB/MExp/HB/Name").Text = propName;

        intInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/IntInput.tscn");
        unsignedIntInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/UnsignedIntInput.tscn");
        floatInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/FloatInput.tscn");
        stringInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/StringInput.tscn");
        objectLayoutScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/ObjectLayout.tscn");

        btnAddProp = GetNode<Button>("../VB/MProps/VBProps/AddProp");
        btnAddProp.Connect("pressed", this, nameof(ShowAddPropPanel));

        btnExpand = GetNode<Button>("../VB/MExp/HB/ExpandBtn");
        btnExpand.Connect("pressed", this, nameof(ToggleExpand));

        mainContainer = GetParent<Control>();
        vbChildren = GetNode<VBoxContainer>("../VB/MProps/VBProps/Children");

        cdAddProp = GetNode<ConfirmationDialog>("../AddPropP/AddProp");
        cdAddProp.Connect("confirmed", this, nameof(AddPropDialogConfirmed));
        cdAddProp.Connect("custom_action", this, nameof(TestCa));

        optNewPropType = cdAddProp.GetNode<OptionButton>("M/VB/OptType");
        txtNewPropName = cdAddProp.GetNode<LineEdit>("M/VB/EditName");

        mProps = GetNode<MarginContainer>("../VB/MProps");

        iconArrowDown = GD.Load<Texture>($"{iconsPath}/down_arrow.png");
        iconArrowUp = GD.Load<Texture>($"{iconsPath}/up_arrow.png");

        IsInit = true;
    }

    public void TestCa(string action)
    {
        GD.Print($"Custom action is {action}");
    }

    public void ToggleExpand()
    {
        isExpanded = !isExpanded;

        mProps.Visible = isExpanded;
        btnExpand.Icon = isExpanded ? iconArrowDown : iconArrowUp;
        btnExpand.FocusMode = FocusModeEnum.None;
    }

    public DataClassInput InstantiateDataInput(DataObject dataObject)
    {
        Node inputNode = null;
        DataClassInput inputInstance = null;
        switch (dataObject.DataType)
        {
            case DataType.Int:
                inputNode = intInputScene.Instance();
                inputInstance = inputNode.GetNode<IntInput>(nameof(IntInput));
                break;
            case DataType.Uint:
                inputNode = unsignedIntInputScene.Instance();
                inputInstance = inputNode.GetNode<UnsignedIntInput>(nameof(UnsignedIntInput));
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

                if(inputInstance is ObjectLayout asObjectLayout)
                {
                    asObjectLayout.Level = Level + 1;
                    asObjectLayout.TypeHierarchy = typeHierarchy;

                    if (dataObject.BaseType != null && !string.IsNullOrEmpty(dataObject.BaseType.Name))
                    {
                        asObjectLayout.InstantiateFromType(dataObject.BaseType);
                    }
                }

                break;
        }

        if(!inputInstance.IsInit)
        {
            inputInstance.Init(dataObject.PropName);
        }        

        if (dataObject.Value != null && dataObject.DataType != DataType.Object)
        {
            inputInstance.SetValue(dataObject.Value, dataObject.BaseType);
        }

        ActiveNodes.Add(inputNode);
        Properties.Add(inputInstance);
        vbChildren.AddChild(inputNode);

        return inputInstance;
    }

    public void InstantiateFromType(Type type)
    {
        typeHierarchy.Add(type.Name);

        foreach (PropertyInfo prop in type.GetProperties())
        {
            if(prop.PropertyType == type)
            {
                GD.PrintErr($"Type {type.Name} has a property of its own type. It is ignored for infinite loop reasons.");
            }
            else
            {
                if(prop.PropertyType.ToDataType() == DataType.Object)
                {
                    if (typeHierarchy.Add(prop.PropertyType.Name))
                    {
                        foreach(string name in typeHierarchy)
                        {
                            GD.Print(name);
                        }
                        InstantiateDataInput(new DataObject() { DataType = prop.PropertyType.ToDataType(), PropName = prop.Name, BaseType = prop.PropertyType });
                    }
                    else
                    {
                        GD.PrintErr($"Type '{prop.PropertyType.Name}' was already parsed in this hierarchy. Ignoring it to avoid infinite loop.");
                    }
                }
                else
                {
                    InstantiateDataInput(new DataObject() { DataType = prop.PropertyType.ToDataType(), PropName = prop.Name, BaseType = prop.PropertyType });
                }
            }
        }

        typeHierarchy.Clear();
    }

    public void InstantiateFromJson(JArray propertiesJson)
    {
        foreach (JObject dataObject in propertiesJson)
        {
            DataObject test = dataObject.ToObject<DataObject>();
            if (dataObject != null)
            {
                DataClassInput instance = InstantiateDataInput(test);
                if(test.Value is JArray instanceArray && instance is ObjectLayout asObjectLayout)
                {
                    asObjectLayout.InstantiateFromJson(instanceArray);
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
        cdAddProp.Popup_();
    }

    public void AddPropDialogConfirmed()
    {
        string newPropName = txtNewPropName.Text;
        string newPropTypeName = optNewPropType.Text;

        if (string.IsNullOrEmpty(newPropName))
        {
            GD.Print("Unable to add property without a name");
            return;
        }
        if (string.IsNullOrEmpty(newPropTypeName))
        {
            GD.Print("Unable to add property without a type");
            return;
        }

        if(!propNames.Add(newPropName))
        {
            GD.Print($"Property '{newPropName}' already exists");
            return;
        }

        if (!Enum.TryParse(newPropTypeName, out DataType dataType))
        {
            GD.Print($"Type '{newPropTypeName}' was not found");
            return;
        }

        DataObject newProp = new DataObject()
        {
            DataType = dataType,
            PropName = newPropName
        };

        InstantiateDataInput(newProp);

        txtNewPropName.Clear();
        optNewPropType.Text = "Type";
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

    public DataObject PropertiesToDataObject()
    {
        DataObject dataObject = new DataObject
        {
            PropName = PropName,
            DataType = DataType.Object
        };

        List<DataObject> children = new List<DataObject>();
        foreach (DataClassInput input in Properties)
        {
            switch (input)
            {
                case ObjectLayout asObject:
                    children.Add(asObject.PropertiesToDataObject());
                    break;
                default:
                    children.Add(new DataObject()
                    {
                        PropName = input.PropName,
                        Value = input.Value
                    });
                    break;
            }
        }

        dataObject.Value = children;
        return dataObject;
    }

    public List<DataClassInput> Properties { get; set; } = new List<DataClassInput>();
    public List<Node> ActiveNodes { get; set; } = new List<Node>();

    protected HashSet<string> typeHierarchy = new HashSet<string>();
    public HashSet<string> TypeHierarchy
    {
        get => typeHierarchy;
        set => typeHierarchy = new HashSet<string>(value);
    }

    protected int level;
    public int Level {
        get => level;
        set
        {
            level = value;
            mainContainer.MarginLeft = Level * 30;
            mainContainer.MarginRight = Level * 30;
        }
    }
}
