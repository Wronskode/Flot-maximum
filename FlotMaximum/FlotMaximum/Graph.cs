namespace FlotMaximum;

public class Graph : ICloneable
{
    public List<Vertex> Vertices { get; } = new();
    public Dictionary<Vertex, HashSet<Vertex>> AdjVertices { get; } = new();
    public int NbVertices = 0;
    public Dictionary<(Vertex, Vertex), int> Edges { get; set; } = new ();
    
    public Graph(IEnumerable<(Vertex, Vertex, int)> neighbors) {
        HashSet<Vertex> vertexSet = new();
        foreach ((Vertex, Vertex, int) element in neighbors) {
            vertexSet.Add(element.Item1);
            vertexSet.Add(element.Item2);
            AddEdge((element.Item1, element.Item2), element.Item3);
        }
        Vertices = new (vertexSet);
        NbVertices = vertexSet.Count;
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
        if (!AdjVertices.ContainsKey(edge.Item2))
        {
            Vertices.Add(edge.Item2);
            AdjVertices.Add(edge.Item2, []);
        }
        AdjVertices[edge.Item1].Add(edge.Item2);
        AdjVertices[edge.Item2].Add(edge.Item1);
    }

    public Graph(int nbVertices, int maxDeg, int minDeg, long seed) {
        NbVertices = nbVertices;
        AdjVertices = new();
    }

    public Graph() {
        AdjVertices = new ();
    }
    public void AddVertex(Vertex v) {
        if (AdjVertices.ContainsKey(v)) return;
        AdjVertices.TryAdd(v, new());
        Vertices.Add(v);
    }

    public void RemoveVertex(Vertex v) {
        Vertices.Remove(v);
        AdjVertices.Remove(v);
        foreach (HashSet<Vertex> listVertex in AdjVertices.Values) {
            listVertex.Remove(v);
        }
        Edges = Edges.Where((neighbor) =>  neighbor.Key.Item1 != v && neighbor.Key.Item2 != v).ToDictionary();
    }

    public object Clone()
    {
        return new Graph(Edges.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)));
    }
}