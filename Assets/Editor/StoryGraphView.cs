using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class StoryGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
    public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
    public Blackboard Blackboard = new Blackboard();
    // public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
    // private NodeSearchWindow _searchWindow;

    Dictionary<FlowNode, FlowEditorNode> _nodeMap = new Dictionary<FlowNode, FlowEditorNode>();
    List<FlowEditorNode> AllNodes = new List<FlowEditorNode>();

    void OnNodeEnter(FlowNode node)
    {
        foreach(var x in AllNodes)
        {
            x.selected = false;
        }

        var editorNode = _nodeMap[node];
        editorNode.selected = true;
    }

    void OnNodeExit(FlowNode node)
    {
        
    }

    public StoryGraphView(Flow flow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        foreach(var x in flow.AllNodes)
        {
            var editorNode = EditorNodeFactory.Create(x);
            _nodeMap[x] = editorNode;
            AllNodes.Add(editorNode);

            x.OnExitEvent -= OnNodeExit;
            x.OnExitEvent += OnNodeExit;
            x.OnEnterEvent -= OnNodeEnter;
            x.OnEnterEvent += OnNodeEnter;
        }

        foreach(var x in AllNodes)
        {
            AddElement(x);
        }

        foreach(var x in flow.AllNodes)
        {
            var editorNode = _nodeMap[x];

            if(x.NextFlow != null)
            {
                var editorNodeNext = _nodeMap[x.nextFlow];
                var edge = (editorNode.outputContainer.Children().First() as Port).ConnectTo(editorNodeNext.inputContainer.Children().First() as Port);
                AddElement(edge);
            }
            
            if(x is ConditionFlowNode conditionFlowNode)
            {
                var flowNo = _nodeMap[conditionFlowNode.nextFlowNo];
                var edgeNo = (editorNode.outputContainer.Children().Last() as Port).ConnectTo(flowNo.inputContainer.Children().First() as Port);
                AddElement(edgeNo);
            }
        }

        if(flow.CurrentNode != null)
        {
            OnNodeEnter(flow.CurrentNode);
        }

        // var startNode = GetEntryPointNodeInstance();
        // var endNode = GenExitPointNodeInstance();

        // var normalNode = GetNormalNodeInstance("x", new Rect(333, 555, 100, 150));
        // AddElement(normalNode);

        // var conditoinNode = GetConditionNodeInstance("condition", new Rect(222, 444, 100, 150));
        // AddElement(conditoinNode);
        // AddElement(startNode);
        // AddElement(endNode);

        // var edge = (startNode.outputContainer.Children().First() as Port).ConnectTo(normalNode.inputContainer.Children().First() as Port);
        // AddElement(edge);

        // var edge1 = (normalNode.outputContainer.Children().First() as Port).ConnectTo(endNode.inputContainer.Children().First() as Port);
        // AddElement(edge1);

        // CreateNewDialogueNode("aaa", new Vector2(0, 9));
        // AddSearchWindow(editorWindow);
    }


    // private void AddSearchWindow(StoryGraph editorWindow)
    // {
    //     _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
    //     _searchWindow.Configure(editorWindow, this);
    //     nodeCreationRequest = context =>
    //         SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    // }


    // public void ClearBlackBoardAndExposedProperties()
    // {
    //     ExposedProperties.Clear();
    //     Blackboard.Clear();
    // }

    // public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
    // {
    //     if(commentBlockData==null)
    //         commentBlockData = new CommentBlockData();
    //     var group = new Group
    //     {
    //         autoUpdateGeometry = true,
    //         title = commentBlockData.Title
    //     };
    //     AddElement(group);
    //     group.SetPosition(rect);
    //     return group;
    // }

    // public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
    // {
    //     var localPropertyName = property.PropertyName;
    //     var localPropertyValue = property.PropertyValue;
    //     if (!loadMode)
    //     {
    //         while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
    //             localPropertyName = $"{localPropertyName}(1)";
    //     }

    //     var item = ExposedProperty.CreateInstance();
    //     item.PropertyName = localPropertyName;
    //     item.PropertyValue = localPropertyValue;
    //     ExposedProperties.Add(item);

    //     var container = new VisualElement();
    //     var field = new BlackboardField {text = localPropertyName, typeText = "string"};
    //     container.Add(field);

    //     var propertyValueTextField = new TextField("Value:")
    //     {
    //         value = localPropertyValue
    //     };
    //     propertyValueTextField.RegisterValueChangedCallback(evt =>
    //     {
    //         var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
    //         ExposedProperties[index].PropertyValue = evt.newValue;
    //     });
    //     var sa = new BlackboardRow(field, propertyValueTextField);
    //     container.Add(sa);
    //     Blackboard.Add(container);
    // }

    // public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    // {
    //     var compatiblePorts = new List<Port>();
    //     var startPortView = startPort;

    //     ports.ForEach((port) =>
    //     {
    //         var portView = port;
    //         if (startPortView != portView && startPortView.node != portView.node)
    //             compatiblePorts.Add(port);
    //     });

    //     return compatiblePorts;
    // }

    // public void CreateNewDialogueNode(string nodeName, Vector2 position)
    // {
    //     AddElement(CreateNode(nodeName, position));
    // }

    // public DialogueNode CreateNode(string nodeName, Vector2 position)
    // {
    //     var tempDialogueNode = new DialogueNode()
    //     {
    //         title = nodeName,
    //         DialogueText = nodeName,
    //         GUID = Guid.NewGuid().ToString()
    //     };
    //     tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
    //     var inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
    //     inputPort.portName = "Input";
    //     tempDialogueNode.inputContainer.Add(inputPort);

    //     tempDialogueNode.RefreshExpandedState();
    //     tempDialogueNode.RefreshPorts();
    //     tempDialogueNode.SetPosition(new Rect(position,
    //         DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

    //     return tempDialogueNode;
    // }


    // public void AddChoicePort(DialogueNode nodeCache, string overriddenPortName = "")
    // {
    //     var generatedPort = GetPortInstance(nodeCache, Direction.Output);
    //     var portLabel = generatedPort.contentContainer.Q<Label>("type");
    //     generatedPort.contentContainer.Remove(portLabel);

    //     var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
    //     var outputPortName = string.IsNullOrEmpty(overriddenPortName)
    //         ? $"Option {outputPortCount + 1}"
    //         : overriddenPortName;


    //     var textField = new TextField()
    //     {
    //         name = string.Empty,
    //         value = outputPortName
    //     };
    //     textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
    //     generatedPort.contentContainer.Add(new Label("  "));
    //     generatedPort.contentContainer.Add(textField);
    //     var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
    //     {
    //         text = "X"
    //     };
    //     generatedPort.contentContainer.Add(deleteButton);
    //     generatedPort.portName = outputPortName;
    //     nodeCache.outputContainer.Add(generatedPort);
    //     nodeCache.RefreshPorts();
    //     nodeCache.RefreshExpandedState();
    // }

    // private void RemovePort(Node node, Port socket)
    // {
    //     var targetEdge = edges.ToList()
    //         .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
    //     if (targetEdge.Any())
    //     {
    //         var edge = targetEdge.First();
    //         edge.input.Disconnect(edge);
    //         RemoveElement(targetEdge.First());
    //     }

    //     node.outputContainer.Remove(socket);
    //     node.RefreshPorts();
    //     node.RefreshExpandedState();
    // }

    // private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
    //     Port.Capacity capacity = Port.Capacity.Single)
    // {
    //     return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    // }

    // private DialogueNode GenExitPointNodeInstance()
    // {
    //     var nodeCache = new DialogueNode()
    //     {
    //         title = "END",
    //         GUID = Guid.NewGuid().ToString(),
    //         DialogueText = "ENTRYPOINT",
    //         EntyPoint = true
    //     };

    //     var generatedPort = GetPortInstance(nodeCache, Direction.Input);
    //     generatedPort.portName = "";
    //     nodeCache.inputContainer.Add(generatedPort);

    //     nodeCache.capabilities &= ~Capabilities.Movable;
    //     nodeCache.capabilities &= ~Capabilities.Deletable;

    //     nodeCache.RefreshExpandedState();
    //     nodeCache.RefreshPorts();
    //     nodeCache.SetPosition(new Rect(400, 200, 100, 150));
    //     return nodeCache;
    // }

    // private DialogueNode GetEntryPointNodeInstance()
    // {
    //     var nodeCache = new DialogueNode()
    //     {
    //         title = "START",
    //         GUID = Guid.NewGuid().ToString(),
    //         DialogueText = "ENTRYPOINT",
    //         EntyPoint = true
    //     };

    //     var generatedPort = GetPortInstance(nodeCache, Direction.Output);
    //     generatedPort.portName = "";
    //     nodeCache.outputContainer.Add(generatedPort);

    //     nodeCache.capabilities &= ~Capabilities.Movable;
    //     nodeCache.capabilities &= ~Capabilities.Deletable;

    //     nodeCache.RefreshExpandedState();
    //     nodeCache.RefreshPorts();
    //     nodeCache.SetPosition(new Rect(100, 200, 100, 150));
    //     nodeCache.selected = true;

    //     return nodeCache;
    // }


    // Node GetConditionNodeInstance(string name, Rect rect)
    // {
    //     var nodeCache = new Node()
    //     {
    //         title = name,
    //     };

    //     var portInput = nodeCache.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
    //     portInput.portName = "";
    //     nodeCache.inputContainer.Add(portInput);

    //     var portOutputYes = nodeCache.InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(float));
    //     portOutputYes.portName = "Yes";
    //     nodeCache.outputContainer.Add(portOutputYes);

    //     var portOutputNo = nodeCache.InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(float));
    //     portOutputNo.portName = "No";
    //     nodeCache.outputContainer.Add(portOutputNo);

    //     nodeCache.capabilities &= ~Capabilities.Movable;
    //     nodeCache.capabilities &= ~Capabilities.Deletable;

    //     nodeCache.RefreshExpandedState();
    //     nodeCache.RefreshPorts();
    //     nodeCache.SetPosition(rect);

    //     nodeCache.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
    //     return nodeCache;
    // }
    // Node GetNormalNodeInstance(string name, Rect rect)
    // {
    //    var tempDialogueNode = new DialogueNode()
    //     {
    //         title = name,
    //     };
    //     tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
    //     tempDialogueNode.RefreshExpandedState();
    //     tempDialogueNode.RefreshPorts();
    //     tempDialogueNode.SetPosition(new Rect(rect.position,
    //         DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

    //     return tempDialogueNode;
    // }
}