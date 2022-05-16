using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    public Canopy.Node node;
    public Port input;
    public Port output;

    Orientation orientation = Orientation.Vertical;

    public NodeView(Canopy.Node node)
    : base(AssetDatabase.GUIDToAssetPath("a71cd26506e4fac4bb7867f00f9090e1"))
    {
        this.node = node;
        this.title = node.title == "" ? (node.title = node.name) : node.title;
        this.viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
        SortChildren();

        var titleLabel = this.Q<Label>("title-label");
        titleLabel.bindingPath = "title";
        titleLabel.Bind(new SerializedObject(node));
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }
    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
    public void SortChildren()
    {
        var composite = node as Canopy.CompositeNode;
        composite?.children?.Sort((a, b) =>
        {
            var x1 = a.position.x;
            var x2 = b.position.x;
            if (x1 < x2)
                return -1;
            else if (x1 > x2)
                return 1;
            else
                return 0;
        });
        if (composite == null)
            return;
        for (int i = 0; i < composite.children.Count; i++)
            composite.children[i].childIndex = i + 1;
    }
    void CreateInputPorts()
    {
        if (node is Canopy.RootNode)
            return;

        input = InstantiatePort(orientation, Direction.Input, Port.Capacity.Single, typeof(bool));
        Debug.Assert(input != null);

        input.portName = "";
        input.style.flexBasis = new StyleLength(25);
        inputContainer.Add(input);

    }
    void CreateOutputPorts()
    {
        switch (node)
        {
            case Canopy.CompositeNode t1:
                output = InstantiatePort(orientation, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case Canopy.DecoratorNode t2:
                output = InstantiatePort(orientation, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            default:
                return;
        }

        Debug.Assert(output != null);


        output.portName = "";
        output.style.flexBasis = new StyleLength(25);
        outputContainer.Add(output);
    }

    void SetupClasses()
    {
        switch (node)
        {
            case Canopy.CompositeNode t1:
                AddToClassList("composite");
                break;
            case Canopy.DecoratorNode t2:
                AddToClassList("decorator");
                break;
            case Canopy.ActionNode t3:
                AddToClassList("action");
                break;
        }
    }

    public void UpdateState()
    {
        var composite = (node.parent as Canopy.CompositeNode);
        if (composite?.children.Count > 1)
        {
            AddToClassList("isChild");
            this.Query<Label>("order-label").First().text = $"#{node.childIndex}";
        }
        else
            RemoveFromClassList("isChild");

        if (!Application.isPlaying)
            return;
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        switch (node.state)
        {
            case Canopy.Node.State.Running:
                if (node.started)
                    AddToClassList("running");
                break;
            case Canopy.Node.State.Failure:
                AddToClassList("failure");
                break;
            case Canopy.Node.State.Success:
                AddToClassList("success");
                break;
        }
    }
}