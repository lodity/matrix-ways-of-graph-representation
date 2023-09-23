﻿using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Models.Graph
{
    internal class Node
    {
        public int Id { get; set; }
        public string Name { get => "x" + Id; }
        public Dictionary<int, Node> Children = new();

        public Node(int id, Dictionary<int, Node> children = null)
        {
            Id = id;
            if (children != null)
                Children = Children.Concat(children).ToDictionary(x => x.Key, x => x.Value);
        }
        public void AddChild(Node node) => Children.Add(Children.Count, node);
    }
}