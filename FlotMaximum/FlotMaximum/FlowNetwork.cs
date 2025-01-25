namespace FlotMaximum;

public class FlowNetwork : Graph
{
    private Vertex Source;
    private Vertex Puits;
    private List<(Vertex, int)> SourceNeighbors;
    private List<(Vertex, int)> PuitNeighbors;
    public Dictionary<(Vertex, Vertex), int> NewEdges { get; set; } = new ();

    public FlowNetwork(IEnumerable<(Vertex, Vertex, int)> neighbors, Vertex source, Vertex puits, List<
        (Vertex, int)> sourceNeighbors, List<(Vertex, int)> puitNeighbors) : base(neighbors)
    {
        Source = source;
        Puits = puits;
        SourceNeighbors = sourceNeighbors;
        PuitNeighbors = puitNeighbors;
        foreach (var edge in Edges)
        {
            NewEdges.Add(edge.Key, edge.Value);
        }

        foreach (var vertex in SourceNeighbors)
        {
            NewEdges.Add((Source, vertex.Item1), vertex.Item2);
        }
        
        foreach (var vertex in PuitNeighbors)
        {
            NewEdges.Add((vertex.Item1, Puits), vertex.Item2);
        }
    }

    public (Dictionary<(Vertex, Vertex), int>, int) FordFulkerson()
    {
        Dictionary<(Vertex, Vertex), int> flot = new();
        foreach (var edge in NewEdges.Keys)
        {
            flot.Add(edge, 0);
        }

        var Nf = GetGraphResiduel(flot);
        List<Vertex> chemin = Nf.Chemin();
        while (chemin.Count > 0)
        {
            HashSet<(Vertex, Vertex)> path = [(chemin[0], chemin[1])];
            /*if (chemin.Count == 0)
            {
                int value = 0;
                foreach (var edge in flot)
                {
                    if (edge.Key.Item1.id == Source.id)
                    {
                        value += edge.Value;
                    }
                }

                return (flot, value);
            }*/

            int delta = NewEdges[(chemin[0], chemin[1])];
            for (int i = 1; i < chemin.Count - 1; i++)
            {
                int newDelta = NewEdges[(chemin[i], chemin[i + 1])];
                path.Add((chemin[i], chemin[i + 1]));
                if (newDelta < delta)
                    delta = newDelta;
            }

            foreach (var edge in path)
            {
                if (NewEdges.Select(x => (x.Key.Item1.id, x.Key.Item2.id)).ToHashSet()
                    .Contains((edge.Item1.id, edge.Item2.id)))
                {
                    flot[(edge.Item1, edge.Item2)] += delta;
                }
                else
                {
                    flot[(edge.Item2, edge.Item1)] -= delta;
                }
            }
            Nf = GetGraphResiduel(flot);
            chemin = Nf.Chemin();
        }
        int val = 0;
        foreach (var edge in flot)
        {
            if (edge.Key.Item1.id == Source.id)
            {
                val += edge.Value;
            }
        }

        return (flot, val);
    }

    public FlowNetwork GetGraphResiduel(Dictionary<(Vertex, Vertex), int> flot)
    {
        FlowNetwork residuel = new(NewEdges.Select(x => (x.Key.Item1.Clone() as Vertex, x.Key.Item2.Clone() as Vertex, x.Value)), Source, Puits, SourceNeighbors, PuitNeighbors);
        var newEdgesNF = NewEdges.ToDictionary();
        foreach (var edge in NewEdges)
        {
            int capacity = edge.Value;
            int currentFlow = flot[(edge.Key.Item1, edge.Key.Item2)];
            newEdgesNF[(edge.Key.Item1, edge.Key.Item2)] = capacity-currentFlow;
            if (edge.Key.Item2.id != Puits.id && edge.Key.Item1.id != Source.id)
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
                    }
                }
            }
            if (newVertex.Count == 0) break;
            chemin.Add(newVertex);
        }

        if (chemin.Last().Select(x => x.Item1).Contains(Puits))
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
}