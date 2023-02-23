using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryGraph : EditorWindow
{
    private string _fileName = "New Narrative";

    private StoryGraphView _graphView;

    public void OpenWithAsset(TextAsset x)
    {
        var flow = new Flow(x.name, x.text, "");
        OpenWithFlow(flow);
    }

    public void OpenWithFlow(Flow flow)
    {
        _graphView = new StoryGraphView(flow)
        {
            name = flow.Title,
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);

        OnOpen();
    }


    // private void ConstructGraphView()
    // {
    //     if(asset == null) return;;

    //     var flow = new Flow(asset.name, asset.text, "");

    //     _graphView = new StoryGraphView(flow)
    //     {
    //         name = "Narrative Graph",
    //     };
    //     _graphView.StretchToParentSize();
    //     rootVisualElement.Add(_graphView);

    // }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});

        toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});
        // toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) {text = "New Node",});
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        // if (!string.IsNullOrEmpty(_fileName))
        // {
        //     var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        //     if (save)
        //         saveUtility.SaveGraph(_fileName);
        //     else
        //         saveUtility.LoadNarrative(_fileName);
        // }
        // else
        // {
        //     EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
        // }
    }

    private void OnOpen()
    {
        // GenerateToolbar();
        GenerateMiniMap();
        // GenerateBlackBoard();
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap {anchored = true};
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    // private void GenerateBlackBoard()
    // {
    //     var blackboard = new Blackboard(_graphView);
    //     blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
    //     blackboard.addItemRequested = _blackboard =>
    //     {
    //         _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
    //     };
    //     blackboard.editTextRequested = (_blackboard, element, newValue) =>
    //     {
    //         var oldPropertyName = ((BlackboardField) element).text;
    //         if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
    //         {
    //             EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
    //                 "OK");
    //             return;
    //         }

    //         var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
    //         _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
    //         ((BlackboardField) element).text = newValue;
    //     };
    //     blackboard.SetPosition(new Rect(10,30,200,300));
    //     _graphView.Add(blackboard);
    //     _graphView.Blackboard = blackboard;
    // }

    private void OnDisable()
    {
        if(_graphView != null)
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}