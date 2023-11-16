#if TOOLS
using Godot;
using Newtonsoft.Json;
using PluginSandbox.addons.GodotJsonEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

[Tool]
public class GodotJsonEditor : EditorPlugin
{
    Control dock;
    Label filePathLabel;

    FileDialog createFileDialog;
    FileDialog loadFileDialog;
    Panel createPanel;
    OptionButton validTypes;
    VBoxContainer vBox;

    Button createBtn;
    Button loadBtn;
    Button validateBtn;
    Button saveBtn;

    PackedScene intInputScene;
    PackedScene floatInputScene;
    PackedScene stringInputScene;

    List<Node> activeNodes = new List<Node>();
    List<DataClassInput> properties = new List<DataClassInput>();

    string filePath;
    string selectedType;

    const string PathToVB = "Margin/VBoxMain/";
    const string PathToPanel = "Margin/CreatePanel/";
    const string PathToVBCreate = "Margin/CreatePanel/Margin/VBCreate/";

    public override void _EnterTree()
    {
        InitCustomTypes();
        InitUserInterface();

        intInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/IntInput.tscn");
        floatInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/FloatInput.tscn");
        stringInputScene = ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Scenes/StringInput.tscn");
    }

    private void InitCustomTypes()
    {
        Texture texture = GD.Load<Texture>("res://icon.png");

        Script intInputScript = GD.Load<Script>("res://addons/GodotJsonEditor/Scripts/IntInput.cs");
        AddCustomType("IntInput", "Control", intInputScript, texture);

        Script floatInputScript = GD.Load<Script>("res://addons/GodotJsonEditor/Scripts/FloatInput.cs");
        AddCustomType("FloatInput", "Control", floatInputScript, texture);

        Script stringInputScript = GD.Load<Script>("res://addons/GodotJsonEditor/Scripts/StringInput.cs");
        AddCustomType("StringInput", "Control", stringInputScript, texture);
    }

