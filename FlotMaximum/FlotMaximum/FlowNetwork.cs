using System.Text;
namespace FlotMaximum;

public class FlowNetwork : Graph
{
    public Vertex Source { get; }
    public Vertex Puits { get; }
    public List<(Vertex, int)> SourceNeighbors { get; }
    public List<(Vertex, int)> PuitsNeighbors { get; }

    public FlowNetwork(IEnumerable<(Vertex, Vertex, int)> neighbors, Vertex source, Vertex puits, IEnumerable<
        (Vertex, int)> sourceNeighbors, IEnumerable<(Vertex, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        InitFlowNetwork();
    }
    
    public FlowNetwork(Dictionary<(Vertex, Vertex), int> neighbors, Vertex source, Vertex puits, IEnumerable<
        (Vertex, int)> sourceNeighbors, IEnumerable<(Vertex, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        InitFlowNetwork();
    }

    private void InitFlowNetwork()
    {
        Dictionary<(Vertex, Vertex), int> newEdges = new(Edges);
        Dictionary<Vertex, HashSet<Vertex>> newAdjVertices = [];
        foreach (var vertex in AdjVertices)
        {
            newAdjVertices.Add(vertex.Key, vertex.Value);
        }
        newAdjVertices.TryAdd(Source, []);
        newAdjVertices.TryAdd(Puits, []);
        foreach (var vertex in SourceNeighbors)
        {
            newEdges.TryAdd((Source, vertex.Item1), vertex.Item2);
            newAdjVertices[Source].Add(vertex.Item1);
        }
        
        foreach (var vertex in PuitsNeighbors)
        {
            newEdges.TryAdd((vertex.Item1, Puits), vertex.Item2);
            newAdjVertices[vertex.Item1].Add(Puits);
        }

        Edges = newEdges;
        AdjVertices = newAdjVertices;
    }

    public Flow FordFulkerson()
    {
        return GetMaxFlow((nf) => nf.CheminDfs());
    }
    
    public Flow EdmondsKarp()
    {
        return GetMaxFlow((nf) => nf.CheminBfs());
    }
    public Flow GetMaxFlow(Func<FlowNetwork, List<Vertex>> getPath)
    {
        Flow flot = new(Edges.ToDictionary(edge => edge.Key, _ => 0), Puits);
        FlowNetwork nf = GetResidualNetwork(flot);
        List<Vertex> chemin = getPath(nf);

        while (chemin.Count > 0)
        {
            int delta = int.MaxValue;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var edge = (chemin[i], chemin[i + 1]);

                if (nf.Edges.TryGetValue(edge, out int capacity))
                {
                    delta = Math.Min(delta, capacity);
                }
            }
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var forwardEdge = (chemin[i], chemin[i + 1]);
                var backwardEdge = (chemin[i + 1], chemin[i]);

                if (Edges.ContainsKey(forwardEdge))
                {
                    flot.IncreaseFlow(forwardEdge, delta);
                }
                else if (flot.FlowEdges.ContainsKey(backwardEdge))
                {
                    flot.DecreaseFlow(backwardEdge, delta);
                }
            }
            
            nf = GetResidualNetwork(flot);
            chemin = getPath(nf);
        }
        return flot;
    }


    public FlowNetwork GetResidualNetwork(Flow  flot)
    {
        Dictionary<(Vertex, Vertex), int> newEdgesNf = [];
        foreach (var edge in Edges)
        {
            int capacity = edge.Value;
            int currentFlow = flot.FlowEdges[(edge.Key.Item1, edge.Key.Item2)];
            int remainFlow = capacity - currentFlow;
            if (!newEdgesNf.TryAdd((edge.Key.Item1, edge.Key.Item2), remainFlow))
            {
                newEdgesNf[(edge.Key.Item1, edge.Key.Item2)] += remainFlow;
            }
            if (currentFlow == 0 || edge.Key.Item2 == Puits || edge.Key.Item1 == Source) continue;
            var reverseEdge = (edge.Key.Item2, edge.Key.Item1);
            if (!newEdgesNf.TryAdd(reverseEdge, currentFlow))
            {
                newEdgesNf[reverseEdge] += currentFlow;
            }
        }
        newEdgesNf = newEdgesNf.Where(x => x.Value > 0).ToDictionary();
        var residuel = new FlowNetwork(newEdgesNf, Source, Puits, [], [],[]);
        return residuel;
    }
    
