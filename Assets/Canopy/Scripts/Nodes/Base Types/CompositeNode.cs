using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canopy
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> children = new List<Node>();

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public void RemoveChild(Node child)
        {
            children.Remove(child);
        }

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.children.Clear();
            foreach (var old in children)
            {
                var child = old.Clone();
                node.children.Add(child);
            }
            return node;
        }
    }
}