﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Models.Graph
{
    public class Node
    {
        public int Id;
        public string Name { get => "x" + Id; }
        public int Color;
        // <weight of child edge, child node>
        public List<Tuple<int, Node>> Children = new();
        // <edge id, edge type, edge weight, nodeTo>
        public List<Tuple<int, EdgeType, int, Node>> Edges = new();

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
                Edges = new List<Tuple<int, EdgeType, int, Node>>(this.Edges)
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
        public void AddChild(Node node, Tuple<int, EdgeType, int, Node> edge)
        {
            Children.Add(new Tuple<int, Node>(1, node));
            Edges.Add(edge);
        }
        public void RemoveChild(int childId, int edgeId)
        {
            Tuple<int, Node>? childToRemove = null;
            foreach (var child in Children)
            {
                if (child.Item2.Id == childId)
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
                    Children.Remove(Children.First(el => el.Item2 == this));
                    i--;
                }
            }
            return loops;
        }
        // <edge id, edge weight>
        public Tuple<int, int> LessWeightedEdge(Node node)
        {
            // <edge id, edge weight>
            Tuple<int, int> Edge = Edges.Aggregate(new Tuple<int, int>(-1, int.MaxValue), (acc, el) =>
            {
                if (el.Item4 == node && el.Item3 < acc.Item2)
                    return new Tuple<int, int>(el.Item1, el.Item3);
                else
                    return acc;
            });
            return Edge;
        }
        public void SetWeight(int edgeId, int weight)
        {
            int foundEdgeId = Edges.FindIndex(el => el.Item1 == edgeId);
            Edges[foundEdgeId] = new Tuple<int, EdgeType, int, Node>(edgeId, Edges[foundEdgeId].Item2, weight, Edges[foundEdgeId].Item4);
        }
    }
}