    // Trouver chemin avec parcours en profondeur (Ford-Fulkerson)
    public List<Vertex> CheminDfs()
    {
        Stack<(Vertex, List<Vertex>)> pile = [];
        pile.Push((Source, [Source]));
        HashSet<Vertex> marked = [];
        while (pile.Count > 0)
        {
            var (s, path) = pile.Pop();
            if (!marked.Contains(s))
            {
                if (s == Puits)
                {
                    return path;
                }
                marked.Add(s);
                foreach (var t in AdjVertices[s])
                {
                    if (Edges.TryGetValue((s, t), out int capacity) && capacity > 0)
                    {
                        pile.Push((t, [..path, t]));
                    }
                }
            }
        }
        return [];
    }
    
    // Edmonds-Karp
    public List<Vertex> CheminBfs()
    {
        Dictionary<Vertex, Vertex> parents = new();
        Queue<Vertex> queue = new();
        HashSet<Vertex> visited = new();
        queue.Enqueue(Source);
        visited.Add(Source);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var t in AdjVertices[current])
            {
                if (Edges.TryGetValue((current, t), out int capacity) && capacity > 0 && !visited.Contains(t))
                {
                    queue.Enqueue(t);
                    visited.Add(t);
                    parents[t] = current;
                }
            }

        }
        if (!parents.ContainsKey(Puits))
        {
            return [];
        }
        List<Vertex> path = [Puits];
        var currentVertex = Puits;

        while (currentVertex != Source)
        {
            currentVertex = parents[currentVertex];
            path.Insert(0, currentVertex);
        }
        return path;
    }

    public override string ToString()
    {
        Graph graph = new Graph(Edges.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)), AdjVertices.Keys);
        return graph.ToString();
    }

    public string ToMiniZinc()
    {
        StringBuilder sommetsBuilder = new StringBuilder("enum SOMMET = {\n");
        List<Vertex> newVertices = new(AdjVertices.Keys);
        foreach (var sommet in newVertices)
        {
            if (sommet == Source)
                sommetsBuilder.Append("s,");
            else if (sommet == Puits)
                sommetsBuilder.Append("p,");
            else
                sommetsBuilder.Append($"S{sommet},");
        }
        sommetsBuilder.Length--;
        sommetsBuilder.Append("\n};");
        StringBuilder matriceBuilder = new StringBuilder("array[SOMMET, SOMMET] of int: flots_max = \n[| ");
        int sumCapacity = 0;

        foreach (var u in newVertices)
        {
            foreach (var v in newVertices)
            {
                if (u == Puits && v == Source)
                {
                    matriceBuilder.Append(sumCapacity + ",");
                }
                else if (AdjVertices[u].Contains(v))
                {
                    var edgeValue = Edges[(u, v)];
                    matriceBuilder.Append(edgeValue + ",");
                    if (u == Source)
                        sumCapacity += edgeValue;
                }
                else
                {
                    matriceBuilder.Append("0,");
                }
            }
            matriceBuilder.Length--;
            matriceBuilder.Append("\n | ");
        }
        matriceBuilder.Length -= 4;
        matriceBuilder.Append(" |];\n");
        matriceBuilder.Append("SOMMET: source = s;\nSOMMET: puits = p;");
        string resultat = sommetsBuilder + "\n" + matriceBuilder
            + "\narray[SOMMET,SOMMET] of var int: flots;" +
            "\nconstraint forall(s1,s2 in SOMMET)(flots[s1,s2] >= 0 /\\" +
            " flots[s1,s2] <= flots_max[s1,s2]);\nconstraint forall(x in SOMMET where x != source /\\" +
            " x != puits)(\n  sum(s2 in SOMMET)(flots[x,s2]) = sum(s2 in SOMMET)(flots[s2,x])\n);" +
            "\nvar int: flot_sortie;\nconstraint flot_sortie = sum(x in SOMMET)(flots[x,puits]);" +
            "\nsolve maximize flot_sortie;";
        File.WriteAllText("maxflow.mzn", resultat);
        return resultat;
    }
    public override object Clone()
    {
        return new FlowNetwork(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            Source.Clone() as Vertex, Puits.Clone() as Vertex, SourceNeighbors.Select(x => (x.Item1.Clone() as Vertex, x.Item2)), PuitsNeighbors.Select(x => (x.Item1.Clone() as Vertex, x.Item2)),
            AdjVertices.Keys);
    }
}