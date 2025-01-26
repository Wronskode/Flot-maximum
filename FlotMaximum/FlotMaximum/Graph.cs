namespace FlotMaximum;

public class Graph : ICloneable
{
    public List<Vertex> Vertices { get; set; } = new();
    protected Dictionary<Vertex, HashSet<Vertex>> AdjVertices { get; } = new();
    public Dictionary<(Vertex, Vertex), int> Edges { get; set; } = new ();
    
    public Graph(IEnumerable<(Vertex, Vertex, int)> neighbors, IEnumerable<Vertex> vertices) {
        HashSet<Vertex> vertexSet = new();
        foreach ((Vertex, Vertex, int) element in neighbors) {
            vertexSet.Add(element.Item1);
            vertexSet.Add(element.Item2);
            AddEdge((element.Item1, element.Item2), element.Item3);
        }

        foreach (Vertex v in vertices)
        {
            vertexSet.Add(v);
        }
        foreach (Vertex v in vertexSet)
        {
            AddVertex(v);
        }
    }
    
    public void AddEdge((Vertex, Vertex) edge, int weight)
    {
        if (edge.Item1 == edge.Item2 || !Edges.TryAdd(edge, weight))
            return;
        
        if (!AdjVertices.ContainsKey(edge.Item1))
        {
            Vertices.Add(edge.Item1);
            AdjVertices.Add(edge.Item1, []);
        }
        AdjVertices[edge.Item1].Add(edge.Item2);
    }
    
    public void AddVertex(Vertex v) {
        if (AdjVertices.TryAdd(v, new()))
            Vertices.Add(v);
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
    public virtual object Clone()
    {
        return new Graph(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            Vertices.Select(x => x.Clone() as Vertex));
    }
}