using System.Text;

namespace FlotMaximum;

public class FlowNetwork : Graph
{
    private readonly Vertex Source;
    private readonly Vertex Puits;
    private List<(Vertex, int)> SourceNeighbors;
    private List<(Vertex, int)> PuitsNeighbors;
    public Dictionary<(Vertex, Vertex), int> NewEdges { get; set; } = new ();

    public FlowNetwork(IEnumerable<(Vertex, Vertex, int)> neighbors, Vertex source, Vertex puits, IEnumerable<
        (Vertex, int)> sourceNeighbors, IEnumerable<(Vertex, int)> puitsNeighbors, IEnumerable<Vertex> vertices) : base(neighbors, vertices)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors.ToList();
        PuitsNeighbors = puitsNeighbors.ToList();
        NewEdges = new(Edges);

        foreach (var vertex in SourceNeighbors)
        {
            NewEdges.Add((Source, vertex.Item1), vertex.Item2);
        }
        
        foreach (var vertex in PuitsNeighbors)
        {
            NewEdges.Add((vertex.Item1, Puits), vertex.Item2);
        }
    }

    public (Dictionary<(Vertex, Vertex), int>, int) FordFulkerson()
    {
        return GetMaxFlow((nf) => nf.Chemin());
    }
    
    public (Dictionary<(Vertex, Vertex), int>, int) EdmondsKarp()
    {
        return GetMaxFlow((nf) => nf.CheminBfs());
    }
    public (Dictionary<(Vertex, Vertex), int>, int) GetMaxFlow(Func<FlowNetwork, List<Vertex>> getPath)
    {
        Flow flot = new(NewEdges.ToDictionary(edge => edge.Key, _ => 0), Puits);
        FlowNetwork nf = GetResidualNetwork(flot);
        List<Vertex> chemin = getPath(nf);

        while (chemin.Count > 0)
        {
            int delta = int.MaxValue;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var edge = (chemin[i], chemin[i + 1]);

                if (nf.NewEdges.TryGetValue(edge, out int capacity))
                {
                    delta = Math.Min(delta, capacity);
                }
            }
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var forwardEdge = (chemin[i], chemin[i + 1]);
                var backwardEdge = (chemin[i + 1], chemin[i]);

                if (flot.FlowEdges.ContainsKey(forwardEdge))
                {
                    flot.IncreaseFlow(forwardEdge, delta);
                }
                else
                {
                    flot.DecreaseFlow(backwardEdge, delta);
                }
            }
            
            nf = GetResidualNetwork(flot);
            chemin = getPath(nf);
        }
        return (flot.FlowEdges, flot.Value);
    }


    public FlowNetwork GetResidualNetwork(Flow  flot)
    {
        FlowNetwork residuel = new(Edges.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)), Source, Puits, SourceNeighbors, PuitsNeighbors, Vertices);
        Dictionary<(Vertex, Vertex), int> newEdgesNf = new(NewEdges);
        foreach (var edge in NewEdges)
        {
            int capacity = edge.Value;
            int currentFlow = flot.FlowEdges[(edge.Key.Item1, edge.Key.Item2)];
            newEdgesNf[(edge.Key.Item1, edge.Key.Item2)] = capacity-currentFlow;
            if (edge.Key.Item2 != Puits && edge.Key.Item1 != Source)
            {
                bool ok = newEdgesNf.TryAdd((edge.Key.Item2, edge.Key.Item1), currentFlow);
                if (!ok)
                {
                    newEdgesNf[(edge.Key.Item1, edge.Key.Item2)] += currentFlow;
                }
            }
        }
        newEdgesNf = newEdgesNf.Where(x => x.Value > 0).ToDictionary();
        residuel.NewEdges = newEdgesNf;
        return residuel;
    }

    public List<Vertex> Chemin()
    {
        List<HashSet< (Vertex, Vertex)>> chemin = [[(Source, Source)]];
        while (true)
        {
            HashSet<(Vertex, Vertex)> newVertex = [];
            bool puitsReached = false;
            foreach (var edge in NewEdges)
            {
                if (!chemin.Last().Select(x => x.Item1).Contains(edge.Key.Item2) && chemin.Last().Select(x => x.Item1).Contains(edge.Key.Item1))
                {
                    bool isContained = false;
                    foreach (var path in chemin)
                    {
                        if (path.Select(x => x.Item1).Contains(edge.Key.Item2))
                        {
                            isContained = true;
                            break;
                        }
                    }

                    if (!isContained)
                    {
                        newVertex.Add((edge.Key.Item2, edge.Key.Item1));
                        if (edge.Key.Item2 == Puits) 
                            puitsReached = true;
                    }
                }
            }
            if (newVertex.Count == 0) break;
            chemin.Add(newVertex);
            if (puitsReached) break;
        }

        if (chemin.SelectMany(x => x).Select(x => x.Item1).Contains(Puits))
        {
            List<Vertex> path = [Puits];
            int lenght = chemin.Count - 1;
            while (lenght > 0)
            {
                foreach (var edge in chemin[lenght])
                {
                    if (edge.Item1 == path[0])
                    {
                        path.Insert(0, edge.Item2);
                        lenght--;
                        break;
                    }
                }
            }
            return path;
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

            foreach (var edge in NewEdges)
            {
                var start = edge.Key.Item1;
                var end = edge.Key.Item2;
                var capacity = edge.Value;
                if (start == current && capacity > 0 && !visited.Contains(end))
                {
                    queue.Enqueue(end);
                    visited.Add(end);
                    parents[end] = current;
                    if (end == Puits)
                    {
                        break;
                    }
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
        Graph graph = new Graph(NewEdges.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)), Vertices);
        return graph.ToString();
    }

    public string ToMiniZinc()
{
    StringBuilder sommetsBuilder = new StringBuilder("enum SOMMET = {\n");
    var newVertices = new List<Vertex>(Vertices) { Source, Puits };
    var newAdjVertices = new Dictionary<Vertex, HashSet<Vertex>>(AdjVertices)
    {
        [Source] = SourceNeighbors.Select(x => x.Item1).ToHashSet()
    };

    foreach (var neighbor in PuitsNeighbors.Select(x => x.Item1))
    {
        if (!newAdjVertices.ContainsKey(neighbor))
        {
            newAdjVertices[neighbor] = new HashSet<Vertex> { Puits };
        }
        else
        {
            newAdjVertices[neighbor].Add(Puits);
        }
    }
    newAdjVertices[Puits] = new HashSet<Vertex>();
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
            else if (newAdjVertices[u].Contains(v))
            {
                var edgeValue = NewEdges[(u, v)];
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
    string resultat = sommetsBuilder.ToString() + "\n" + matriceBuilder.ToString()
        + "\narray[SOMMET,SOMMET] of var int: flots;\n\nconstraint forall(s1,s2 in SOMMET)(flots[s1,s2] >= 0 /\\ flots[s1,s2] <= flots_max[s1,s2]);\n\nconstraint forall(x in SOMMET where x != source /\\ x != puits)(\n  sum(s2 in SOMMET)(flots[x,s2]) = sum(s2 in SOMMET)(flots[s2,x])\n);\n\nvar int: flot_sortie;\nconstraint flot_sortie = sum(x in SOMMET)(flots[x,puits]);\n\nsolve maximize flot_sortie;";
    File.WriteAllText("maxflow.mzn", resultat);

    return resultat;
}


    public override object Clone()
    {
        return new FlowNetwork(Edges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)),
            Source.Clone() as Vertex, Puits.Clone() as Vertex, SourceNeighbors.Select(x => (x.Item1.Clone() as Vertex, x.Item2)), PuitsNeighbors.Select(x => (x.Item1.Clone() as Vertex, x.Item2)),
            Vertices.Select(x => x.Clone() as Vertex));
    }
}