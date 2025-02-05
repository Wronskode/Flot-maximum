using System.Text;

namespace FlotMaximum;

public class Graph : ICloneable
{
    public Dictionary<Vertex, HashSet<Vertex>> AdjVertices { get; protected set; } = new();
    public Dictionary<(Vertex, Vertex), int> Edges { get; protected set;  } = new ();
    
    public Graph(IEnumerable<(Vertex?, Vertex?, int Value)> neighbors, IEnumerable<Vertex?> vertices) {
        HashSet<Vertex> vertexSet = [];
        foreach (var (u, v, value) in neighbors) {
            if (u is null || v is null) continue;
            AddEdge((u, v), value);
        }

        foreach (Vertex? v in vertices)
        {
            if (v is null) continue;
            vertexSet.Add(v);
        }
        foreach (Vertex v in vertexSet)
        {
            AddVertex(v);
        }
    }
    
    public Graph(Dictionary<(Vertex, Vertex), int> neighbors, IEnumerable<Vertex> vertices) {
        HashSet<Vertex> vertexSet = [];
        foreach (var element in neighbors) {
            AddEdge((element.Key.Item1, element.Key.Item2), element.Value);
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
        if (!AdjVertices.TryGetValue(edge.Item1, out HashSet<Vertex>? value))
        {
            value = [];
            AdjVertices.Add(edge.Item1, value);
        }

        AdjVertices.TryAdd(edge.Item2, []);
        value.Add(edge.Item2);
    }

    public void RemoveEdge((Vertex, Vertex) edge)
    {
        if (Edges.Remove(edge))
        {
            AdjVertices[edge.Item1].Remove(edge.Item2);
        }
    }
    
    public void AddVertex(Vertex v)
    {
        AdjVertices.TryAdd(v, []);
    }

    public override string ToString()
    {
        StringBuilder output = new();
        foreach (Vertex vertex in AdjVertices.Keys)
        {
            StringBuilder txt = new();
            foreach (var edge in Edges)
            {
                if (edge.Key.Item2 == vertex)
                {
                    txt.Append(edge.Key.Item1 + ",");
                }
            }
            if (txt.Length == 0) continue;
            txt.Length -= 1;
            txt.Append(" -> " + vertex);
            output.Append(txt + "\n");
        }
        return output.ToString();
    }

    public String toString2()
    {
        string output = "";
        foreach (var ((u,v), i) in Edges)
        {
            output+=u+" -> "+v+"   poid : "+i+"\n";
        }
        return output;
    }
    
    
    
    public virtual object Clone()
    {
        return new Graph(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            AdjVertices.Keys.Select(x => x.Clone() as Vertex));
    }
}