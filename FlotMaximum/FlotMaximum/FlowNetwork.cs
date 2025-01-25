namespace FlotMaximum;

public class FlowNetwork : Graph
{
    private Vertex Source;
    private Vertex Puits;
    private List<(Vertex, int)> SourceNeighbors;
    private List<(Vertex, int)> PuitsNeighbors;
    public Dictionary<(Vertex, Vertex), int> NewEdges { get; set; } = new ();

    public FlowNetwork(IEnumerable<(Vertex, Vertex, int)> neighbors, Vertex source, Vertex puits, List<
        (Vertex, int)> sourceNeighbors, List<(Vertex, int)> puitsNeighbors) : base(neighbors)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors;
        PuitsNeighbors = puitsNeighbors;
        foreach (var edge in Edges)
        {
            NewEdges.Add(edge.Key, edge.Value);
        }

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
        return GetMaxFlow((nf) => nf.CheminBFS());
    }
    public (Dictionary<(Vertex, Vertex), int>, int) GetMaxFlow(Func<FlowNetwork, List<Vertex>> getPath)
    {
        Dictionary<(Vertex, Vertex), int> flot = NewEdges.ToDictionary(edge => edge.Key, edge => 0);
        FlowNetwork Nf = GetGraphResiduel(flot);
        List<Vertex> chemin = getPath(Nf);

        while (chemin.Count > 0)
        {
            int delta = int.MaxValue;
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var edge = (chemin[i], chemin[i + 1]);

                if (Nf.NewEdges.TryGetValue(edge, out int capacity))
                {
                    delta = Math.Min(delta, capacity);
                }
            }
            
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                var forwardEdge = (chemin[i], chemin[i + 1]);
                var backwardEdge = (chemin[i + 1], chemin[i]);

                if (flot.ContainsKey(forwardEdge))
                {
                    flot[forwardEdge] += delta;
                }
                else
                {
                    flot[backwardEdge] -= delta;
                }
            }
            
            Nf = GetGraphResiduel(flot);
            chemin = getPath(Nf);
        }
        int valeurFlot = flot.Where(edge => edge.Key.Item2 == Puits).Sum(edge => edge.Value);
        return (flot, valeurFlot);
    }


    public FlowNetwork GetGraphResiduel(Dictionary<(Vertex, Vertex), int> flot)
    {
        FlowNetwork residuel = new(Edges.Select(x => (x.Key.Item1, x.Key.Item2, x.Value)), Source, Puits, SourceNeighbors, PuitsNeighbors);
        var newEdgesNF = NewEdges.ToDictionary();
        foreach (var edge in NewEdges)
        {
            int capacity = edge.Value;
            int currentFlow = flot[(edge.Key.Item1, edge.Key.Item2)];
            newEdgesNF[(edge.Key.Item1, edge.Key.Item2)] = capacity-currentFlow;
            if (edge.Key.Item2 != Puits && edge.Key.Item1 != Source)
            {
                bool ok = newEdgesNF.TryAdd((edge.Key.Item2, edge.Key.Item1), currentFlow);
                if (!ok)
                {
                    newEdgesNF[(edge.Key.Item1, edge.Key.Item2)] += currentFlow;
                }
            }
        }
        newEdgesNF = newEdgesNF.Where(x => x.Value > 0).ToDictionary();
        residuel.NewEdges = newEdgesNF;
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
    public List<Vertex> CheminBFS()
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
}