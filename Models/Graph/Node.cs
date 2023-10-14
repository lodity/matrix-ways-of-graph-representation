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
        public List<bool> Edges = new();

        public Node(int id, List<Tuple<int, Node>>? children = null)
        {
            Id = id;
            if (children != null)
                Children = (List<Tuple<int, Node>>)Children.Concat(children);
        }
        public void AddChild(Node node, bool isEdgeSingleOriented)
        {
            Children.Add(new Tuple<int, Node>(node.Id, node));
            Edges.Add(isEdgeSingleOriented);
        }
    }
}
