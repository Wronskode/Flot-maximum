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

    public void RemoveVertex(Vertex v)
    {
        AdjVertices = AdjVertices.Where((uv, value) => uv.Key != v && !uv.Value.Contains(v)).ToDictionary();
        Edges = Edges.Where((uv) => v != uv.Key.Item1 &&  v != uv.Key.Item2).ToDictionary();
        
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

    public String ToString2()
    {
        string output = "";
        foreach (var ((u,v), i) in Edges)
        {
            output+=u+" -> "+v+"   poid : "+i+"\n";
        }
        return output;
    }

    public List<(Vertex, int)> neighborsRight (Vertex vertex)
    {
        List<(Vertex, int)> res = new List<(Vertex, int)>();
        foreach (var edge in Edges)
        {
            if (edge.Key.Item1 == vertex)
            {
                res.Add((edge.Key.Item2,edge.Value));
            }
        }
        return res;
    }
    
    public List<(Vertex, int)> neighborsLeft (Vertex vertex)
    {
        List<(Vertex, int)> res = new List<(Vertex, int)>();
        foreach (var edge in Edges)
        {
            if (edge.Key.Item2 == vertex)
            {
                res.Add((edge.Key.Item1,edge.Value));
            }
        }
        return res;
    }
    
    public virtual object Clone()
    {
        return new Graph(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            AdjVertices.Keys.Select(x => x.Clone() as Vertex));
    }
    
    public int GetNombreAretes()
    {
        return Edges.Count;
    }
    
}