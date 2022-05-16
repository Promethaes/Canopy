using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Canopy;
using System;
using System.Linq;
using UnityEngine;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    BehaviourTree tree;


    Vector2 mousePos = new Vector2();
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        var styleSheet = LoadAssetByGUID.Load<StyleSheet>("806b0aa08637abe4b93e46dc71208a9f");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {

        //https://answers.unity.com/questions/1825041/how-to-get-the-correct-contextual-menu-mouse-posit.html
        VisualElement contentViewContainer = ElementAt(1);
        Vector3 screenMousePosition = evt.localMousePosition;
        Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
        worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
        mousePos = worldMousePosition;

        void AddToContextMenu(TypeCache.TypeCollection t)
        {
            foreach (var type in t)
                if (type.Name.CompareTo("RootNode") != 0)
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (x) => CreateNode(type));
        }

        //get types deriving from action node 
        var types = TypeCache.GetTypesDerivedFrom<Canopy.CompositeNode>();
        AddToContextMenu(types);
        types = TypeCache.GetTypesDerivedFrom<Canopy.ActionNode>();
        AddToContextMenu(types);
        types = TypeCache.GetTypesDerivedFrom<Canopy.DecoratorNode>();
        AddToContextMenu(types);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node)
            .ToList();
    }

    private void CreateNode(Type type)
    {
        var node = tree.CreateNode(type);
        CreateNodeView(node, true);
    }

    NodeView FindNodeView(Canopy.Node node)
    {
        var output = GetNodeByGuid(node.guid);
        if (output == null)
            Debug.LogError("output was null");

        return output as NodeView;
    }

    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        tree.nodes.ForEach(x => { CreateNodeView(x); });
        AssetDatabase.SaveAssets();

        tree.nodes.ForEach(x =>
        {
            if (x == null)
                Debug.LogError("fuck");

            var children = tree.GetChildren(x);

            children?.ForEach(y =>
            {
                if (y == null)
                    return;

                var parentView = FindNodeView(x);
                var childView = FindNodeView(y);
                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(x =>
        {
            switch (x)
            {
                case NodeView t1:
                    tree.DeleteNode((x as NodeView).node);
                    break;
                case Edge t2:
                    //STINKY! probably
                    var edge = x as Edge;
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                    break;
            }
        });
        graphViewChange.edgesToCreate?.ForEach(x =>
        {
            var parentView = x.output.node as NodeView;
            var childView = x.input.node as NodeView;
            Debug.Assert(parentView != null && childView != null);
            //stinky?????
            tree.AddChild(parentView.node, childView.node);
        });

        if (graphViewChange.movedElements != null)
            nodes.ForEach((x) =>
            {
                var nodeView = x as NodeView;
                nodeView?.SortChildren();
            });
        return graphViewChange;
    }

    void CreateNodeView(Canopy.Node node, bool userCreated = false)
    {
        NodeView nodeView = new NodeView(node);

        if (userCreated)
        {

            var nPos = nodeView.GetPosition();
            nPos.min = mousePos;
            nodeView.SetPosition(nPos);
        }

        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(x =>
        {
            (x as NodeView).UpdateState();
        });
    }
}
