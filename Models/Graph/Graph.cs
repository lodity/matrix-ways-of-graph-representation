using System.Collections;
using System.Collections.Generic;

namespace CDM_Lab_3._1.Models.Graph
{
    public class Graph : IEnumerable
    {
        public List<Node> Nodes = new();
        public int NodeCount { get => Nodes.Count; }
        public int EdgeCount
        {
            get
            {
                int count = 0;
                foreach (var node in Nodes)
                    foreach (var child in node.Children)
                        count++;
                return count;
            }
        }
        public Graph(int nodeCount)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                Node node = new(i);
                AddNode(node);
            }
        }
        public void AddNode(Node node)
        {
            Nodes.Add(node);
        }
        public void AddNode()
        {
            Node node = new(Nodes.Count);
            Nodes.Add(node);
        }
        public void RemoveNode(int nodeIndex)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes[i].Children.Count; j++)
                {
                    if (Nodes[i].Children[j].Item2.Id == nodeIndex)
                        Nodes[i].Children.RemoveAt(j);
                }
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Id == nodeIndex)
                {
                    Nodes.RemoveAt(i);
                    break;
                }
            }
        }
        public IEnumerator GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }
    }
}
