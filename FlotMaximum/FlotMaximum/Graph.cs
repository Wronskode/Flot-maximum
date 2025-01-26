namespace FlotMaximum;

public class Graph : ICloneable
{
    private List<Vertex> Vertices { get; } = new();
    private Dictionary<Vertex, HashSet<Vertex>> AdjVertices { get; } = new();
    public Dictionary<(Vertex, Vertex), int> Edges { get; set; } = new ();
    
    public Graph(IEnumerable<(Vertex, Vertex, int)> neighbors) {
        HashSet<Vertex> vertexSet = new();
        foreach ((Vertex, Vertex, int) element in neighbors) {
            vertexSet.Add(element.Item1);
            vertexSet.Add(element.Item2);
            AddEdge((element.Item1, element.Item2), element.Item3);
        }
        Vertices = new (vertexSet);
    }
    
    public Graph(IEnumerable<KeyValuePair<(Vertex, Vertex), int>> neighbors) {
        HashSet<Vertex> vertexSet = new();
        foreach (KeyValuePair<(Vertex, Vertex), int> element in neighbors) {
            vertexSet.Add(element.Key.Item1);
            vertexSet.Add(element.Key.Item2);
            AddEdge((element.Key.Item1, element.Key.Item2), element.Value);
        }
        Vertices = new (vertexSet);
    }
    
    public void AddEdge((Vertex, Vertex) edge, int weight) {
        if (!Edges.TryAdd(edge, weight))
        {
            return;
        }

        if (!AdjVertices.ContainsKey(edge.Item1))
        {
            Vertices.Add(edge.Item1);
            AdjVertices.Add(edge.Item1, []);
        }
        AdjVertices[edge.Item1].Add(edge.Item2);
    }

    public override string ToString()
    {
        string output = "";
        foreach (Vertex vertex in Vertices)
        {
            string txt = "";
            foreach (var edge in Edges)
            {
                if (edge.Key.Item2 == vertex)
                {
                    txt += edge.Key.Item1 + ",";
                }
            }
            txt = txt.TrimEnd(',');
            if (txt.Length == 0) continue;
            txt += " -> " + vertex;
            output += txt + "\n";
        }
        return output;
    }

    public object Clone()
    {
        return new Graph(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)));
    }
}