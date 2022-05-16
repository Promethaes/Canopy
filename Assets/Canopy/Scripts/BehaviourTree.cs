using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Canopy
{
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "Canopy/BehaviourTree", order = 0)]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State state = Node.State.Running;
        public List<Node> nodes = new List<Node>();

        Blackboard blackboard = null;
        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running)
                state = rootNode.Update();
            return state;
        }

        public Node CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            //adds new node to this scriptable object so it saves
            //the node properly
            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }
        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }


        //Stinky!!!
        //maybe move this to respective node subclasses?
        public void AddChild(Node parent, Node child)
        {
            switch (parent)
            {
                case CompositeNode t2:
                    Object[] t2objs = {t2,child};
                    Undo.RecordObjects(t2objs, "Behaviour Tree (AddChild)");
                    t2.AddChild(child);
                    child.parent = t2;
                    break;
                case DecoratorNode t3:
                    Object[] t3objs = {t3,child};
                    Undo.RecordObjects(t3objs, "Behaviour Tree (AddChild)");
                    t3.child = child;
                    child.parent = t3;
                    break;
                default:
                    Debug.LogError("Failed to add child");
                    return;
            }
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(Node parent, Node child)
        {
            switch (parent)
            {
                case CompositeNode t2:
                    Object[] t2objs = {t2,child};
                    Undo.RecordObjects(t2objs, "Behaviour Tree (RemoveChild)");
                    child.parent = null;
                    t2.RemoveChild(child);
                    break;
                case DecoratorNode t3:
                    Object[] t3objs = {t3,child};
                    Undo.RecordObjects(t3objs, "Behaviour Tree (RemoveChild)");
                    child.parent = null;
                    t3.child = null;
                    break;
                default:
                    Debug.LogError("Failed to remove child");
                    return;
            }
            EditorUtility.SetDirty(parent);
        }

        public List<Node> GetChildren(Node parent)
        {
            switch (parent)
            {
                case CompositeNode t2:
                    return t2.children;
                case DecoratorNode t3:
                    var l = new List<Node>();
                    l.Add(t3.child);
                    return l;
                default:
                    var emptyList = new List<Node>();
                    return emptyList;
            }
        }

        public void DepthFirstSearch(Node node, System.Action<Node> visiter)
        {
            if (!node)
                return;
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((x) => DepthFirstSearch(x, visiter));
        }

        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            tree.rootNode = rootNode.Clone();
            tree.nodes = new List<Node>();
            DepthFirstSearch(tree.rootNode, (x) => tree.nodes.Add(x));

            return tree;
        }

        public void SetBlackboard(Blackboard blackboard)
        {
            this.blackboard = blackboard;
            nodes.ForEach(x => x.blackboard = this.blackboard);
        }
    }
}