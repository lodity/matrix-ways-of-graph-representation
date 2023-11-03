using System;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Models.Graph
{
    public class Node
    {
        public int Id;
        public string Name { get => "x" + Id; }
        public List<Tuple<int, Node>> Children = new();
        public List<Tuple<int, EdgeType>> Edges = new();

        public enum EdgeType
        {
            Loop,
            Directed,
            Undirected
        }

        public Node(int id, List<Tuple<int, Node>>? children = null)
        {
            Id = id;
            if (children != null)
                Children = (List<Tuple<int, Node>>)Children.Concat(children);
        }
        public void AddChild(Node node, Tuple<int, EdgeType> edge)
        {
            Children.Add(new Tuple<int, Node>(node.Id, node));
            Edges.Add(edge);
        }
        public void RemoveChild(int childIndex, Tuple<int, EdgeType> edge)
        {
            Tuple<int, Node>? childToRemove = null;
            foreach (var child in Children)
            {
                if (child.Item1 == childIndex)
                    childToRemove = child;
            }
            if (childToRemove != null)
                Children.Remove(childToRemove);

            Edges.Remove(edge);
        }
    }
}