    private void InitUserInterface()
    {
        dock = (Control)ResourceLoader.Load<PackedScene>("addons/GodotJsonEditor/Dock/JsonEditorDock.tscn").Instance();
        AddControlToDock(DockSlot.RightUl, dock);

        filePathLabel = dock.GetNode<Label>(PathToVB + "FilePath");

        createBtn = dock.GetNode<Button>(PathToVB + "HBoxCL/Create");
        createBtn.Connect("pressed", this, "Create");

        loadBtn = dock.GetNode<Button>(PathToVB + "HBoxCL/Load");
        loadBtn.Connect("pressed", this, "Load");

        saveBtn = dock.GetNode<Button>(PathToVB + "Save");
        saveBtn.Connect("pressed", this, "Save");

        vBox = dock.GetNode<VBoxContainer>(PathToVB + "VB");

        createPanel = dock.GetNode<Panel>(PathToPanel);

        createFileDialog = dock.GetNode<FileDialog>(PathToPanel + "CreateFile");
        createFileDialog.Connect("file_selected", this, "FileSelected");

        loadFileDialog = dock.GetNode<FileDialog>(PathToPanel + "LoadFile");
        loadFileDialog.Connect("file_selected", this, "LoadData");

        validTypes = dock.GetNode<OptionButton>(PathToVBCreate + "ValidTypes");
        validTypes.Connect("item_selected", this, "TypeSelected");
        validTypes.AddItem("None");

        validateBtn = dock.GetNode<Button>(PathToVBCreate + "Validate");
        validateBtn.Connect("pressed", this, "ValidateTypePressed");

        foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (t.GetCustomAttribute<DataClassAttribute>() != null)
            {
                string itemLabel = string.Format("{0}{1}", string.IsNullOrEmpty(t.Namespace) ? "" : t.Namespace + ".", t.Name);
                validTypes.AddItem(itemLabel);
            }
        }
    }

    public void Create()
    {
        if(!createPanel.Visible)
        {
            QueueFreeSavedNodes();
            filePathLabel.Text = "";

            createPanel.Visible = true;
            createFileDialog.Popup_();
        }
    }

    public void FileSelected(string godotPath)
    {
        if(string.IsNullOrEmpty(godotPath))
        {
            GD.PrintErr("Le chemin est vide");
            return;
        }

        filePathLabel.Text = godotPath;
        filePath = ProjectSettings.GlobalizePath(godotPath);

        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Close();
        }

        validTypes.Visible = true;
    }

    public void TypeSelected(int index)
    {
        if(index == 0)
        {
            return;
        }

        selectedType = validTypes.Items[index + (index * 4)] as string;
    }

    public void ValidateTypePressed()
    {
        Type assemblyType = Assembly.GetExecutingAssembly().GetType(selectedType);

        if(assemblyType == null)
        {
            GD.PrintErr("Type not found !");
            return;
        }

        foreach (PropertyInfo prop in assemblyType.GetProperties())
        {
            switch (prop.PropertyType.Name)
            {
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                    InstantiateDataInput(new DataObject() { DataType = DataType.Int, PropName = prop.Name });
                    break;
                case nameof(UInt16):
                case nameof(UInt32):
                case nameof(UInt64):
                    // TODO
                    break;
                case nameof(Double):
                case nameof(Single):
                    InstantiateDataInput(new DataObject() { DataType = DataType.Float, PropName = prop.Name });
                    break;
                case nameof(String):
                    InstantiateDataInput(new DataObject() { DataType = DataType.String, PropName = prop.Name });
                    break;
            }

            GD.Print($"Found prop : {prop.Name} as {prop.PropertyType.Name}");
        }

        createPanel.Visible = false;
    }

    public void Load()
    {
        if (!createPanel.Visible)
        {
            QueueFreeSavedNodes();
            filePathLabel.Text = "";

            //createPanel.Visible = true;
            loadFileDialog.Popup_();
        }
    }

    public void LoadData(string godotPath)
    {
        if (string.IsNullOrEmpty(godotPath))
        {
            GD.PrintErr("Path is empty");
            return;
        }

        filePath = ProjectSettings.GlobalizePath(godotPath);

        if (!System.IO.File.Exists(filePath))
        {
            GD.PrintErr($"File '{filePath}' does not exist");
            loadFileDialog.Popup_();
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        if(string.IsNullOrEmpty(json))
        {
            GD.PrintErr($"Json is empty at path : {filePath}");
            return;
        }

        filePathLabel.Text = godotPath;

        List<DataObject> dataObjects = JsonConvert.DeserializeObject<List<DataObject>>(json);

        if(dataObjects == null)
        {
            GD.PrintErr("Error during json result cast to list !");
            return;
        }

        foreach (DataObject dataObject in dataObjects)
        {
            if (dataObject != null )
            {
                InstantiateDataInput(dataObject);
            }
        }

        createPanel.Visible = false;
    }

    private void InstantiateDataInput(DataObject dataObject)
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

    public void Save()
    {
        List<DataObject> dataObjects = new List<DataObject>();
        
        foreach(DataClassInput input in properties)
        {
            dataObjects.Add(new DataObject() { PropName = input.PropName, Value = input.Value, DataType = input.TypeToDataType() });
        }

        string json = JsonConvert.SerializeObject(dataObjects, Formatting.Indented);

        System.IO.File.WriteAllText(filePath, json);
    }

    private void QueueFreeSavedNodes()
    {
        foreach (Node inputNode in properties.ToArray())
        {
            inputNode.QueueFree();
        }
        foreach (Node node in activeNodes.ToArray())
        {
            node.QueueFree();
        }

        properties.Clear();
        activeNodes.Clear();
    }

    public override void _ExitTree()
    {
        RemoveCustomType("IntInput");
        RemoveCustomType("FloatInput");
        RemoveCustomType("StringInput");
        RemoveControlFromDocks(dock);

        QueueFreeSavedNodes();

        dock.Free();
    }
}

public enum DataType
{
    Int,
    Float,
    String,
    Uint
}

#endif