using System;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Models.Graph
{
    public class Node
    {
        public int Id;
        public string Name { get => "x" + Id; }
        public int Color;
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
        public Node Copy()
        {
            Node newNode = new(Id)
            {
                Children = new List<Tuple<int, Node>>(this.Children),
                Edges = new List<Tuple<int, EdgeType>>(this.Edges)
            };
            return newNode;
        }
        public bool HasChild(Node node)
        {
            foreach (var child in Children)
                if (child.Item2 == node) return true;
            return false;
        }
        public bool HasChild(int edge)
        {
            foreach (var child in Edges)
                if (child.Item1 == edge) return true;
            return false;
        }
        public void AddChild(Node node, Tuple<int, EdgeType> edge)
        {
            Children.Add(new Tuple<int, Node>(node.Id, node));
            Edges.Add(edge);
        }
        public void RemoveChild(int childId, int edgeId)
        {
            Tuple<int, Node>? childToRemove = null;
            foreach (var child in Children)
            {
                if (child.Item1 == childId)
                    childToRemove = child;
            }
            if (childToRemove != null)
            {
                Children.Remove(childToRemove);
                Edges.Remove(Edges.First(el => el.Item1 == edgeId));
            }
        }
        public List<int> RemoveAllLoops()
        {
            List<int> loops = new();
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Item2 == EdgeType.Loop)
                {
                    loops.Add(Edges[i].Item1);
                    Edges.RemoveAt(i);
                    Children.Remove(new Tuple<int, Node>(this.Id, this));
                    i--;
                }
            }
            return loops;
        }
    }
}
